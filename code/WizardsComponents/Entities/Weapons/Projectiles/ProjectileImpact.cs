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
	}

	public void OnCollisionStop( CollisionStop other )
	{
		GameObject.Destroy();
	}

	public void OnCollisionUpdate( Collision other )
	{

	}
}
