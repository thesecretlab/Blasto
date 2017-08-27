using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerAppearance updates the colour of the player object. 
// It assumes that it has a child MeshRenderer, which uses a material called 
// "metal". This material will be customised to use the desired colour.
public class PlayerAppearance : MonoBehaviour {

    // --- Inspector fields ---

    // The color we're using.
    [SerializeField]
    Color color;

    // Called when the object first appears.
    void Awake()
    {
        // Update our colour immediately.
        ApplyColor();
    }

    // When this property is set, the colour stored in the configuration
    // is used to update our display colour.
    public PlayerConfiguration configuration {
        set {
            // Bail out if we were passed null
            if (value == null)
                return;

            // Get the colour out of the config
            color = value.color;

            // Apply this color change.
            ApplyColor();
        }
    }

    // 
    public void ApplyColor () {

        var meshRenderer = GetComponentInChildren<MeshRenderer>();

		// We have to make a local copy, make changes, and put them back
		var materials = meshRenderer.materials;

        // Find the main colour material, which is named "metal"
        // (Unity will append "(Instance)" to it)
		foreach (var material in materials)
		{
            // Does this material's name start with "metal", ignoring case?
            if (material.name.StartsWith("metal", System.StringComparison.CurrentCultureIgnoreCase))
			{
                // If so, adjust its colour.
                material.color = this.color;
			}
		}

        // Update the list of materials that this renderer uses.
		meshRenderer.materials = materials;
	}
	
	
}
