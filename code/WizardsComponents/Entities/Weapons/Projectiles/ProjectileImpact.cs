using Sandbox;

public sealed class ProjectileImpact : BaseComponent, BaseComponent.ICollisionListener
{
	[Property] public GameObject ImpactParticles { get; set; }

	public void OnCollisionStart( Collision o )
	{
		if(ImpactParticles is not null)
		{
			SceneUtility.Instantiate( ImpactParticles, o.Contact.Point );
		}

		GameObject.DestroyImmediate();
	}

	public void OnCollisionStop( CollisionStop other )
	{
		
	}

	public void OnCollisionUpdate( Collision other )
	{

	}
}
