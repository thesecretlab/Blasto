using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ignore 'this variable is never written to' warnings, 
// because Unity will be setting them externally.
#pragma warning disable 0649

// Spawn points are locations where players can appear.
public class SpawnPoint : MonoBehaviour {

    // --- Inspector fields ---

    // The transform at which players will spawn.
    [SerializeField]
    Transform _spawnPointTransform;

    // The position at which players will spawn from this spawn point.
    public Vector3 position {
        get {
            return _spawnPointTransform.position;
        }
    }

    // The rotation at which players will spawn from this spawn point.
    public Quaternion rotation
	{
		get
		{
            return _spawnPointTransform.rotation;
		}
	}
	
}
