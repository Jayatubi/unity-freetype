Shader "Unlit/FreeType Text"
{
	Properties
	{
		_MainTex ("Alpha (A)", 2D) = "white" {}
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Offset -1, -1
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "Unlit - FreeType Text.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : POSITION;
					float4 color1 : COLOR;
					float4 color2 : COLOR2;
					float2 texcoord : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					decodeColor(v.color, o.color1, o.color2);
					o.texcoord = v.texcoord;					

					return o;
				}

				float4 frag (v2f i) : COLOR
				{
					float2 alpha = decodeAlpha(tex2D(_MainTex, i.texcoord)) * float2(i.color1.a, i.color2.a * i.color1.a);
					float4 col;
					col.rgb = i.color1.rgb * alpha.x + i.color2.rgb * alpha.y * (1 - alpha.x);
					col.a = max(alpha.x, alpha.y);

					return col;
				}
			ENDCG
		}
	}
}
