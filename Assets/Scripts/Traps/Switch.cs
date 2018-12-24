using UnityEngine;
using UnityEngine.Networking;

public class Switch : NetworkBehaviour {

	public ObjectTrigger[] triggers;
	public bool sticky;
    public bool autoReset;
    public float stickyDuration;

	private Animator animator;
	private float timer = 0;
	private bool down;
	private bool entered;

    void Start () {
		animator = GetComponent<Animator> ();
	}

	[ServerCallback]
	void Update () {
		if (!isServer)
			return;

        if(autoReset && down) {
            if(timer >= stickyDuration) {
                timer = 0;
                down = false;

				animator.SetInteger ("AnimState", 0);
				foreach (ObjectTrigger trigger in triggers) {
                    if (trigger != null)
                        trigger.Toggle(false);
                }
				
				RpcClose ();
			}

            timer += Time.deltaTime;
        }
    }

	[ServerCallback]
	void OnTriggerEnter2D(Collider2D target) {
        if (target.gameObject.CompareTag ("Player")) {
            down = true;
			animator.SetInteger ("AnimState", 1);
			RpcOpen ();

			foreach (ObjectTrigger trigger in triggers) {
				if (trigger != null)
					trigger.Toggle (true);
			}
		}
	}

	[ServerCallback]
	void OnTriggerExit2D(Collider2D target){

		if (sticky && down)
			return;

		down = false;
		entered = false;
		animator.SetInteger ("AnimState", 0);
		RpcClose ();

		foreach (ObjectTrigger trigger in triggers) {
			if(trigger != null)
				trigger.Toggle(false);
		}
	}

	[ClientRpc]
	void RpcOpen () {
		animator.SetInteger ("AnimState", 1);
	}

	[ClientRpc]
	void RpcClose () {
		animator.SetInteger ("AnimState", 0);
	}

	void OnDrawGizmos (){
		Gizmos.color = sticky ? Color.red : Color.blue;

		foreach (ObjectTrigger trigger in triggers) {
			if(trigger != null)
				Gizmos.DrawLine (transform.position, trigger.transform.position);
				//Gizmos.DrawLine(transform.position, trigger.triggerObject.transform.position);
		}

	}

	[Server] // This is called by TrapManager
    public bool isDown() {
        if (down && !entered) {
            entered = true;
            return true;
        }

        return false;
    }
}
