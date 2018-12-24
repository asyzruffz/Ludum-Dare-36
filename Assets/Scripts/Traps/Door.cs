using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Door : Triggerable
{

	public const int IDLE = 0;
	public const int OPENING = 1;
	public const int OPEN = 2;
	public const int CLOSING = 3;
	public float closeDelay = .5f;

    [HideInInspector]
	public int state = IDLE;
	private Animator animator;
    
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	void OnOpenStart() {
		state = OPENING;
	}

	void OnOpenEnd() {
		state = OPEN;
	}


	void OnCloseStart() {
		state = CLOSING;
	}
	
	void OnCloseEnd() {
		state = IDLE;
	}

	void DissableCollider2D() {
		GetComponent<Collider2D>().enabled = false;
	}

	void EnableCollider2D() {
		GetComponent<Collider2D>().enabled = true;
	}

	[Server]
	public override void Open () {
		animator.SetInteger ("AnimState", 1);
		RpcOpen ();
	}

	[ClientRpc]
	void RpcOpen () {
		animator.SetInteger ("AnimState", 1);
	}

	[Server]
	public override void Close () {
		StartCoroutine (CloseNow ());
		RpcClose ();
	}

	[ClientRpc]
	void RpcClose () {
		StartCoroutine (CloseNow ());
	}

	IEnumerator CloseNow (){
		yield return new WaitForSeconds(closeDelay);
		animator.SetInteger ("AnimState", 2);
	}
}
