// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fresnel&gradient"
{
	Properties
	{
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 27.2
		_BASEcolor1("BASE color 1", Color) = (1,0,0,0)
		_BASEcolor2("BASE color 2", Color) = (0.03448248,0,1,0)
		[Toggle]_BASEcolor1only("BASE color 1 only", Float) = 0
		_RIMcolor1("RIM color 1", Color) = (1,1,1,0)
		_RIMcolor2("RIM color 2", Color) = (0.9912779,1,0.3676471,0)
		[Toggle]_RIMcolor1only("RIM color 1 only", Float) = 0
		_RIMpower("RIM power", Range( 0 , 10)) = 1
		_RIMbias("RIM bias", Range( 0 , 1)) = 0.5
		_RIMintensity("RIM intensity", Range( 0 , 20)) = 0.5
		[KeywordEnum(X,Y,Z,XYZ)] _Orientation("Orientation", Float) = 0
		[Toggle]_RIMcleancut("RIM clean cut?", Float) = 1
		_Distribution("Distribution", Float) = 0.2
		_Startpoint("Start point", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Background+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#pragma shader_feature _ORIENTATION_X _ORIENTATION_Y _ORIENTATION_Z _ORIENTATION_XYZ
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _BASEcolor1only;
		uniform float4 _BASEcolor1;
		uniform float4 _BASEcolor2;
		uniform float _Startpoint;
		uniform float _Distribution;
		uniform float _RIMcolor1only;
		uniform float4 _RIMcolor1;
		uniform float4 _RIMcolor2;
		uniform float _RIMcleancut;
		uniform float _RIMbias;
		uniform float _RIMintensity;
		uniform float _RIMpower;
		uniform float _EdgeLength;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 temp_cast_0 = (ase_vertex3Pos.x).xxx;
			float3 temp_cast_1 = (ase_vertex3Pos.x).xxx;
			float3 temp_cast_2 = (ase_vertex3Pos.y).xxx;
			float3 temp_cast_3 = (ase_vertex3Pos.z).xxx;
			#if defined(_ORIENTATION_X)
				float3 staticSwitch127 = temp_cast_0;
			#elif defined(_ORIENTATION_Y)
				float3 staticSwitch127 = temp_cast_2;
			#elif defined(_ORIENTATION_Z)
				float3 staticSwitch127 = temp_cast_3;
			#elif defined(_ORIENTATION_XYZ)
				float3 staticSwitch127 = ase_vertex3Pos;
			#else
				float3 staticSwitch127 = temp_cast_0;
			#endif
			float3 temp_output_96_0 = saturate( ( ( ( staticSwitch127 + _Startpoint ) + ( _Distribution / 2.0 ) ) / _Distribution ) );
			float4 lerpResult98 = lerp( _BASEcolor1 , _BASEcolor2 , temp_output_96_0.x);
			float4 lerpResult117 = lerp( _RIMcolor1 , _RIMcolor2 , temp_output_96_0.x);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV1 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode1 = ( _RIMbias + _RIMintensity * pow( 1.0 - fresnelNdotV1, (10.0 + (_RIMpower - 0.0) * (0.0 - 10.0) / (10.0 - 0.0)) ) );
			float temp_output_112_0 = saturate( fresnelNode1 );
			float4 lerpResult111 = lerp( lerp(lerpResult98,_BASEcolor1,_BASEcolor1only) , lerp(lerpResult117,_RIMcolor1,_RIMcolor1only) , lerp(temp_output_112_0,round( temp_output_112_0 ),_RIMcleancut));
			o.Emission = lerpResult111.rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15600
7;29;1906;1004;3845.519;1970.183;3.200529;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;122;-2176.268,-754.6877;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;128;-2176.445,-480.9186;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;89;-2174.633,-616.218;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;129;-2183.445,-343.9186;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;127;-1864.445,-598.9186;Float;False;Property;_Orientation;Orientation;15;0;Create;True;0;0;False;0;0;0;0;True;;KeywordEnum;4;X;Y;Z;XYZ;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;88;-1813.086,-256.5714;Float;False;Property;_Startpoint;Start point;18;0;Create;True;0;0;False;0;0;3.75;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-1815.765,-164.4707;Float;False;Property;_Distribution;Distribution;17;0;Create;True;0;0;False;0;0.2;13.64;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1642.995,589.6707;Float;False;Property;_RIMpower;RIM power;12;0;Create;True;0;0;False;0;1;8.44;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;91;-1505.039,-424.0032;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;92;-1465.665,-313.1252;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;93;-1321.064,-423.4255;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1641.995,508.6702;Float;False;Property;_RIMintensity;RIM intensity;14;0;Create;True;0;0;False;0;0.5;0.39;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;13;-1367.302,588.6492;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;10;False;3;FLOAT;10;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-1642.995,427.6702;Float;False;Property;_RIMbias;RIM bias;13;0;Create;True;0;0;False;0;0.5;0.691;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;1;-1150.99,386.6703;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;94;-1242.016,-181.8237;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;116;-1146.803,169.7446;Float;False;Property;_RIMcolor2;RIM color 2;10;0;Create;True;0;0;False;0;0.9912779,1,0.3676471,0;1,0,0.9733333,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;97;-1147.398,-470.1396;Float;False;Property;_BASEcolor2;BASE color 2;7;0;Create;True;0;0;False;0;0.03448248,0,1,0;0.5882353,0.5607843,0.5607843,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;112;-910.3007,386.5801;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;95;-1149.812,-636.5255;Float;False;Property;_BASEcolor1;BASE color 1;5;0;Create;True;0;0;False;0;1,0,0,0;0.5882352,0.5622836,0.5622836,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;109;-1150.986,3.602875;Float;False;Property;_RIMcolor1;RIM color 1;9;0;Create;True;0;0;False;0;1,1,1,0;0.862069,1,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;96;-1088.849,-295.6947;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;98;-929.0599,-553.3591;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;117;-924.8112,98.72144;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RoundOpNode;113;-908.3007,451.5797;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;114;-764.3008,383.5798;Float;False;Property;_RIMcleancut;RIM clean cut?;16;0;Create;True;0;0;False;0;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;115;-765.2496,-638.4308;Float;False;Property;_BASEcolor1only;BASE color 1 only;8;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;118;-767.26,3.687279;Float;False;Property;_RIMcolor1only;RIM color 1 only;11;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;111;-343.9131,-300.6735;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1,-7;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Fresnel&gradient;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Opaque;;Background;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;27.2;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;6;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;127;1;122;1
WireConnection;127;0;89;2
WireConnection;127;2;128;3
WireConnection;127;3;129;0
WireConnection;91;0;127;0
WireConnection;91;1;88;0
WireConnection;92;0;90;0
WireConnection;93;0;91;0
WireConnection;93;1;92;0
WireConnection;13;0;4;0
WireConnection;1;1;2;0
WireConnection;1;2;3;0
WireConnection;1;3;13;0
WireConnection;94;0;93;0
WireConnection;94;1;90;0
WireConnection;112;0;1;0
WireConnection;96;0;94;0
WireConnection;98;0;95;0
WireConnection;98;1;97;0
WireConnection;98;2;96;0
WireConnection;117;0;109;0
WireConnection;117;1;116;0
WireConnection;117;2;96;0
WireConnection;113;0;112;0
WireConnection;114;0;112;0
WireConnection;114;1;113;0
WireConnection;115;0;98;0
WireConnection;115;1;95;0
WireConnection;118;0;117;0
WireConnection;118;1;109;0
WireConnection;111;0;115;0
WireConnection;111;1;118;0
WireConnection;111;2;114;0
WireConnection;0;2;111;0
ASEEND*/
//CHKSM=98E10CAE03418A518480D92A0D12D197C86A8407