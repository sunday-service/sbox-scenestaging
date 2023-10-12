﻿using Sandbox.Diagnostics;
using Sandbox.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Nodes;

public static class SceneUtility
{
	/// <summary>
	/// Find all "Id" guids, and replace them with new Guids.
	/// This is used to make GameObject serializations unique, so when
	/// you duplicate stuff, it copies over uniquely and keeps associations.
	/// </summary>
	public static void MakeGameObjectsUnique( JsonObject json )
	{
		Dictionary<Guid, Guid> translate = new();

		//
		// Find all guids with "Id" as their name. Add them to translate 
		// with a new target value.
		//
		Sandbox.Json.WalkJsonTree( json, ( k, v ) =>
		{
			if ( k != "Id" ) return v;

			if ( v.TryGetValue<Guid>( out var guid ) )
			{
				translate[guid] = Guid.NewGuid();
			}

			return v;
		} );

		//
		// Find every guid and translate them, but only if they're in our
		// guid dictionary.
		//
		Sandbox.Json.WalkJsonTree( json, ( k, v ) =>
		{
			if ( !v.TryGetValue<Guid>( out var guid ) ) return v;
			if ( !translate.TryGetValue( guid, out var updatedGuid ) ) return v;

			return updatedGuid;
		} );
	}


	/// <summary>
	/// Create a unique copy of the passed in GameObject
	/// </summary>
	public static GameObject Instantiate( GameObject template, Transform transform )
	{
		using var spawnScope = DeferInitializationScope( "Instantiate" );

		var json = template.Serialize();

		MakeGameObjectsUnique( json );

		var go = GameObject.Create();
		go.Deserialize( json );
		go.Transform.Local = transform;

		if ( template is PrefabScene prefabScene )
		{
			go.SetPrefabSource( prefabScene.Source.ResourcePath );
		}

		GameManager.ActiveScene.Register( go );

		return go;
	}

	/// <summary>
	/// Create a unique copy of the passed in GameObject
	/// </summary>
	public static GameObject Instantiate( GameObject template ) => Instantiate( template, Transform.Zero );

	/// <summary>
	/// Create a unique copy of the passed in GameObject
	/// </summary>
	public static GameObject Instantiate( GameObject template, Vector3 position, Quaternion rotation ) 
		=> Instantiate( template, new Transform( position, rotation, template.Transform.LocalScale.x ) );


	static HashSet<GameObject> spawnList;

	/// <summary>
	/// Create a scope in which all created gameobjects only become enabled at the end.
	/// This is useful if you have situation where you're spawning a prefab with lots of 
	/// connections, and want everything deserialized and existing before activating.
	/// </summary>
	public static IDisposable DeferInitializationScope( string scopeName )
	{
		var lastSpawnList = spawnList;
		spawnList = new();

		return DisposeAction.Create( () =>
		{
			var sw = spawnList;
			spawnList = lastSpawnList;

			foreach ( var o in sw )
			{
				o.UpdateEnabledStatus();
			}

			sw.Clear();
			sw = null;
		} );
	}

	internal static void ActivateGameObject( GameObject o )
	{
		Assert.NotNull( o.Scene );

		if ( spawnList is not null )
		{
			spawnList.Add( o );
			return;
		}

		o.UpdateEnabledStatus();
	}

	internal static void ActivateComponent( BaseComponent o )
	{
		if ( o.GameObject is null || o.Scene is null ) return;

		if ( spawnList is null )
		{
			o.UpdateEnabledStatus();
			return;
		}

		ActivateGameObject( o.GameObject );
	}
}
