using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent (typeof (NetworkTransform))]
[RequireComponent (typeof (Rigidbody2D), typeof(Animator))]
public class NetworkPlayerCharacter : NetworkBehaviour {
	
	[Header("Movement")]
    public float speed = 10f;
    public float jumpSpeed = 15f;
    public float airSpeedMultiplier = .3f;
    public Vector2 maxVelocity = new Vector2(3, 5);

	[Header("Sounds")]
	public AudioClip walkSound;
	public AudioClip thudSound;
	public AudioClip floatingSound;

	private MovementController controller;
	private NetworkCharacterInfo charInfo;
	private Animator animator;
	private Rigidbody2D body;
	private Collider2D[] coll;
    private bool onGround = false;
	private bool canControl = true;
	private bool isAttacking = false;

	void Awake () {
		GameMaster.sPlayers.Add (this);
	}

    void Start(){
        controller = GetComponent<MovementController> ();
		charInfo = GetComponent<NetworkCharacterInfo> ();
		animator = GetComponent<Animator>();
		body = GetComponent<Rigidbody2D>();
		coll = GetComponents<Collider2D> ();

        maxVelocity.x = Mathf.Max(speed, maxVelocity.x);
        maxVelocity.y = Mathf.Max(jumpSpeed, maxVelocity.y);

		//We don't want to handle collision on client, so disable collider there
		//foreach (Collider2D c in coll) {
			//c.enabled = isServer;
		//}
	}

	void OnDestroy () {
		GameMaster.sPlayers.Remove (this);
	}
	
	[ClientCallback]
	void Update ()
    {
		if (!isLocalPlayer || !canControl)
			return;
		
    }

	[ClientCallback]
	void FixedUpdate () {
		if (!hasAuthority)
			return;

		if (!canControl) { //if we can't control, mean we're destroyed, so make sure the character stay in spawn place
			body.rotation = 0;
			body.position = Vector2.zero;
			body.velocity = Vector2.zero;
			body.angularVelocity = 0;
		} else {

			float absVelX = Mathf.Abs (body.velocity.x);
			float absVelY = Mathf.Abs (body.velocity.y);
			Vector2 pos = new Vector2 (transform.position.x, transform.position.y);

			onGround = Physics2D.Linecast (pos, pos + new Vector2 (0, -1.5f), 1 << LayerMask.NameToLayer ("Ground"));

			float forceX = 0f;
			if (controller.moving.x > 0 || controller.moving.x < 0) {
				int direction = controller.moving.x > 0 ? 1 : -1;
				forceX = onGround ? speed : (speed * airSpeedMultiplier);
				forceX *= direction;

				if (absVelX < maxVelocity.x) {
					body.AddForce (new Vector2 (forceX, 0));
				}
			}

			float forceY = 0f;
			if (controller.jumping) {
				if (absVelY < maxVelocity.y)
					forceY = jumpSpeed;

				PlayRocketSound ();
				body.AddForce (new Vector2 (0, forceY), ForceMode2D.Impulse);
			}

			isAttacking = controller.attacking;
		}
	}

	[ClientCallback]
	void LateUpdate () {

		animator.SetBool ("OnGround", onGround);

		if (body.velocity.x > 0 || body.velocity.x < 0) {
			int direction = (int)Mathf.Sign (body.velocity.x);
			GetComponent<SpriteRenderer>().flipX = (direction > 0) ? false : true;

			// walking animation
			animator.SetInteger ("AnimState", 1);
		} else {
			// standing/idle animation
			animator.SetInteger ("AnimState", 0);
		}

		// jumping or falling animation
		if (!onGround) {
			animator.SetInteger ("AnimState", 2);
			if (body.velocity.y < 0) {
				animator.SetInteger ("AnimState", 3);
			}
		}

		if (isAttacking) {
			// hitting animation
			animator.SetInteger ("AnimState", 4);
		}
	}

	void PlayWalkSound(){
		if (walkSound)
			AudioSource.PlayClipAtPoint (walkSound, transform.position);
	}

	void PlayThudSound () {
		if (thudSound)
			AudioSource.PlayClipAtPoint (thudSound, transform.position);
	}

	void PlayRocketSound (){
		if (!floatingSound || GameObject.Find ("RocketSound"))
			return;

		GameObject go = new GameObject ("RocketSound");
		AudioSource aSrc = go.AddComponent<AudioSource> ();
		aSrc.clip = floatingSound;
		aSrc.volume = 0.7f;
		aSrc.Play ();

		Destroy (go, floatingSound.length);

	}

	[ClientCallback]
	void OnCollisionEnter2D(Collision2D target) {
		if (isServer)
			return; // hosting client, server path will handle collision

		if (!onGround) {
			var absVelX = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x);
			var absVelY = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y);

			if(absVelX <= .1f || absVelY <= .1f){
				PlayThudSound ();
			}
		}
	}

	//We can't disable the whole object, as it would impair synchronisation/communication
	//So disabling mean disabling collider & renderer only
	public void EnableCharacter (bool enable) {
		GetComponent<Renderer> ().enabled = enable;
		foreach (Collider2D c in coll) {
			c.enabled = isServer && enable;
		}

		canControl = enable;
	}

	[Client]
	public void LocalDestroy () {
		/*killParticle.transform.SetParent (null);
		killParticle.transform.position = transform.position;
		killParticle.gameObject.SetActive (true);
		killParticle.time = 0;
		killParticle.Play (); */

		if (!canControl)
			return; //already destroyed, happen if destroyed Locally, Rpc will call that later

		EnableCharacter (false);
	}

	//this tell the game this should ONLY be called on server, will ignore call on client & produce a warning
	[Server]
	public void BeingHit (AttackDamage dmg) {
		if (charInfo.isInvincible)
			return;

		Health health = GetComponent<Health> ();

		int oldLife = charInfo.life;
		charInfo.life -= dmg.damageLife;
		charInfo.hp -= dmg.damageHP;
		while (charInfo.hp <= 0 && charInfo.life > 0) {
			charInfo.life--;
			charInfo.hp += health.maxHP;
		}
		
		if (charInfo.life < oldLife) {
			Kill ();
		}

		if (!health.IsDead ()) {
			StartCoroutine (BeInvincible (health));
		}
	}

	[Server]
	public void GainScore (ScoreValue score) {
		if (charInfo.isInvincible)
			return;
		
		charInfo.score += score.amount;
	}

	IEnumerator BeInvincible (Health health) {
		charInfo.isInvincible = true;
		yield return new WaitForSeconds (health.invincibleTime);
		charInfo.isInvincible = false;
	}

	[Server]
	public void Kill () {
		EnableCharacter (false);
		RpcDestroyed ();

		if (charInfo.life > 0) {
			Debug.Log ("Die! Waiting to respawn...");
			//we start the coroutine on the manager, as disabling a gameobject stop ALL coroutine started by it
			GameMaster.GManager.StartCoroutine (GameMaster.GManager.WaitForRespawn (this));
		} else {
			Debug.Log ("Die permanently! Back to Lobby...");
			GameMaster.SetGameEndStatus (true);
		}
	}

	[Server]
	public void Respawn () {
		charInfo.hp = GetComponent<Health> ().maxHP;
		EnableCharacter (true);
		RpcRespawn ();
		StartCoroutine (BeInvincible (GetComponent<Health> ()));
	}

	//called on client when the player die, spawn the particle (this is only cosmetic, no need to do it on server)
	[ClientRpc]
	void RpcDestroyed () {
		LocalDestroy ();
	}

	// =========== NETWORK FUNCTIONS

	[ClientRpc]
	void RpcRespawn () {
		EnableCharacter (true);
	}
}
