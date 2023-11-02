using Sandbox;

public sealed class DelayedExplosion : BaseComponent
{
	[Property] public GameObject ExplosionParticles { get; set; }
	[Property] float Seconds { get; set; }

	TimeUntil timeUntilDie;

	public override void OnEnabled()
	{
		timeUntilDie = Seconds;
	}

	public override void Update()
	{
		if ( timeUntilDie <= 0.0f )
		{
			if ( ExplosionParticles is not null )
			{
				SceneUtility.Instantiate( ExplosionParticles, GameObject.Transform.Position );
			}

			GameObject.Destroy();
		}
	}
}
