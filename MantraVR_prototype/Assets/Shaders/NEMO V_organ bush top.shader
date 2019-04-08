// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NEMO/NEMO V_organ bush top"
{
	Properties
	{
		_Basecolor("Base color", Color) = (0,1,0,0)
		_Glowcolor("Glow color", Color) = (1,0,0,0)
		_Luminositygradient("Luminosity gradient", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Basecolor;
		uniform float globalBreathValue;
		uniform float4 _Glowcolor;
		uniform float _Luminositygradient;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _Basecolor.rgb;
			float2 uv_TexCoord19 = i.uv_texcoord * float2( 2,2 );
			float clampResult8 = clamp( ( (0.5 + (uv_TexCoord19.y - 0.0) * (1.0 - 0.5) / (1.0 - 0.0)) + ( ( globalBreathValue * -1.0 ) + 0.5 ) ) , -10.0 , 1.0 );
			float3 lerpResult3 = lerp( float3(1,1,1) , float3(0,0,0) , clampResult8);
			o.Emission = ( float4( lerpResult3 , 0.0 ) * _Glowcolor * _Luminositygradient ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
597;195;765;755;2435.948;-15.2021;1.398473;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-1791.963,384.0615;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-1872.968,515.8823;Float;False;Global;globalBreathValue;globalBreathValue;3;0;Create;True;0;0;False;0;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1809.04,597.9138;Float;False;Constant;_Value2;Value 2;0;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1534.252,643.1862;Float;False;Constant;_Value1;Value 1;0;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;16;-1534.882,386.0811;Float;False;True;False;False;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1534.252,545.9625;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;10;-1280.482,384.5322;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-1279.815,545.2048;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-1023.333,386.0732;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;7;-895.4084,136.39;Float;False;Constant;_Vector1;Vector 1;0;0;Create;True;0;0;False;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;6;-895.4085,0.7598938;Float;False;Constant;_Vector0;Vector 0;0;0;Create;True;0;0;False;0;1,1,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ClampOpNode;8;-895.4086,384.532;Float;False;3;0;FLOAT;0;False;1;FLOAT;-10;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-638.5316,385.1498;Float;False;Property;_Glowcolor;Glow color;1;0;Create;True;0;0;False;0;1,0,0,0;1,0.6431373,0,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-651.6028,619.7756;Float;False;Property;_Luminositygradient;Luminosity gradient;2;0;Create;True;0;0;False;0;0;0.72;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;3;-639.5802,255.781;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-256.4624,256.8635;Float;False;3;3;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1;-511.4597,0.9147674;Float;False;Property;_Basecolor;Base color;0;0;Create;True;0;0;False;0;0,1,0,0;0,0.3793091,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;NEMO/NEMO V_organ bush top;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;16;0;19;2
WireConnection;12;0;14;0
WireConnection;12;1;15;0
WireConnection;10;0;16;0
WireConnection;11;0;12;0
WireConnection;11;1;13;0
WireConnection;9;0;10;0
WireConnection;9;1;11;0
WireConnection;8;0;9;0
WireConnection;3;0;6;0
WireConnection;3;1;7;0
WireConnection;3;2;8;0
WireConnection;2;0;3;0
WireConnection;2;1;4;0
WireConnection;2;2;5;0
WireConnection;0;0;1;0
WireConnection;0;2;2;0
ASEEND*/
//CHKSM=EC55DA57E4EAE4BAED79FC3A8532881EC2B9AECD