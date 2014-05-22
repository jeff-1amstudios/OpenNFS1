
//Input variables
float4x4 WorldViewProj;
float3 BrakeColor;

texture baseTexture;

sampler baseSampler =
sampler_state
{
	Texture = < baseTexture >;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
};

struct VS_INPUT
{
	float4 ObjectPos: POSITION;
	float2 TextureCoords: TEXCOORD0;
};

struct VS_OUTPUT
{
	float4 ScreenPos:   POSITION;
	float2 TextureCoords: TEXCOORD0;
};

struct PS_OUTPUT
{
	float4 Color:   COLOR;
};


VS_OUTPUT SimpleVS(VS_INPUT In)
{
	VS_OUTPUT Out;

	//Move to screen space
	Out.ScreenPos = mul(In.ObjectPos, WorldViewProj);
	Out.TextureCoords = In.TextureCoords;

	return Out;
}

PS_OUTPUT SimplePS(VS_OUTPUT In)
{
	PS_OUTPUT Out;

	float4 color = tex2D(baseSampler, In.TextureCoords);

	clip((color.a < 0.001) ? -1 : 1);

	float4 rgbCol = color * 255;
	if (color.r > 0.3 && color.r < 0.7 
		&& color.g > 0.7 && color.g < 1.0 
		&& color.b > 0.0 && color.b < 0.4)
	/*if (round(rgbCol.r) > 130 && round(rgbCol.r) < 150
		&& round(rgbCol.g) > 238 && round(rgbCol.g) < 258
		&& round(rgbCol.b) > 14 && round(rgbCol.b) < 33)*/
	{
		Out.Color.rgb = BrakeColor;
	}
	else {
		Out.Color = color;
	}
	
	

	return Out;
}

//--------------------------------------------------------------//
// Technique Section for Simple screen transform
//--------------------------------------------------------------//
technique Car2
{
	pass Single_Pass
	{
		VertexShader = compile vs_2_0 SimpleVS();
		PixelShader = compile ps_2_0 SimplePS();
	}

}