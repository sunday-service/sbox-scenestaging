//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	Description = "Scene Color Shader Example for S&box";
}

MODES
{
    VrForward();                                               // Indicates this shader will be used for main rendering
    ToolsVis( S_MODE_TOOLS_VIS );                              // Ability to see in the editor
    ToolsWireframe("vr_tools_wireframe.shader");               // Allows for mat_wireframe to work
    ToolsShadingComplexity("tools_shading_complexity.shader"); // Shows how expensive drawing is in debug view
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
FEATURES
{
    #include "common/features.hlsl"
}

//=========================================================================================================================
COMMON
{
	#include "common/shared.hlsl"
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
};

//=========================================================================================================================

VS
{
	#include "common/vertex.hlsl"

	PixelInput MainVs( VertexInput v )
	{
		PixelInput i = ProcessVertex( v );

		i.vPositionPs = float4(v.vPositionOs.xy, 0.0f, 1.0f);

		return FinalizeVertex( i );
	}
}

//=========================================================================================================================
PS
{
	#include "common/pixel.hlsl"

	CreateTexture2D(g_tColorBuffer)<Attribute("ColorBuffer"); SrgbRead(false); Filter(MIN_MAG_MIP_POINT); AddressU(CLAMP); AddressV(CLAMP); >;
	
	float4 GreyscaleFilter( float4 SceneColor )
    {

        float lum = SceneColor.r * 0.3 + SceneColor.g * 0.59 + SceneColor.b * 0.11;
        return float4(lum, lum, lum, SceneColor.a);
    }

	float4 MainPs( PixelInput i ) : SV_Target
	{
		float2 screenUv = CalculateViewportUv( i.vPositionSs.xy ); 

		float4 screenColor = Tex2D(g_tColorBuffer, screenUv);
		
		return GreyscaleFilter(screenColor);
	}
}
