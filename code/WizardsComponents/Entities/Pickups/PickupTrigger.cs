using Sandbox;

public class PickupTrigger : BaseComponent, BaseComponent.ITriggerListener
{
	[Property] public float RespawnTime { get; set; } = 5f;

	private TimeSince TimeSinceRespawned { get; set; }

	public bool Available { get; private set; } = true;

	public void Pickup()
	{
		if ( Available )
		{	
			Available = false;
			TimeSinceRespawned = 0;
		}
	}

	public override void Update()
	{
		base.Update();

		if ( TimeSinceRespawned > RespawnTime )
		{
			Available = true;
		}
	
		foreach(var child in GameObject.Children)
		{
			if ( child.GetComponent<ModelComponent>().SceneObject is SceneObject model )
			{
				model.RenderingEnabled = Available;
			}
		}
	}

	void ITriggerListener.OnTriggerEnter( Collider other ) 
	{
		if ( !Available )
			return;

		Pickup();
	}

	void ITriggerListener.OnTriggerExit( Collider other ) 
	{

	}

}
