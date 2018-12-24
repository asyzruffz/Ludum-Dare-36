using UnityEngine;
using UnityEngine.Networking;

public class Collectable : NetworkBehaviour {

	public AudioClip pickupSound;

	[ServerCallback]
	void OnTriggerEnter2D(Collider2D target){
		if (target.gameObject.CompareTag ("Player")) {
			if(pickupSound) {
				AudioSource.PlayClipAtPoint (pickupSound, transform.position);
				RpcPlaySound ();
			}

			NetworkServer.Destroy (gameObject);
		}
	}

	[ClientRpc]
	void RpcPlaySound () {
		AudioSource.PlayClipAtPoint (pickupSound, transform.position);
	}
}
