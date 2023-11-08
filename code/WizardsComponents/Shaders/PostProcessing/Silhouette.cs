using Sandbox;

[Title( "Silhouette" )]
[Category( "Post Processing" )]
[Icon( "apps" )]
public sealed class Silhouette : BaseComponent, BaseComponent.ExecuteInEditor
{
	[Property, Range( 0, 1 )] public float SceneDepthScale { get; set; } = 1.0f;
	[Property] public Color SceneDepthNearColor { get; set; } = new Color( 1, 1, 1, 1 );
	[Property] public Color SceneDepthFarColor { get; set; } = new Color( 0, 0, 0, 1 );

	IDisposable renderHook;

	public override void OnEnabled()
	{
		renderHook?.Dispose();

		var cc = GetComponent<CameraComponent>( false, false );
		renderHook = cc.AddHookAfterTransparent( "Silhouette", 502, RenderEffect );
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
		attributes.Set( "SceneDepthNearColor", SceneDepthNearColor );
		attributes.Set( "SceneDepthFarColor", SceneDepthFarColor );

		Graphics.GrabFrameTexture( "ColorBuffer", attributes );
		Graphics.GrabDepthTexture( "DepthBuffer", attributes );

		Graphics.Blit( Material.FromShader( "shaders/postprocess/silhouette.shader" ), attributes );
	}

}
