//=========================================================================================================================

HEADER
{
    Description = "Simple Liquid Shader";
}

//=========================================================================================================================

FEATURES
{
    #include "common/features.hlsl"
}

//=========================================================================================================================

MODES
{
    VrForward();                                               // Indicates this shader will be used for main rendering
    ToolsVis( S_MODE_TOOLS_VIS );                              // Ability to see in the editor
    ToolsWireframe("vr_tools_wireframe.shader");               // Allows for mat_wireframe to work
    ToolsShadingComplexity("tools_shading_complexity.shader"); // Shows how expensive drawing is in debug view
}

//=========================================================================================================================

COMMON
{
    #include "common/shared.hlsl"

    #ifndef S_ALPHA_TEST
	#define S_ALPHA_TEST 1
	#endif

	float g_flWobbleX <Attribute("WobbleX"); Range(-8, 4); Default(0);>;
	float g_flWobbleY <Attribute("WobbleY"); Range(-8, 4); Default(0);>;
}

//=========================================================================================================================

struct VertexInput
{
    #include "common/vertexinput.hlsl"
};

//=========================================================================================================================

struct PixelInput
{
    #include "common/pixelinput.hlsl"

	float3 vFillPosition : POSITION < Semantic( PosXyz ); >;
};

//=========================================================================================================================

VS
{
    #include "common/vertex.hlsl"

    float3 RotateAroundX(float3 position, float degrees) 
	{
		degrees = radians(degrees);

		float3x3 mat = 
		{
			1, 0, 0,
			0, cos(degrees), -sin(degrees),
			0, sin(degrees), cos(degrees)	
		};

		return mul(mat, position);
	}

	float3 RotateAroundY(float3 position, float degrees)
	{
		degrees = radians(degrees);

		float3x3 mat = 
		{
			cos(degrees), 0, sin(degrees),
			0, 1, 0,
			-sin(degrees), 0, cos(degrees)	
		};

		return mul(mat, position);
	}
	
	PixelInput MainVs( VertexInput v )
	{
		PixelInput i = ProcessVertex( v );

		float3 vPositionWs = normalize(mul(CalculateInstancingObjectToWorldMatrix(v), v.vPositionOs.xyz));

		float3 worldPosX = RotateAroundX(vPositionWs, 90);
		float3 worldPosY = RotateAroundY(vPositionWs, 90);

		i.vFillPosition = vPositionWs + (worldPosX * g_flWobbleX) + (worldPosY * g_flWobbleY);

		return FinalizeVertex( i );
	}
}

//=========================================================================================================================

PS
{
    #include "common/pixel.hlsl"

    // Liquid Fill Level Attributes
    float g_flFillAmount < Attribute("FillAmount"); Range(0, 1.0); Default(0.5f); >;
    float g_flFoamThickness < Attribute("FoamThickness"); Range(0, 1.0); Default(0.05f); >;

    // Liquid Animation Attributes
    float g_flFillWobbleFrequency <Attribute("FillWobbleFrequency"); UiType( Slider); Range(0, 64.0); Default(8);>;
	float g_flFillWobbleAmplitude <Attribute("FillWobbleAmplitude"); UiType( Slider); Range(0, 1.0); Default(0.1);>;
	
    // Liquid Color Attributes
	float3 g_vFoamColor <Attribute("FillColorFoam"); Default3(0, 0.6, 0.7); >;
	float3 g_vFillColorUpper < Attribute("FillColorUpper");  Default3(0, 0.5, 0.5); >;
	float3 g_vFillColorLower < Attribute("FillColorLower");  Default3(0, 0, 1); >;
	
	float g_flRimStrengthPower <Attribute("RimLightStrengthPower"); Default(2); >;

    float4 MainPs(PixelInput i, bool isFrontFace : SV_IsFrontFace)  : SV_Target0
    {
        float wobbleIntensity = abs(g_flWobbleX) + abs(g_flWobbleY);
		float wobble = sin((i.vFillPosition.x * g_flFillWobbleFrequency) + (i.vFillPosition.y * g_flFillWobbleFrequency) + (g_flTime)) * (g_flFillWobbleAmplitude * wobbleIntensity);

		float fillPosition = i.vFillPosition.z + wobble;
        float fillEdge = (2 * g_flFillAmount) - 1.0f;
        float fillAmount = step(fillPosition, fillEdge);
        
		float3 fillColor = lerp(g_vFillColorUpper, g_vFillColorLower, fillPosition);

        float3 viewDirection = CalculatePositionToCameraDirWs(i.vPositionWithOffsetWs + g_vHighPrecisionLightingOffsetWs.xyz );
		float3 frensel = pow(1.0 - dot(normalize(i.vNormalWs), normalize(viewDirection)), g_flRimStrengthPower);
        
        float3 frontFaceColor = fillColor + frensel;
        float3 backFaceColor = g_vFoamColor;

        float4 result = float4(lerp(backFaceColor, frontFaceColor, isFrontFace), fillAmount);    
        
        return result;
    }
}