// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AS_Skybox"
{
	Properties
	{
		_SkyColor("SkyColor", Color) = (1,1,1,1)
		_HorizonColor("HorizonColor", Color) = (0,0.472,1,1)
		_SunRadiusA("Sun Radius A", Range( 0 , 0.1)) = 0.0337
		_SunRadiusB("Sun Radius B", Range( 0 , 0.1)) = 0.0289
		_SunIntensity("Sun Intensity", Float) = 2
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 viewDir;
			float3 worldPos;
		};

		uniform float4 _SkyColor;
		uniform float4 _HorizonColor;
		uniform float _SunRadiusA;
		uniform float _SunRadiusB;
		uniform float _SunIntensity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float dotResult26 = dot( i.viewDir , float3(0,-1,0) );
			float4 lerpResult32 = lerp( _SkyColor , _HorizonColor , pow( ( 1.0 - dotResult26 ) , 0.5 ));
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult10 = dot( -ase_worldlightDir , i.viewDir );
			float temp_output_3_0 = min( _SunRadiusA , _SunRadiusB );
			float temp_output_4_0 = max( _SunRadiusA , _SunRadiusB );
			float clampResult17 = clamp( (0.0 + (dotResult10 - ( 1.0 - ( temp_output_3_0 * temp_output_3_0 ) )) * (1.0 - 0.0) / (( 1.0 - ( temp_output_4_0 * temp_output_4_0 ) ) - ( 1.0 - ( temp_output_3_0 * temp_output_3_0 ) ))) , 0.0 , 1.0 );
			o.Emission = ( lerpResult32 + ( ase_lightColor * pow( clampResult17 , 5.0 ) * _SunIntensity ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
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
				surfIN.viewDir = worldViewDir;
				surfIN.worldPos = worldPos;
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
Version=15401
259;257;980;645;63.09189;845.4298;2.003868;True;False
Node;AmplifyShaderEditor.RangedFloatNode;2;-520.2704,-43.29579;Float;False;Property;_SunRadiusB;Sun Radius B;3;0;Create;True;0;0;False;0;0.0289;0.0289;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-517.4671,-129.5;Float;False;Property;_SunRadiusA;Sun Radius A;2;0;Create;True;0;0;False;0;0.0337;0.0337;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;4;-181.2934,-37.4734;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;7;-311.9932,-410.8976;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMinOpNode;3;-179.2923,-147.5309;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-17.67001,-35.74944;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;8;-19.67061,-408.4616;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;9;-53.77493,-311.0208;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-15.234,-145.3704;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;10;189.8273,-328.0728;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;165.7814,90.92293;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;24;124.5912,-714.07;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;16;168.2174,178.6197;Float;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;11;168.2173,-135.6271;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;12;165.7813,-35.7502;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;25;129.8695,-555.7261;Float;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;False;0;0,-1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TFHCRemapNode;14;411.8199,-225.7597;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;26;392.0145,-636.6573;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;635.6915,-78.67297;Float;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;17;630.3745,-229.5069;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;585.5447,-538.1322;Float;False;Constant;_Float3;Float 3;3;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;27;583.7858,-636.6571;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;30;766.7585,-1046.596;Float;False;Property;_SkyColor;SkyColor;0;0;Create;True;0;0;False;0;1,1,1,1;0,0,0,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;22;802.6997,-21.3757;Float;False;Property;_SunIntensity;Sun Intensity;4;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;31;770.2792,-865.3772;Float;False;Property;_HorizonColor;HorizonColor;1;0;Create;True;0;0;False;0;0,0.472,1,1;0,0,0,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;29;810.7449,-610.2667;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;19;826.1009,-163.6629;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;20;812.8177,-370.4418;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LerpOp;32;1053.54,-786.2059;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;1074.195,-201.8108;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;1396.524,-492.2318;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;34;1598.995,-537.7471;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;AS_Skybox;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;1;0
WireConnection;4;1;2;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;6;0;4;0
WireConnection;6;1;4;0
WireConnection;8;0;7;0
WireConnection;5;0;3;0
WireConnection;5;1;3;0
WireConnection;10;0;8;0
WireConnection;10;1;9;0
WireConnection;11;0;5;0
WireConnection;12;0;6;0
WireConnection;14;0;10;0
WireConnection;14;1;11;0
WireConnection;14;2;12;0
WireConnection;14;3;13;0
WireConnection;14;4;16;0
WireConnection;26;0;24;0
WireConnection;26;1;25;0
WireConnection;17;0;14;0
WireConnection;27;0;26;0
WireConnection;29;0;27;0
WireConnection;29;1;28;0
WireConnection;19;0;17;0
WireConnection;19;1;18;0
WireConnection;32;0;30;0
WireConnection;32;1;31;0
WireConnection;32;2;29;0
WireConnection;21;0;20;0
WireConnection;21;1;19;0
WireConnection;21;2;22;0
WireConnection;23;0;32;0
WireConnection;23;1;21;0
WireConnection;34;2;23;0
ASEEND*/
//CHKSM=122F197EB79103BFA80AE06BBFE3225369C72DBB