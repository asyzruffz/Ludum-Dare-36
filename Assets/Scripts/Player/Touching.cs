using UnityEngine;
using UnityEngine.Networking;

public class Touching : NetworkBehaviour {

	private NetworkPlayerCharacter player;
	private NetworkTransform netTransform;
    
	void Start () {
		player = GetComponent<NetworkPlayerCharacter> ();
		netTransform = GetComponent<NetworkTransform> ();
    }

	[ServerCallback]
	void OnTriggerEnter2D(Collider2D target){
		if (netTransform) {
			netTransform.SetDirtyBit (1);
		}

		if (target.gameObject.CompareTag ("Deadly")) {
			AttackDamage damage = target.gameObject.GetComponent<AttackDamage>();
			if (damage) {
				player.BeingHit(damage);
			}
		} else if (target.gameObject.CompareTag ("Score")) {
			ScoreValue amount = target.gameObject.GetComponent<ScoreValue> ();
			if (amount) {
				player.GainScore (amount);
			}
		}

	}

	[ServerCallback]
	void OnCollisionEnter2D(Collision2D target) {
		if (netTransform) {
			netTransform.SetDirtyBit (1);
		}

		if (target.gameObject.CompareTag ("Deadly")) {
			AttackDamage damage = target.gameObject.GetComponent<AttackDamage>();
			if (damage) {
				player.BeingHit(damage);
			}
		}
	}
}
