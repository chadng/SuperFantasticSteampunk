Texture2D TexTexture; 
SamplerState TexSampler;

struct VSOutputTx
{
    float4 PositionPS : SV_Position;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 PixelShaderFunction(VSOutputTx input) : COLOR0
{
    return TexTexture.Sample(TexSampler, input.TexCoord) * input.Color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
