float xPixelWidth;
float xPixelHeight;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;};

// Blurs using a 3x3 filter kernel
float4 BlurFunction3x3(float2 input : TEXCOORD) : COLOR0
{
  // TOP ROW
  float4 s11 = tex2D(TextureSampler, input + float2(-xPixelWidth, -xPixelHeight));    // LEFT
  float4 s12 = tex2D(TextureSampler, input + float2(0, -xPixelHeight));              // MIDDLE
  float4 s13 = tex2D(TextureSampler, input + float2(xPixelWidth, -xPixelHeight)); // RIGHT
 
  // MIDDLE ROW
  float4 s21 = tex2D(TextureSampler, input + float2(-xPixelWidth, 0));             // LEFT
  float4 col = tex2D(TextureSampler, input);                                          // DEAD CENTER
  float4 s23 = tex2D(TextureSampler, input + float2(xPixelWidth, 0));                 // RIGHT
 
  // LAST ROW
  float4 s31 = tex2D(TextureSampler, input + float2(-xPixelWidth, xPixelHeight)); // LEFT
  float4 s32 = tex2D(TextureSampler, input + float2(0, xPixelHeight));                   // MIDDLE
  float4 s33 = tex2D(TextureSampler, input + float2(xPixelWidth, xPixelHeight));  // RIGHT
 
  // Average the color with surrounding samples
  col =  (col + s11 + s12 + s13 + s21 + s23 + s31 + s32 + s33) / 9;
  return col;
}

sampler s0;

float4 Test(float2 input : TEXCOORD) : COLOR0
{
	return tex2D(s0, input);
}

//-----------------------------------------------------------------------------
// Techniques.
//-----------------------------------------------------------------------------

technique GaussianBlur
{
    pass Pass0
    {
        PixelShader = compile ps_4_0_level_9_1 Test();
    }
}
