Texture2D TexTexture; 
SamplerState TexSampler;
float4 TexColor;

struct VSOutputTx
{
    float4 PositionPS : SV_Position;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 PixelShaderFunction(VSOutputTx input) : COLOR0
{
    float4 textureColor = TexTexture.Sample(TexSampler, input.TexCoord) * input.Color;
    return float4(lerp(textureColor.rgb, TexColor.rgb, TexColor.a), textureColor.a);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
