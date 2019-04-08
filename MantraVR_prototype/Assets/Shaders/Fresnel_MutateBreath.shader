// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fresnel_Mutatebreath"
{
	Properties
	{
		_Colorbase("Color base", Color) = (1,0.5514706,0.950507,0)
		_Value2("Value 2", Range( -2 , 2)) = -1
		_Colorrim("Color rim", Color) = (0.8161765,0.9923934,1,0)
		[Toggle]_Cleancut("Clean cut?", Float) = 0
		_Power("Power", Range( 0 , 10)) = 1
		_Bias("Bias", Range( 0 , 1)) = 0.5
		_lumi("lumi", Range( 0 , 5)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _Colorbase;
		uniform float4 _Colorrim;
		uniform float _Cleancut;
		uniform float _Bias;
		uniform float globalBreathValue;
		uniform float _Value2;
		uniform float _Power;
		uniform float _lumi;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float temp_output_99_0 = ( ( globalBreathValue * _Value2 ) + 0.5 );
			float fresnelNdotV1 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode1 = ( _Bias + temp_output_99_0 * pow( 1.0 - fresnelNdotV1, (10.0 + (_Power - 0.0) * (0.0 - 10.0) / (10.0 - 0.0)) ) );
			float temp_output_91_0 = saturate( fresnelNode1 );
			float4 lerpResult87 = lerp( _Colorbase , _Colorrim , lerp(temp_output_91_0,round( temp_output_91_0 ),_Cleancut));
			o.Albedo = lerpResult87.rgb;
			o.Emission = ( lerpResult87 * ( temp_output_99_0 + _lumi ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

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
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
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
Version=16400
314;178;1081;1004;1090.308;631.2419;1.293704;True;False
Node;AmplifyShaderEditor.RangedFloatNode;102;-696.4727,-543.1;Float;False;Global;globalBreathValue;globalBreathValue;3;0;Create;True;0;0;False;0;1;6;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-632.473,-463.1004;Float;False;Property;_Value2;Value 2;1;0;Create;True;0;0;False;0;-1;0.3;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1340.242,252.2057;Float;False;Property;_Power;Power;4;0;Create;True;0;0;False;0;1;8.8;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-359.322,-541.9721;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;-360.4737,-415.1004;Float;False;Constant;_Value1;Value 1;0;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-1340.242,90.2052;Float;False;Property;_Bias;Bias;5;0;Create;True;0;0;False;0;0.5;0.321;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;13;-1030.951,233.5843;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;10;False;3;FLOAT;10;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;99;-104.4735,-511.1005;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;1;-848.2396,49.2052;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;91;-569.332,202.9615;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;92;-567.332,267.9615;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;93;-390.332,257.9615;Float;False;Property;_Cleancut;Clean cut?;3;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;85;-819.1877,-120.9927;Float;False;Property;_Colorrim;Color rim;2;0;Create;True;0;0;False;0;0.8161765,0.9923934,1,0;1,0.3254712,0.8333518,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;86;-819.7497,-289.2398;Float;False;Property;_Colorbase;Color base;0;0;Create;True;0;0;False;0;1,0.5514706,0.950507,0;1,0.9333333,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;105;-297.2715,-277.9151;Float;False;Property;_lumi;lumi;7;0;Create;True;0;0;False;0;0;1.09;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;87;-183.332,-9.038513;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;104;110.3137,-335.71;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1339.242,171.2053;Float;False;Property;_Intensity;Intensity;6;0;Create;True;0;0;False;0;0.5;0.6;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;114.8414,62.23593;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;311.5398,-0.6759262;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Fresnel_Mutatebreath;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;2;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;103;0;102;0
WireConnection;103;1;101;0
WireConnection;13;0;4;0
WireConnection;99;0;103;0
WireConnection;99;1;100;0
WireConnection;1;1;2;0
WireConnection;1;2;99;0
WireConnection;1;3;13;0
WireConnection;91;0;1;0
WireConnection;92;0;91;0
WireConnection;93;0;91;0
WireConnection;93;1;92;0
WireConnection;87;0;86;0
WireConnection;87;1;85;0
WireConnection;87;2;93;0
WireConnection;104;0;99;0
WireConnection;104;1;105;0
WireConnection;96;0;87;0
WireConnection;96;1;104;0
WireConnection;0;0;87;0
WireConnection;0;2;96;0
ASEEND*/
//CHKSM=06EFCF1D53317FE90A9DEF2F87D637F6F64598A7