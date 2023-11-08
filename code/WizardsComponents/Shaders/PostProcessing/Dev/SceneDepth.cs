using Sandbox;
using System;

[Title( "Scene Depth" )]
[Category( "Post Processing" )]
[Icon( "apps" )]
public sealed class SceneDepth : BaseComponent, BaseComponent.ExecuteInEditor
{
	[Property, Range( 0, 1 )] public float SceneDepthScale { get; set; } = 1.0f;
	
	IDisposable renderHook;

	public override void OnEnabled()
	{
		renderHook?.Dispose();

		var cc = GetComponent<CameraComponent>( false, false );
		renderHook = cc.AddHookAfterTransparent( "Scene Depth", 501, RenderEffect );
	}

	public override void OnDisabled()
	{
		renderHook?.Dispose();
		renderHook = null;
	}

	RenderAttributes attributes = new RenderAttributes();

	public void RenderEffect( SceneCamera camera )
	{
		if ( !camera.EnablePostProcessing )
			return;

		attributes.Set( "SceneDepthScale", SceneDepthScale );

		Graphics.GrabFrameTexture( "ColorBuffer", attributes );
		Graphics.GrabDepthTexture( "DepthBuffer", attributes );

		Graphics.Blit( Material.FromShader( "shaders/postprocess/dev/scenedepth.shader" ), attributes );
	}

}
