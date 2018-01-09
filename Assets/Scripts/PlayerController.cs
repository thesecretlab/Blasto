using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ignore 'this variable is never written to' warnings, 
// because Unity will be setting them externally.
#pragma warning disable 0649

// PlayerController manages the spawning and scorekeeping for players.
// They're not visible; instead, they create and manage the visible objects
// that belong to a player.
public class PlayerController : MonoBehaviour {

	// --- Inspector fields ---

    // The UI label that shows our player number and colour.
	[SerializeField]
	UnityEngine.UI.Text playerLabel;

    // The UI label that shows our current score.
	[SerializeField]
	UnityEngine.UI.Text scoreLabel;

    // The object that represents the player.
	[SerializeField]
	GameObject avatarPrefab;

    // The data for this player. It's passed to an avatar's components so that
    // they can customise their behaviour appropriately.
	[SerializeField]
	PlayerConfiguration config;

    // How long to wait before respawning, after the avatar is destroyed.
	[SerializeField]
	float spawnDelay = 1.0f;

	// The sound effect to play when a player respawns.
	[SerializeField]
	SoundManager.Effect respawnEffect;

    // --- Private variables ---

    // The current avatar for this player in the game.
	GameObject currentAvatar;

    // The player score. (Managed by the 'score' property, below.)
	int _score;

    // When 'score' is updated, the scoreLabel's text is updated too.
    int score {
        get {
            return _score;
        }
        set {
            // Update our variable that's tracking score
            _score = value;

            // Update the label
            if (scoreLabel != null)
                scoreLabel.text = _score.ToString();
        }
    }

	// Creates a new avatar for this player.
	void Spawn(bool isFirstSpawn = false) {

		var selectedSpawn = SpawnPoint.GetRandomAvailableSpawnPoint ();
        
        // Get rid of the old avatar if it's still around
        if (currentAvatar != null) {
            Destroy(currentAvatar.gameObject);
        }

		// Our new avatar is an instance of the avatar prefab.
        currentAvatar = Instantiate(avatarPrefab, selectedSpawn.position, selectedSpawn.rotation);

		// If this is a REspawn, play the sound effect at this location
		if (isFirstSpawn == false) {
			SoundManager.instance.PlaySound (respawnEffect, currentAvatar.transform.position);
		}

		// Configure the components on the avatar.
        var input = currentAvatar.GetComponent<PlayerInput>();
        var appearance = currentAvatar.GetComponent<PlayerAppearance>();
        var weapons = currentAvatar.GetComponent<PlayerWeapons>();
        var health = currentAvatar.GetComponent<PlayerHealth>();

        if (input != null)
        {
            input.configuration = config;
        }
        else
        {
            Debug.LogWarningFormat("{0} can't configure input: not present on the prefab", this.name);
        }

        if (appearance != null)
        {
            appearance.configuration = config;
        }
        else
        {
            Debug.LogWarningFormat("{0} can't configure appearance: not present on the prefab", this.name);
        }

        if (weapons != null)
        {
            weapons.configuration = config;
        }
        else
        {
            Debug.LogWarningFormat("{0} can't configure weapons: not present on the prefab", this.name);
        }

        if (health) {
			// When the avatar reports that it's died, run this delegate.
			currentAvatar.GetComponent<PlayerHealth>().onDeath = delegate {

				// Respawn this player after a the right amount of time.
				StartCoroutine(SpawnAfterDelay(spawnDelay));

				CreatePilot ();

			};
        } else {
            Debug.LogWarningFormat("{0} can't configure health: not present on the prefab", this.name);
        }

         
    }

	// The force with which 
	[SerializeField]
	float _pilotEjectionForce = 2.0f;

	[SerializeField]
	float _pilotDespawnTime = 3.0f;

    // Called by other objects to give a point to this player.
    public void DestroyedOtherPlayer()
    {
        // Update the score; this will also update the score label.
        score++;

        // (We do this instead of making 'score' public because there's no need
        // for other objects to know what our score is, or to be able to set it 
        // directly. Keeps it tidier by doing this.)
    }

	// Called by other objects to indicate that the player destroyed themselves.
	public void DestroyedSelf()
	{
        // Deduct several points.
        score -= 3;
	}

    // Returns the PlayerController that is associated with the given player number.
    public static PlayerController FindWithNumber(PlayerConfiguration.PlayerNumber number)
    {
        // Loop through the list of all player controllers until we find the
        // one with the requested player number.
        var allPlayerControllers = FindObjectsOfType<PlayerController>();

        foreach (var controller in allPlayerControllers) {
            if (controller.config.playerNumber == number) {
                return controller;
            }
        }

        // We didn't find one with this number; return null.
        return null;
    }

    // A coroutine that waits a certain amount of time, and then spawns.
    // Called when the player dies.
    IEnumerator SpawnAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);

        Spawn();
    }

    // Called when the player first appears.
    void Start() {

		// Make our label show the correct color
		if (playerLabel != null)
			playerLabel.color = config.color;

		// Set our score to zero; this will also update the label
		score = 0;

		// Spawn the player immediately (without waiting); indicate that this is the first
		// time it's happened.
		Spawn(true);
    }

	// True if the player is currently alive.
	public bool isSpawned {
		get {
			return this.currentAvatar != null;
		}
	}

	// The position of the player.
	public Vector3 position {
		get {
			if (isSpawned == false) {
				throw new InvalidOperationException ("Can't get the position of a dead player. " +
					"Check the isSpawned property first.");
			}
			return this.currentAvatar.transform.position;
		}
	}


	void CreatePilot ()
	{
		// Bail out if we don't have a prefab to use.
		if (config.pilotPrefab == null) {
			return;
		}

		// Immediately spawn the pilot
		var prefab = config.pilotPrefab;
		var pilot = Instantiate (prefab, currentAvatar.transform.position, currentAvatar.transform.rotation);
		var pilotBody = pilot.GetComponentInChildren<Rigidbody> ();

		// Ensure that the pilot never collides with the current avatar
		var pilotCollider = pilot.GetComponentInChildren<Collider>();
		var avatarCollider = currentAvatar.GetComponentInChildren<Collider> ();
		Physics.IgnoreCollision (pilotCollider, avatarCollider);

		// Throw the pilot forward
		var throwDirection = (currentAvatar.transform.forward).normalized;
		var throwForce = throwDirection * _pilotEjectionForce;
		pilotBody.AddForce (throwForce);

		// Spin the pilot randomly
		pilotBody.AddTorque (UnityEngine.Random.insideUnitSphere * _pilotEjectionForce * 2);

		// Destroy this pilot after a delay
		Destroy (pilot.gameObject, _pilotDespawnTime);
	}
}
