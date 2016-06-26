Texture permTexture;
float Overcast;
float2 xCoord;

sampler TextureSampler = sampler_state { texture = <permTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = mirror; AddressV = mirror; };

//------- Technique: PerlinNoise --------
struct PNVertexToPixel
{
	float4 Position         : POSITION;
	float2 TextureCoords    : TEXCOORD0;
};

struct PNPixelToFrame
{
	float4 Color : COLOR0;
};

PNVertexToPixel PerlinVS(float4 inPos : SV_POSITION, float2 inTexCoords : TEXCOORD)
{
	PNVertexToPixel Output = (PNVertexToPixel)0;

	Output.Position = inPos;
	Output.TextureCoords = inTexCoords;

	return Output;
}

PNPixelToFrame PerlinPS(PNVertexToPixel PSIn)
{
	PNPixelToFrame Output = (PNPixelToFrame)0;

	float2 move = float2(1, 1);

	float4 perlin = tex2D(TextureSampler, float2(PSIn.TextureCoords.x + xCoord.x*move.x, PSIn.TextureCoords.y + xCoord.y*move.y)) / 2;
	perlin += tex2D(TextureSampler, float2(PSIn.TextureCoords.x * 2 + xCoord.x*move.x, PSIn.TextureCoords.y * 2 + xCoord.y*move.y)) / 4;
	perlin += tex2D(TextureSampler, float2(PSIn.TextureCoords.x * 4 + xCoord.x*move.x, PSIn.TextureCoords.y * 4 + xCoord.y*move.y)) / 8;
	perlin += tex2D(TextureSampler, float2(PSIn.TextureCoords.x * 8 + xCoord.x*move.x, PSIn.TextureCoords.y * 8 + xCoord.y*move.y)) / 16;
	perlin += tex2D(TextureSampler, float2(PSIn.TextureCoords.x * 16 + xCoord.x*move.x, PSIn.TextureCoords.y* 16 + xCoord.y*move.y)) / 32;
	perlin += tex2D(TextureSampler, float2(PSIn.TextureCoords.x * 32 + xCoord.x*move.x, PSIn.TextureCoords.y * 32 + xCoord.y*move.y)) / 32;

	[loop]
	for (uint i = 0; i < 4; i++)
	{
		perlin.r = sqrt(perlin.r);
	}

	Output.Color.rgb = perlin.r/2;
	Output.Color.a = 1;

	return Output;
}

technique PerlinNoise
{
	pass Pass0
	{
		VertexShader = compile vs_4_0_level_9_1 PerlinVS();
		PixelShader = compile ps_4_0_level_9_1 PerlinPS();
	}
}