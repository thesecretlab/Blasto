using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	[System.Serializable]
	public class EffectData {
		public Effect effect;
		public AudioClip[] clips;
	}

	public enum Effect {
		None,
		Respawn,
		PlayerExplosion,
		ShotExplosion,
		Shot
	}

	public static SoundManager instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<SoundManager> ();
			}
			return _instance;
		}
	}

	static SoundManager _instance;

	[SerializeField]
	float volume = 1.0f;

	void Awake() {
		AudioListener.volume = volume;
	}

	EffectData GetEffectData(Effect effect) {
		foreach (var effectData in _effects) {
			if (effectData.effect == effect) {
				return effectData;
			}
		}
		Debug.LogErrorFormat ("No effect data found for {0}", effect);
		return null;
	}

	[SerializeField]
	EffectData[] _effects;

	public AudioClip GetClip(Effect effect) {
		var data = GetEffectData (effect);

		var clip = data.clips [Random.Range (0, data.clips.Length)];

		return clip;
	}

	public void PlaySound(Effect effect, Vector3 worldPosition) {

		// Bail out if we were told to play a nil effect
		if (effect == Effect.None) {
			return;
		}

		var clip = GetClip (effect);

		AudioSource.PlayClipAtPoint (clip, worldPosition);
	}

}
