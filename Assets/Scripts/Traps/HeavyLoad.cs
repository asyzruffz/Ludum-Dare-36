using UnityEngine;
using UnityEngine.Networking;

public class HeavyLoad : NetworkBehaviour {

    public float lifetime;

    private float timer;
    private Rigidbody2D body;
	private NetworkTransform netTransform;

	void Start () {
        body = GetComponent<Rigidbody2D>();
		netTransform = GetComponent<NetworkTransform> ();
	}

	[ServerCallback]
	void Update () {
		if (!isServer)
			return;

        if (timer >= lifetime) {
			NetworkServer.Destroy (gameObject);
		}

        float speed = body.velocity.magnitude;
		if (speed <= 0.2f) {
			timer += Time.deltaTime;
		} else {
			timer = 0;
		}
	}

	[ServerCallback]
	void OnTriggerEnter2D (Collider2D target) {
		if (netTransform) {
			netTransform.SetDirtyBit (1);
		}
	}

	[ServerCallback]
	void OnCollisionEnter2D (Collision2D target) {
		if (netTransform) {
			netTransform.SetDirtyBit (1);
		}
	}
}
