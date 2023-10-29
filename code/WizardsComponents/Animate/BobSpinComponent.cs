using Sandbox;

public sealed class BobSpinComponent : BaseComponent
{
	[Property] public float Amplitude { get; set; }
	[Property] public float RotationSpeed { get; set; }

	public override void Update()
	{
		Transform.LocalPosition += new Vector3( 0, 0, (float)Math.Sin( Time.Now ) * Amplitude );

		Transform.LocalRotation = Transform.LocalRotation.Angles().WithYaw(Time.Now * RotationSpeed).ToRotation();
	}
}
