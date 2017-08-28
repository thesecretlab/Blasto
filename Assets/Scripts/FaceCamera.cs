using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Execute in Edit Mode because we want 'OnWillRenderObject' to run in the scene view.
[ExecuteInEditMode]
public class FaceCamera : MonoBehaviour {


	// Called by Unity just before a camera renders this object.
	void OnWillRenderObject()
	{
		// In this method, we'll rotate to face the camera that's rendering.
		// This means that we'll always be looking at the sprite face-on. 
		// (This is sometimes called 'billboard' rendering)

		// Camera.current is whatever camera we're rendering with
		// In the editor, there are two: the Camera object that the
		// player views through, and also the Scene camera, which is what
		// we see the camera through
		var renderCamera = Camera.current;

		// Rotate ourselves to look at this camera
		transform.LookAt(
			transform.position + renderCamera.transform.rotation * Vector3.forward,
			renderCamera.transform.rotation * Vector3.up
		);
	}
}
