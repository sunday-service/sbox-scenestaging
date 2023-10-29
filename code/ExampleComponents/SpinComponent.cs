using Sandbox;

public sealed class BobComponent : BaseComponent
{
	[Property] public Angles SpinAngles { get; set; }

	public override void Update()
	{
		Transform.LocalRotation *= (SpinAngles * RealTime.Delta).ToRotation();
	}
}
