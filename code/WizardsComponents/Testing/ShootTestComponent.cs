using Sandbox;
using Sandbox.Diagnostics;


public sealed class ShootTestComponent : BaseComponent
{
	[Property] GameObject Projectile { get; set; }

	[Property] float ProjectileVelocityMultiplier { get; set; } = 256.0f;
	[Property] GameObject Muzzle { get; set; }
	[Property] public float ShootInterval { get; set; }

	private TimeSince TimeSinceShoot { get; set; }

	public override void Update()
	{
		if ( TimeSinceShoot > ShootInterval )
		{
			Assert.NotNull( Projectile );

			var projectile = SceneUtility.Instantiate( Projectile, Muzzle.Transform.Position, Muzzle.Transform.Rotation );

			var physics = projectile.GetComponent<PhysicsComponent>( true, true );

			if ( physics is not null )
			{
				physics.Velocity = Muzzle.Transform.Rotation.Forward * ProjectileVelocityMultiplier;
			}

			TimeSinceShoot = 0;
		}
	}
}
