using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerConfiguration stores information that's unique to each player,
// and doesn't change over time. Components on the Player prefab make use
// of this to control their behaviour.

// It's stored in a ScriptableObject so that we can store it in an asset, and 
// connect it to instances.

[CreateAssetMenu]
public class PlayerConfiguration : ScriptableObject
{

    // An enumeration that represents which player we are.
    public enum PlayerNumber {
        NotSet,
        One,
        Two
    }

    // Which player are we?
    public PlayerNumber playerNumber;

    // Keyboard button mapping.
	public KeyCode turnLeftKey;
	public KeyCode turnRightKey;
	public KeyCode increaseThrustKey;
	public KeyCode decreaseThrustKey;
	public KeyCode fireKey;
	public KeyCode boostKey;

    // The colour to appear as, in both the game and in the UI.
	public Color color;

	// The 'pilot', who's thrown out of the spaceship when it's destroyed
	public GameObject pilotPrefab;
}
