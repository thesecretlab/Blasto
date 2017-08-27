using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ignore 'this variable is never written to' warnings, 
// because Unity will be setting them externally.
#pragma warning disable 0649

// PlayerMovement responds to input by turning and moving the player on command.
public class PlayerMovement : MonoBehaviour {

	// --- Inspector Fields ---

    [Header("Movement")]
    // The amount of turning force to apply.
	[SerializeField]
	float turnSpeed = 1.0f;

    // The amount of thrust force to apply.
	[SerializeField]
	float speed = 5.0f;

    // The amount of force to apply when the user requests a boost.
	[SerializeField]
	float boostImpulse = 200;

    [Header("Dust")]
	// The particle system that appears when we're going fast
	[SerializeField] ParticleSystem dustTrail;

	// The dust particles are visible if our speed is at or over this value.
	[SerializeField]
    float dustSpeedThreshold = 20;

	// --- Private variables ---

	// The input object; we'll be using this to find out what the player
	// wants to do.
	PlayerInput input;


    // The rigidbody that moves in response to physical forces.
    Rigidbody body;

    // Called when the object first appears.
	void Awake()
    {

        // Get references to the components used in Update()
        input = GetComponent<PlayerInput>();
        body = GetComponent<Rigidbody>();
    }

	void Update()
	{

        // Calculate our thrust force by multiplying the thrust input axis
        // (which is 0 to 1) by speed.
        var thrust = input.thrust.value * speed;

        // If we're boosting, add that too.
        if (input.boost.isActive) {
            thrust += boostImpulse;
        }

        // The force is in a forward direction
        var force = transform.forward * thrust;

        // Apply the movement force.
		body.AddForce(force);

        // Next, we turn:

		// Calculate our turning torque - we rotate around the Y axis
		var twist = input.rotation.value * turnSpeed;

        var torque = new Vector3(0, twist, 0);

        // Apply the torque.
        body.AddTorque(torque);

        // What's our current speed?
        var currentSpeed = body.velocity.magnitude;

        // If we're going fast enough, enable the dust particles.
        // Otherwise, stop them.
        if (currentSpeed > dustSpeedThreshold) {
            dustTrail.Play();
        } else {
            // Stop emitting particles, but let existing ones fade out.
            dustTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
	}


}
