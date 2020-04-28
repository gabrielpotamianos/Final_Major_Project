// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Quantum Theory/PolyWorld/PolyWorld SurfaceNoise"
{
	Properties
	{
		_OffsetMap("Offset Map", 2D) = "white" {}
		_Intensity("Intensity", Range( 0 , 1)) = 0.15
		_Speed("Speed", Range( 0 , 1)) = 0.15
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 2.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float4 vertexColor : COLOR;
		};

		uniform float _Metallic;
		uniform float _Smoothness;
		uniform sampler2D _OffsetMap;
		uniform float4 _OffsetMap_ST;
		uniform float _Speed;
		uniform float _Intensity;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 uv_OffsetMap = v.texcoord.xy * _OffsetMap_ST.xy + _OffsetMap_ST.zw;
			float mulTime10 = _Time.y * 1;
			v.vertex.xyz += ( ( (-1 + (tex2Dlod( _OffsetMap, float4( ( uv_OffsetMap + ( mulTime10 * _Speed ) ), 0, 0) ).r - 0) * (1 - -1) / (1 - 0)) * ( _Intensity * float3(0,2,0) ) ) * v.color.a );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = i.vertexColor.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=14504
2487;533;1398;819;2155.073;37.45993;1.3;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;10;-1472.81,591.2587;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1574.81,662.2587;Float;False;Property;_Speed;Speed;2;0;Create;True;0;0;False;0;0.15;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-1512.843,463.8204;Float;False;0;13;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1311.81,614.2587;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-1155.81,477.2583;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1317.51,745.1077;Float;False;Property;_Intensity;Intensity;1;0;Create;True;0;0;False;0;0.15;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-1021.096,464.3003;Float;True;Property;_OffsetMap;Offset Map;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;18;-1223.394,939.0427;Float;False;Constant;_Vector0;Vector 0;7;0;Create;True;0;0;False;0;0,2,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TFHCRemapNode;14;-724.8217,482.4787;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-908.2501,846.3531;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-521.0535,699.5442;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.VertexColorNode;1;-684,-221.5;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-282.8831,495.1934;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-506,35.5;Float;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-423,134.5;Float;False;Property;_Metallic;Metallic;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;0;Float;;0;0;Standard;Quantum Theory/PolyWorld/PolyWorld SurfaceNoise;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;0;False;-1;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;10;0
WireConnection;11;1;12;0
WireConnection;9;0;8;0
WireConnection;9;1;11;0
WireConnection;13;1;9;0
WireConnection;14;0;13;1
WireConnection;17;0;16;0
WireConnection;17;1;18;0
WireConnection;15;0;14;0
WireConnection;15;1;17;0
WireConnection;20;0;15;0
WireConnection;20;1;1;4
WireConnection;0;0;1;0
WireConnection;0;3;2;0
WireConnection;0;4;3;0
WireConnection;0;11;20;0
ASEEND*/
//CHKSM=91B66FD13E1CDA00F5D6636B8912A2E4442F9ACC