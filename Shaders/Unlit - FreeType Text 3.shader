Shader "Unlit/FreeType Text 3"
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
                    float4 worldPos : TEXCOORD1;
                    float2 worldPos2 : TEXCOORD2;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
                float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
                float4 _ClipArgs0 = float4(1000.0, 1000.0, 0.0, 1.0);
                float4 _ClipRange1 = float4(0.0, 0.0, 1.0, 1.0);
                float4 _ClipArgs1 = float4(1000.0, 1000.0, 0.0, 1.0);
                float4 _ClipRange2 = float4(0.0, 0.0, 1.0, 1.0);
                float4 _ClipArgs2 = float4(1000.0, 1000.0, 0.0, 1.0);

                float2 Rotate (float2 v, float2 rot)
                {
                    float2 ret;
                    ret.x = v.x * rot.y - v.y * rot.x;
                    ret.y = v.x * rot.x + v.y * rot.y;
                    return ret;
                }

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					decodeColor(v.color, o.color1, o.color2);
					o.texcoord = v.texcoord;
                    o.worldPos.xy = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
                    o.worldPos.zw = Rotate(v.vertex.xy, _ClipArgs1.zw) * _ClipRange1.zw + _ClipRange1.xy;
                    o.worldPos2 = Rotate(v.vertex.xy, _ClipArgs2.zw) * _ClipRange2.zw + _ClipRange2.xy;
    			
					return o;
				}

				float4 frag (v2f i) : COLOR
				{
					float2 alpha = decodeAlpha(tex2D(_MainTex, i.texcoord)) * float2(i.color1.a, i.color2.a * i.color1.a);
					float4 col;
					col.rgb = i.color1.rgb * alpha.x + i.color2.rgb * alpha.y * (1 - alpha.x);
					col.a = max(alpha.x, alpha.y);
					
                    // First clip region
                    float2 factor = (float2(1.0, 1.0) - abs(i.worldPos.xy)) * _ClipArgs0.xy;
                    float f = min(factor.x, factor.y);

                    // Second clip region
                    factor = (float2(1.0, 1.0) - abs(i.worldPos.zw)) * _ClipArgs1.xy;
                    f = min(f, min(factor.x, factor.y));

                    // Third clip region
                    factor = (float2(1.0, 1.0) - abs(i.worldPos2)) * _ClipArgs2.xy;
                    f = min(f, min(factor.x, factor.y));

					col.a *= clamp( f, 0.0, 1.0);

					return col;
				}
			ENDCG
		}
	}
}
