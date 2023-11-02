using Sandbox;

public sealed class ClusterProjectile : BaseComponent, BaseComponent.ICollisionListener
{
	[Property] public GameObject Projectile { get; set; }
	[Property] public int NumberOfProjectiles { get; set; } = 3;
	[Property] public float ProjectileSpread { get; set; } = 128f;
	
	public void OnCollisionStart( Collision o )
	{
		if ( Projectile is not null )
		{
			for ( int i = 0; i < NumberOfProjectiles; i++ )
			{
				var random = new Vector3( Game.Random.Float( -1f, 1f ), Game.Random.Float( -1f, 1f ), Game.Random.Float( 0.5f, 1f ) );
				var direction = (Vector3.Up * Game.Random.Float( 0.1f, 1f )) + (random * Game.Random.Float( 1f, 1.5f ));

				var projectile = SceneUtility.Instantiate( Projectile, o.Contact.Point + (direction));
				projectile.GetComponent<PhysicsComponent>().Velocity = direction * ProjectileSpread;
				
			}
		}

		GameObject.Destroy();
	}

	public void OnCollisionStop( CollisionStop other )
	{
		
	}

	public void OnCollisionUpdate( Collision other )
	{

	}
}
