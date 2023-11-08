//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	Description = "Silhouette Shader for S&box";
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

	float g_flDepthScale<Attribute("SceneDepthScale"); Range(0, 1); Default(1); >;
	float4 g_vDepthNearColor<Attribute("SceneDepthNearColor"); Default4(0,0,0,1);>;
	float4 g_vDepthFarColor<Attribute("SceneDepthFarColor"); Default4(1,1,1,1);>;

	CreateTexture2D(g_tDepthBuffer)<Attribute("DepthBuffer"); SrgbRead(false); Filter(MIN_MAG_MIP_POINT); AddressU(CLAMP); AddressV(CLAMP); >;

	float FetchSceneDepth(float2 vTexCoord)
	{
		float flProjectedDepth = Tex2D(g_tDepthBuffer, vTexCoord.xy).x;

		flProjectedDepth = RemapValClamped(flProjectedDepth, g_flViewportMinZ, g_flViewportMaxZ, 0.0, 1.0);

		float flZScale = g_vInvProjRow3.z;
		float flZTranslation = g_vInvProjRow3.w;

		float flDepthRelativeToRayLength = 1.0 / ((flProjectedDepth * flZScale + flZTranslation));

		return flDepthRelativeToRayLength;
	}

	float FetchCenterDepth(PixelInput i)
	{
		float flObjectDepth = i.vPositionSs.z;

		float2 vScreenUv = CalculateViewportUvFromInvSize(i.vPositionSs.xy, 1.0f / g_vRenderTargetSize);

		float flCenterDepth = FetchSceneDepth(vScreenUv);

		flCenterDepth = RemapValClamped(flCenterDepth, g_flViewportMinZ, 1000, 0.0, 1.0);

		return flCenterDepth;
	}
	
	float4 MainPs( PixelInput i ) : SV_Target0
	{
		float depth = FetchCenterDepth(i);
		float difference = saturate(abs(depth) / g_flDepthScale);

		float4 result = lerp(g_vDepthNearColor, g_vDepthFarColor, difference);
		
		return result;
	}
}
