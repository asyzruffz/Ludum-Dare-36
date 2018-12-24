using UnityEngine;
using UnityEngine.Networking;


public class NetworkCharacterInfo : NetworkBehaviour {

	//Network syncvar
	[SyncVar]
	public string playerName;
	[SyncVar]
	public Color color;

	[Space]

	[SyncVar (hook = "OnScoreChanged")]
	public int score;
	[SyncVar (hook = "OnLifeChanged")]
	public int life;
	[SyncVar (hook = "OnHPChanged")]
	public float hp;
	[SyncVar]
	public bool isInvincible;

	// --- Score & Life management & display
	void OnScoreChanged (int newValue) {
		score = newValue;
		//UpdateScoreText ();
	}

	void OnLifeChanged (int newValue) {
		life = newValue;
		UpdateHealthUI ();
	}

	void OnHPChanged (float newValue) {
		hp = newValue;
		UpdateHealthUI ();
	}

	void UpdateHealthUI () {
		if (life < 1)
			transform.Find ("Heart1").gameObject.SetActive (false);
		if (life < 2)
			transform.Find ("Heart2").gameObject.SetActive (false);
		if (life < 3)
			transform.Find ("Heart3").gameObject.SetActive (false);
	}
}
