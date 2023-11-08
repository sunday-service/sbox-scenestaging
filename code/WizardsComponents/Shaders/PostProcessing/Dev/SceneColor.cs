using Sandbox;

[Title( "Scene Color" )]
[Category( "Post Processing" )]
[Icon( "apps" )]
public sealed class SceneColor : BaseComponent, BaseComponent.ExecuteInEditor
{
	IDisposable renderHook;

	public override void OnEnabled()
	{
		renderHook?.Dispose();

		var cc = GetComponent<CameraComponent>( false, false );
		renderHook = cc.AddHookAfterTransparent( "Scene Color", 501, RenderEffect );
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

		
		Graphics.GrabFrameTexture( "ColorBuffer", attributes );
		Graphics.GrabDepthTexture( "DepthBuffer", attributes );
		Graphics.Blit( Material.FromShader( "shaders/postprocess/dev/scenecolor.shader" ), attributes );
	}

}
