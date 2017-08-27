using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ignore 'this variable is never written to' warnings, 
// because Unity will be setting them externally.
#pragma warning disable 0649

// Player health is responsible for managing the player's hit points.
// It's also responsible for responding to damage, and for causing the player to
// die when hitpoints reach zero.
public class PlayerHealth : MonoBehaviour {

	// --- Inspector fields ---

    [Header("Hit Points")]

    // How many hit points do we start with?
	[SerializeField]
	int startingHealth = 10;

    // How long to be invincible for immediately after spawning.
	[SerializeField]
	float spawnInvincibilityTime = 1.0f;

    [Header("Effects")]

    // The explosion object to create when we die.
    [SerializeField]
	Explosion deathExplosion;

    // The particle effect to show when we're damaged.
	[SerializeField]
	ParticleSystem smokeParticles;

    // Show smoke when our health (as a percentage of startingHealth) is at or
    // below this value.
    [SerializeField]
    [Range(0, 1)]
    float smokeVisibilityTreshold = 0.5f;

	// --- Private variables ---

    // How many hit points do we currently have?
	int currentHealth;

    // Are we currently immune to damage?
	bool isInvincible;


	// --- Public variables ---

    // The data representing the player that owns us. Used to control who
    // owns the deathExplosion, and to check to see if damage came from us.
	[HideInInspector]
    public PlayerConfiguration configuration;

    // A delegate to call when we die.
    // (No [HideInInspector] needed because Actions are never shown anyway.)
    public Action onDeath;

    // Called when the object first appears.
    void Awake()
    {
        // We start out with 'startingHealth' hit points.
        currentHealth = startingHealth;

        // Ensure that the smoke particles are not visible
        smokeParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Become invincible for a while after we first appear
        StartCoroutine(BecomeInvincible(spawnInvincibilityTime));
    }

    // A coroutine that makes the object become invincible for a fixed time.
    IEnumerator BecomeInvincible(float duration) {

        isInvincible = true;

        yield return new WaitForSeconds(duration);

        isInvincible = false;
    }

    public void TakeDamage(int amount, PlayerConfiguration.PlayerNumber attacker) {

        // Ignore any damage we take if we're currently invincible
        if (isInvincible) {
            return;
        }

        // Record the change in damage
        currentHealth -= amount;

        // Start the smoke system if health <= 50%
        if (smokeParticles.isStopped) {
            var healthPercentage = currentHealth / (float)startingHealth;

            if (healthPercentage <= smokeVisibilityTreshold) {
                smokeParticles.Play();
            }
        }

        // If we're at zero or less health, we're dead
        if (currentHealth <= 0) {
            Die();


			// Give our attacker the point
			var attackerController = PlayerController.FindWithNumber(attacker);

            if (attackerController != null) {

				if (configuration != null && configuration.playerNumber == attacker)                    
				{
					// If attackerController is us, tell it that it destroyed
					// itself.
					attackerController.DestroyedSelf();
                } else {
                    // Otherwise, tell it that it destroyed another player.
                    attackerController.DestroyedOtherPlayer();
                }

			}
        }
    }

    // Called when health drops to zero or lower.
    private void Die()
    {
        // Become invincible, so that we avoid an infinite loop caused by our
        // death explosion hurting us
        isInvincible = true;

        // Stop and detach all particle systems
        foreach (var particle in GetComponentsInChildren<ParticleSystem>()) {

            // Stop the particle effect, but don't immediately remove the particles
            particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            // Detach the particle effect so that it doesn't vanish when we die
            particle.transform.SetParent(null, true);

            // But do destroy the effect in a few seconds, to prevent the scene
            // from getting cluttered with dead particle systems
            Destroy(particle, 5);
        }

        // Create an explosion if we have a prefab for it
        if (deathExplosion != null) {
            var explosion = Instantiate(deathExplosion, transform.position, transform.rotation);

            explosion.owner = configuration;
        }

        // Report that we died
        if (onDeath != null)
            onDeath();

        // Remove this whole gameobject
        Destroy(gameObject);
    }
}
