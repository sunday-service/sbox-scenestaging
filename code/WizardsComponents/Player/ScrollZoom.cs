namespace Sandbox;

public partial class ScrollZoom : BaseComponent
{
	[Property] public float ZoomSpeed { get; set; } = 32f;
	[Property, Range(0, 800)] public int MinCameraDistance { get; set; } = 32;
	[Property, Range(0, 800)] public int MaxCameraDistance { get; set; } = 400;

	private float CameraDistance = 100;
	public override void FixedUpdate()
	{
		base.FixedUpdate();

		if(Input.MouseWheel != 0)
		{
			CameraDistance += Input.MouseWheel * ZoomSpeed;
			CameraDistance = MathX.Clamp(CameraDistance, MinCameraDistance, MaxCameraDistance);

			var playerController = GameObject.GetComponent<PlayerController>();
			playerController.CameraDistance = CameraDistance;
		}
	}
}
