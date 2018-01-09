using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ignore 'this variable is never written to' warnings, 
// because Unity will be setting them externally.
#pragma warning disable 0649

// Spawn points are locations where players can appear.
public class SpawnPoint : MonoBehaviour {

    // --- Inspector fields ---

    [SerializeField]
    float _exclusionRadius = 1.0f;

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

	// Can a player spawn at this location?
	public bool canSpawnHere {
		get {
			// False if there's a player within _exclusionRadius of the spawn point
			foreach (var player in FindObjectsOfType<PlayerController>()) {

				// We can only get the location of players that are spawned
				if (player.isSpawned == false) {
					continue;
				}

				// Get their position, and use that to figure out the distance to
				// the spawn point
				var pos = player.position;
				var distance = (pos - _spawnPointTransform.position).magnitude;

				if (distance <= _exclusionRadius) {
					// A player avatar is nearby, so indicate that this spawn is not available
					return false;
				}

			}

			// No players are nearby, so we indicate that this spawn is available
			return true;
		}
	}

	// Shows additional information in the scene view when selected.
	public void OnDrawGizmosSelected() {

		// When this object is selected in the Scene View, draw a 
		// semi transparent red ball that indicates the exclusion area

		var color = Color.red;
		color.a = 0.5f;
		Gizmos.color = color;
		Gizmos.DrawSphere (_spawnPointTransform.position, _exclusionRadius);
	}

	// Returns a random spawn point that isn't too close to another player.
	public static SpawnPoint GetRandomAvailableSpawnPoint() {
		// Get the list of all spawn points in the game.
		var allSpawnPoints = FindObjectsOfType<SpawnPoint>();

		// Shuffle this array
		int n = allSpawnPoints.Length;  
		while (n > 1) {  
			n--;  
			int k = Random.Range (0, n+1);
			var value = allSpawnPoints[k];  
			allSpawnPoints[k] = allSpawnPoints[n];  
			allSpawnPoints[n] = value;  
		} 

		// Pick the first available spawn from this list
		foreach (var spawn in allSpawnPoints) {
			if (spawn.canSpawnHere) {
				return spawn;
			}
		}

		// None was available - throw an exception, because this shouldn't be possible
		// since there's always 1 more spawn point than there are alive players.
		throw new System.InvalidOperationException("No spawn points available!");
	}
}