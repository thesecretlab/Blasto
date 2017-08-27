using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The PlayerInput class takes raw input from the user, and converts it to a 
// simpler form that the rest of the game can use.
public class PlayerInput : MonoBehaviour {

    // An Axis represents a value that can range from -1 to 1. When it receives
    // input, it gradually moves to the target value.
    public class Axis {

        // The current value, ranging from -1 to 1.
        public float value;

        // Moves the value towards 
        public void Move(int direction, float speed) {

            // Ensure that 'direction' is either +1, -1 or 0
            direction = (int) Mathf.Sign(direction);

            // Move towards 'direction' over time.
            value += direction * speed * Time.deltaTime;

            // Clampo ourselves to prevent going out of bounds
            value = Mathf.Clamp(value, -1, 1);
        }

    }

    // A button simply represents an on-off value.
    public class Button {
        public bool isActive;
    }

    // --- Public variables ---
    public Axis rotation = new Axis();
    public Axis thrust = new Axis();
    public Button boost = new Button();
    public Button fire = new Button();

    // The configuration stores the key codes that this player should be looking
    // at.
    public PlayerConfiguration configuration;

    // The rate at which we throttle up forward/reverse thrust.
    [SerializeField]
    float turnInertia;
	
    // The rate at which we throttle up turning torque.
    [SerializeField]
	public float thrustInertia;

    // Each frame, we read the raw input and update our axes and buttons.
    void Update()
    {

        // If we don't have a configuration, we can't know what keys to use.
        // So, we have to bail out.
        if (configuration == null)
            return;

        // Read the left and right keys, and adjust the rotation axis accordingly

        if (Input.GetKey(configuration.turnLeftKey)) {
            rotation.Move(-1, turnInertia);
        } else if (Input.GetKey(configuration.turnRightKey)) {
            rotation.Move(1, turnInertia);
        } else {
            // Reset to zero immediately if no input
            rotation.value = 0;
        }

		// Read the forward and reverse keys, and adjust the rotation axis accordingly
		
        if (Input.GetKey(configuration.increaseThrustKey)) {
            thrust.Move(1, thrustInertia);
        } else if (Input.GetKey(configuration.decreaseThrustKey)) {
            thrust.Move(-1, thrustInertia);
        } else {
            // Reset to zero immediately if no input
            thrust.value = 0;
		}

        // Fire is active if the fire key is being held down
        fire.isActive = Input.GetKey(configuration.fireKey);

        // Boost is only active on the frame that the Boost key _starts_ being held down
        boost.isActive = Input.GetKeyDown(configuration.boostKey);

    }
}
