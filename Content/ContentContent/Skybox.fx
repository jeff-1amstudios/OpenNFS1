
//Input variables
float4x4 worldViewProjection;

texture baseTexture;

sampler baseSampler =
sampler_state
{
	Texture = < baseTexture >;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
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
	Out.ScreenPos = mul(In.ObjectPos, worldViewProjection);
	Out.TextureCoords = In.TextureCoords;

	return Out;
}

PS_OUTPUT SimplePS(VS_OUTPUT In)
{
	PS_OUTPUT Out;

	Out.Color = tex2D(baseSampler, In.TextureCoords);

	return Out;
}

//--------------------------------------------------------------//
// Technique Section for Simple screen transform
//--------------------------------------------------------------//
technique Simple
{
	pass Single_Pass
	{
		//LIGHTING = FALSE;
		ZENABLE = FALSE;
		ZWRITEENABLE = FALSE;
		//ALPHATESTENABLE = FALSE;
		ALPHABLENDENABLE = FALSE;

		CULLMODE = CCW;

		VertexShader = compile vs_2_0 SimpleVS();
		PixelShader = compile ps_2_0 SimplePS();

		//LIGHTING = TRUE;
		//ZENABLE = TRUE;
		//ZWRITEENABLE = TRUE;
		//ALPHATESTENABLE = FALSE;
		//ALPHABLENDENABLE = FALSE;
	}

}