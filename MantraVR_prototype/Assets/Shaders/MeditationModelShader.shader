// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MeditationModelShader"
{
	Properties
	{
		_DefaultColor("Default Color", Color) = (1,1,1,0)
		_BaseColor("Base Color", Color) = (1,1,1,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _BaseColor;
		uniform float Pitch;
		uniform float4 _DefaultColor;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 temp_output_7_0 = ( _BaseColor + float4( 0,0,0,0 ) + (( ase_vertex3Pos.z > Pitch ) ? _DefaultColor :  float4( 0,0,0,0 ) ) );
			o.Albedo = temp_output_7_0.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
-1529.6;6.4;1523;790;1870.331;572.8316;1.427967;True;False
Node;AmplifyShaderEditor.RangedFloatNode;14;-3159.497,-679.2922;Float;False;Global;Pitch;Pitch;2;0;Create;True;0;0;False;0;-0.25;-0.0194771;-0.25;0.02;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;16;-2223.579,-1000.011;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;13;-3123.423,-216.2416;Float;False;Property;_DefaultColor;Default Color;1;0;Create;True;0;0;False;0;1,1,1,0;0.5660378,0.5660378,0.5660378,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareGreater;5;-1682.77,-154.155;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;2;-1709.341,-1083.968;Float;False;Property;_BaseColor;Base Color;2;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-793.0726,-352.489;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-719.3535,383.6359;Float;False;Property;_Float0;Float 0;4;0;Create;True;0;0;False;0;0;1.41;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareLowerEqual;3;-1691.252,-596.3622;Float;True;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-369.8299,280.8024;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-546.5428,0.5786896;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-903.7287,139.6234;Float;False;Property;_GlowMultiplier;Glow Multiplier;3;0;Create;True;0;0;False;0;0;6;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-3180.199,-6.139603;Float;False;Global;Volume;Volume;4;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;4;-2177.736,-556.062;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;9;-3121.631,-418.1367;Float;False;Global;GlowEndColor;Glow End Color;0;0;Create;True;0;0;False;0;0,1,0,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;10;-3129.286,-594.2914;Float;False;Property;_GlowStartColor;Glow Start Color;0;0;Create;True;0;0;False;0;0,0,0,0;0.5660378,0.5660378,0.5660378,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareGreater;15;-2862.624,80.78236;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-3112.639,144.6167;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MeditationModelShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;16;3
WireConnection;5;1;14;0
WireConnection;5;2;13;0
WireConnection;7;0;2;0
WireConnection;7;2;5;0
WireConnection;3;0;16;3
WireConnection;3;1;14;0
WireConnection;3;2;4;0
WireConnection;17;0;3;0
WireConnection;17;1;18;0
WireConnection;6;0;7;0
WireConnection;6;1;15;0
WireConnection;6;2;8;0
WireConnection;4;0;10;0
WireConnection;4;1;9;0
WireConnection;4;2;15;0
WireConnection;15;0;12;0
WireConnection;15;1;11;0
WireConnection;15;2;12;0
WireConnection;15;3;11;0
WireConnection;0;0;7;0
ASEEND*/
//CHKSM=034CD508005B6FAD60D4271DAC19253DEA26E67B