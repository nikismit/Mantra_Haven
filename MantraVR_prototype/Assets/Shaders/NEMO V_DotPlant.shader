// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NEMO/NEMO_DotPlant"
{
	Properties
	{
		_Basecolor("Base color", Color) = (0,1,0,0)
		_Value2("Value 2", Range( -3 , 3)) = -1
		_glowcolor("glow color", Color) = (0,1,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			half filler;
		};

		uniform float4 _Basecolor;
		uniform float globalBreathValue;
		uniform float _Value2;
		uniform float4 _glowcolor;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _Basecolor.rgb;
			o.Emission = ( ( globalBreathValue * _Value2 ) + _glowcolor ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
7;27;1081;984;1336.277;153.8568;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;14;-1114.407,512.1179;Float;False;Global;globalBreathValue;globalBreathValue;3;0;Create;True;0;0;False;0;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1054.244,592.2671;Float;False;Property;_Value2;Value 2;1;0;Create;True;0;0;False;0;-1;-1;-3;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-779.4563,540.3158;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;20;-645.4537,181.1989;Float;False;Property;_glowcolor;glow color;2;0;Create;True;0;0;False;0;0,1,0,0;0.9577581,1,0.476415,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-511.4597,0.9147674;Float;False;Property;_Basecolor;Base color;0;0;Create;True;0;0;False;0;0,1,0,0;1,0.8401979,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-230.8083,247.038;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;NEMO/NEMO_DotPlant;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;14;0
WireConnection;12;1;15;0
WireConnection;11;0;12;0
WireConnection;11;1;20;0
WireConnection;0;0;1;0
WireConnection;0;2;11;0
ASEEND*/
//CHKSM=2A6988515A039EC8453C2190DA66AF0DE5006147