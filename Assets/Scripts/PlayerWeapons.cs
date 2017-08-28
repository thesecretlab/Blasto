using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ignore 'this variable is never written to' warnings, 
// because Unity will be setting them externally.
#pragma warning disable 0649

// PlayerWeapons provides the mechanism for letting players shoot.
// While the player's 'fire' button is being held down, a stream of
// shot objects is created, with a delay between them. Each shot emits
// from one of several fire points, which are cycled through.
public class PlayerWeapons : MonoBehaviour {

    // --- Inspector fields ---

    // The prefab for each shot.
    [SerializeField] 
    Shot shotPrefab;

    // The amount of time between each shot.
    [SerializeField] 
    float timeBetweenShots = 0.01f;

    // --- Private variables ---

	// The list of transforms, attached to this object, from which
	// shots can be emitted
    Transform[] firePoints;

	// How long we're waiting until we can fire. If <= 0, we can fire.
	float timeUntilNextShot = 0.0f;

    // The index into the firePoints array that represents which fire point the
    // next shot will come from.
	int currentFirePoint;

	// The input object, which we use to see if the player wants to fire. 
	PlayerInput input;

	// --- Public variables ---

    // The data representing the player that owns us, and consequently our
    // shots (and the explosions they create).
	[HideInInspector]
    public PlayerConfiguration configuration;

    // Called when the object first appears.
    void Awake()
    {
        // Store the reference to the input, so that we can use it later
        input = GetComponent<PlayerInput>();

        // Add all children named 'FirePoint' to the firePoints array, by
        // first creating a temporary list, adding items to the list, and
        // then saving the list as an array.
        var points = new List<Transform>();

        foreach (Transform child in transform) {
            if (child.name == "FirePoint") {
                points.Add(child);
            }
        }

        firePoints = points.ToArray();

    }

	// Update is called once per frame
	void Update () {

        // If we're waiting until the next opportunity to shoot,
        // count down.
        if (timeUntilNextShot > 0)
            timeUntilNextShot -= Time.deltaTime;

        // Is the fire button down?
        if (input.fire.isActive) {

            // Don't fire if we're still waiting
			if (timeUntilNextShot > 0)
			{
				return;
			}

            // Get the currently selected fire point.
            var firePoint = firePoints[currentFirePoint];

            // Fire a shot if we have the prefab
            if (shotPrefab != null) {
                
				// Create the new shot
				var newShot = Instantiate(shotPrefab, firePoint.position, firePoint.rotation);

				// Pass along the configuration if we have one
				if (configuration != null)
				{
					newShot.owner = configuration;
					newShot.name += string.Format(" (from Player {0})",
												  configuration.playerNumber.ToString());
				}

				// Make this shot not collide with the ship's collider
				var myCollider = GetComponentInChildren<Collider>();
				var shotCollider = newShot.GetComponentInChildren<Collider>();

				Physics.IgnoreCollision(myCollider, shotCollider);

            } else {
                Debug.LogWarning("PlayerWeapons can't create shot: prefab wasn't connected.");
            }

			// Move to the next fire point
			currentFirePoint += 1;

			// Wrap back to zero if we've reached the end of the list
			currentFirePoint %= firePoints.Length;

			// Wait until 'timeBetweenShots' before we can fire again
			timeUntilNextShot = timeBetweenShots;


        }


	}
}
