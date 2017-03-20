#include "UnityCG.cginc"

void decodeColor(float4 value, out float4 color1, out float4 color2)
{
    int4 compose = value * 0xFF;
#if !SHADER_API_D3D9
    int4 high = (compose >> 4) & 0xF;
    int4 low = compose & 0xF;  
#else
    int4 high = compose / int(0x10);
    int4 low = compose - high * int(0x10);
#endif  
    color1 = float4(high) / 0xF;
    color2 = float4(low) / 0xF;
}

float2 decodeAlpha(float4 value)
{
    return float2(value.b + value.a * 0.0625, value.r + value.g * 0.0625);
}