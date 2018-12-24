using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HangTrap : Triggerable {

    public GameObject weights;
    public float reloadRate = 1f;

	private Animator animator;
	private bool release = false;
    private float timer = 0;
    
    void Start () {
        animator = GetComponent<Animator>();
    }
	
	[ServerCallback]
	void Update () {
		if (!isServer)
			return;

        if (release) {
            release = false;
			OnRelease ();
            timer = 0;
        }

        if (timer >= reloadRate) {
            animator.SetInteger("AnimState", 0);
			RpcReload ();
		}

        timer += Time.deltaTime;
    }

	[Server]
    void OnRelease () {
		animator.SetInteger ("AnimState", 1);
		RpcRelease ();

        GameObject w = Instantiate(weights, transform.position + new Vector3(1.5f, 0, 0), Quaternion.identity, transform);
		NetworkServer.Spawn (w);
    }

	[ClientRpc]
	void RpcRelease () {
		animator.SetInteger ("AnimState", 1);
	}

	[ClientRpc]
	void RpcReload () {
		animator.SetInteger ("AnimState", 0);
	}

	[Server]
	public override void Open () {
        release = true;
    }

	[Server]
	public override void Close () {
        // not needed
    }
}
