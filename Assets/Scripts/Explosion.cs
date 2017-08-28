using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ignore 'this variable is never written to' warnings, 
// because Unity will be setting them externally.
#pragma warning disable 0649

// An Explosion shows a visual explosion, deals damage, and pushes objects away.
// They're created when shots hit anything, and when a player dies.
public class Explosion : MonoBehaviour {

    // --- Inspector fields ---

    [Header("Gameplay")]

    // If true, we'll apply a push force to nearby rigidbodies
    [SerializeField]
    bool appliesExplosionForce = true;

	// The radius of our explosion sphere
	[SerializeField]
	float explosionRadius = 2.5f;

	// The amount of explosive force we're applying, at the centre of the sphere
	// (it will tail off as you get further away)
	[SerializeField]
	float explosionForce = 50;

	// The amount of damage to apply to objects within the explosion radius
	[SerializeField]
	int damageAmount = 1;

    [Header("Appearance")]

	// The degree to which the position will be randomised.
	[SerializeField]
	float positionJitter = 1.0f;

	// The sprites we're animating through.
	[SerializeField]
	Sprite[] sprites;

	// The rate at which we cycle through the sprites
	[SerializeField]
	float framesPerSecond = 30;

    // --- Public variables ---

    // The information representing the player who caused this explosion.
    // Not shown, because it's set up by whoever caused it.
    [HideInInspector]
    public PlayerConfiguration owner;

    // Called when the explosion first appears.
    public IEnumerator Start() {
        
        var offset = Random.onUnitSphere * positionJitter;
        transform.position += offset;

        // Find all nearby rigidbodies
        var nearbyColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // For each collider...
        foreach (var nearbyCollider in nearbyColliders)
        {

            // Apply a force to nearby rigidbodies, if needed
            if (appliesExplosionForce)
            {
                var body = nearbyCollider.GetComponentInParent<Rigidbody>();

                // Skip if the collider has no rigidbody
                if (body == null)
                    continue;

                // Apply the force
                body.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            }

            // Does the object have a PlayerHealth attached?
            var playerHealth = nearbyCollider.GetComponentInParent<PlayerHealth>();

			// If it does, make it take damage
			if (playerHealth != null) {
                // First figure out whom we should attribute it to - if
                // this Explosion knows its owner, use its player number; otherwise,
                // say 'Not Set'
				var number = owner != null ? owner.playerNumber : PlayerConfiguration.PlayerNumber.NotSet;

                //  Now take the amount of damage required
				playerHealth.TakeDamage(damageAmount, number);
            }

        }

        // Now that we've applied the physical force and damage, we'll animate the sprites

        // Calculate the amount of time between each frame
        var timeBetweenFrames = 1.0f / framesPerSecond;

        // Get the sprite renderer we're modifying
        var spriteRenderer = GetComponent<SpriteRenderer>();

        // Loop through each sprite and update the renderer, pausing each time
        foreach (var sprite in sprites) {
            spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(timeBetweenFrames);
        }

        // The animation is done; destroy ourselves
        Destroy(gameObject);
    }

	
    // Called by the Unity editor when the object is selected; in
    // this method, you can use the Gizmos class to draw stuff in the scene view
    void OnDrawGizmosSelected()
    {
        // Visualise our explosion radius by using a sphere

        // We'll draw using a 50% transparent yellow colour
        Gizmos.color = new Color(1, 1, 0, 0.5f);

        // Draw the sphere at our current location and the explosion radius
        Gizmos.DrawSphere(transform.position, explosionRadius);

    }

}
