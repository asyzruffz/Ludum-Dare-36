using UnityEngine;
using UnityEngine.Networking;

public class Arrow : NetworkBehaviour {

    public float speed = 1.0f;
    public Vector2 direction = new Vector2(1,0);
    public float lifetime = 1.0f;

    private float timer = 0.0f;
    private Rigidbody2D body;
	private NetworkTransform netTransform;

	void Start () {
        body = GetComponent<Rigidbody2D>();
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        body.velocity = direction * speed;
    }

	[ServerCallback]
	void Update () {
		if (!isServer)
			return;

		if (timer >= lifetime) {
			NetworkServer.Destroy (gameObject);
		}

		timer += Time.deltaTime;
	}

	[ServerCallback]
	void OnTriggerEnter2D(Collider2D target) {
        if (target.gameObject.CompareTag ("Player")) {
			NetworkServer.Destroy (gameObject);
		} else if(!target.gameObject.CompareTag ("Through")) {
            // stuck on wall
            body.velocity = Vector2.zero;
			RpcStucked ();
		}
    }

	[ClientRpc]
	void RpcStucked () {
		body.velocity = Vector2.zero;
	}
}
