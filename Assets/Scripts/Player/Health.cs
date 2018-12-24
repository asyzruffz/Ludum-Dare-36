using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public int maxLives = 3;
    public float maxHP = 100.0f;
    public float invincibleTime = 1.0f;

	private NetworkCharacterInfo charInfo;

	public int currentLife {
		get {
			if (charInfo != null) {
				return charInfo.life;
			} else {
				Debug.Log ("Missing info for Life!");
				return -1;
			}
		}
	}

	public float currentHP {
		get {
			if (charInfo != null) {
				return charInfo.hp;
			} else {
				Debug.Log ("Missing info for HP!");
				return -1;
			}
		}
	}

	void Start () {
		charInfo = GetComponent<NetworkCharacterInfo> ();
    }
	
    public bool IsDead() {
        return currentLife < 1;
    }
}
