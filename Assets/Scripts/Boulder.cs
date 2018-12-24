using UnityEngine;
using System.Collections;

public class Boulder : MonoBehaviour {

    public float lifetime;

    private float timer;
    private Rigidbody2D body;

	void Start () {
        body = GetComponent<Rigidbody2D>();
    }
	
	void Update () {
        if (timer >= lifetime)
            Destroy(gameObject);

        float speed = body.velocity.magnitude;
        if(speed <= 0.2f)
            timer += Time.deltaTime;
    }
}
