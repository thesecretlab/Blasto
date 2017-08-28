using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ignore 'this variable is never written to' warnings, 
// because Unity will be setting them externally.
#pragma warning disable 0649

// Shots are objects that are created by PlayerWeapons. They fly forward until
// they hit something, at which point they create an explosion. Shots are owned
// by players, which is how we attribute damage (so that players can score points.)
public class Shot : MonoBehaviour {

    // --- Inspector fields ---

    // The speed at which we travel.
    [SerializeField]
    float speed = 20.0f;

    // The explosion to create when we collide with something.
	[SerializeField]
	Explosion explosionPrefab;

    // -- Public variables ---

    // The player that fired us.
    public PlayerConfiguration owner;

    // Called when the object first appears.
    void Start()
    {
        // Remove ourself in a few seconds, if we don't collide with anything
        Destroy(gameObject, 5.0f);
    }

    // Called every frame.
    void Update()
    {
        // Every frame, ensure that we're travelling forward.
        // (This means that we aren't pushed around by other forces.)
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    // Called when we collide with something.
    void OnCollisionEnter(Collision collision)
    {

        // Create an explosion if we have one
        if (explosionPrefab != null)
        {
            // Instantiate our explosion
            var explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);

            // If we have an owner, associate the explosion with that owner.
            if (owner != null)
            {
                explosion.owner = owner;
                explosion.name += string.Format(" (from Player {0}", owner.playerNumber);
            }
        }
        else
        {
            Debug.LogWarning("Shot can't create explosion: no prefab connected");
        }

        // Remove this shot from the game.
        Destroy(gameObject);
    }

}
