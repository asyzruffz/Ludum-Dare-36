using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public bool isPlayer2 = false;
    public float speed = 10f;
    public float jumpSpeed = 15f;
    public float airSpeedMultiplier = .3f;
    public Vector2 maxVelocity = new Vector2(3, 5);
	public AudioClip walkSound;
	public AudioClip thudSound;
	public AudioClip floatingSound;

	private Animator animator;
	private PlayerController controller;
	private Rigidbody2D body;
    private bool onGround = false;

    void Start(){
        if(isPlayer2)
            controller = GetComponent<Player2Controller>();
        else
		    controller = GetComponent<Player1Controller>();
		animator = GetComponent<Animator>();
		body = GetComponent<Rigidbody2D>();

        maxVelocity.x = Mathf.Max(speed, maxVelocity.x);
        maxVelocity.y = Mathf.Max(jumpSpeed, maxVelocity.y);
    }

    void Update()
    {
        float absVelX = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x);
        float absVelY = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y);
        float velY = GetComponent<Rigidbody2D>().velocity.y;
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);

        onGround = Physics2D.Linecast(pos, pos + new Vector2(0, -1.5f), 1 << LayerMask.NameToLayer("Ground"));
        //onGround = (velY <= 0f && velY > -0.1f);
        animator.SetBool("OnGround", onGround);

        float forceX = 0f;
        if (controller.moving.x > 0 || controller.moving.x < 0)
        {
            int direction = controller.moving.x > 0 ? 1 : -1;
            forceX = onGround ? speed : (speed * airSpeedMultiplier);
            forceX *= direction;

            transform.localScale = new Vector3(direction, 1, 1);
            transform.Find("Heart1").localScale = new Vector3(direction, 1, 1);
            transform.Find("Heart2").localScale = new Vector3(direction, 1, 1);
            transform.Find("Heart3").localScale = new Vector3(direction, 1, 1);

            // walking animation
            animator.SetInteger("AnimState", 1);

            if (absVelX < maxVelocity.x)
                body.AddForce(new Vector2(forceX, 0));
        }
        else
        {
            // standing/idle animation
            animator.SetInteger("AnimState", 0);
        }

        float forceY = 0f;
        if (controller.jumping)
        {
            PlayRocketSound();
            if (absVelY < maxVelocity.y)
                forceY = jumpSpeed;

            // jumping animation
            animator.SetInteger("AnimState", 2);

            body.AddForce(new Vector2(0, forceY), ForceMode2D.Impulse);
        }

        // jumping or falling animation
        if(!onGround)
        {
            animator.SetInteger("AnimState", 2);
            if (velY < 0)
            {
                animator.SetInteger("AnimState", 3);
            }
        }

        if (controller.attacking)
        {
            // hitting animation
            animator.SetInteger("AnimState", 4);
        }
    }

    void PlayWalkSound(){
		if (walkSound)
			AudioSource.PlayClipAtPoint (walkSound, transform.position);
	}

	void PlayRocketSound(){
		if (!floatingSound || GameObject.Find ("RocketSound"))
			return;

		GameObject go = new GameObject ("RocketSound");
		AudioSource aSrc = go.AddComponent<AudioSource> ();
		aSrc.clip = floatingSound;
		aSrc.volume = 0.7f;
		aSrc.Play ();

		Destroy (go, floatingSound.length);

	}

	void OnCollisionEnter2D(Collision2D target){
		if (!onGround) {
			var absVelX = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x);
			var absVelY = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y);

			if(absVelX <= .1f || absVelY <= .1f){
				if(thudSound)
					AudioSource.PlayClipAtPoint(thudSound, transform.position);
			}
		}

	}
}
