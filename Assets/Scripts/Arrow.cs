using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

    public float speed = 1.0f;
    public Vector2 direction = new Vector2(1,0);
    public float lifetime = 1.0f;

    private float timer = 0.0f;

    private Rigidbody2D body;

	void Start () {
        body = GetComponent<Rigidbody2D>();
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        body.velocity = direction * speed;
    }
	
	void Update () {
        if (timer >= lifetime)
            Destroy(gameObject);
        
        //transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        timer += Time.deltaTime;
	}

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
        else if(target.gameObject.tag != "Through")
        {
            // stuck on wall
            body.velocity = Vector2.zero;
        }
    }
}
