using UnityEngine;
using UnityEngine.Networking;

public abstract class Triggerable : NetworkBehaviour {
	public abstract void Open ();

	public abstract void Close ();
}

public class ObjectTrigger : NetworkBehaviour {

	public Triggerable triggerObject;
	public bool ignoreTrigger;
    public bool sticky;
	
	[ServerCallback]
	void OnTriggerEnter2D(Collider2D target) {

		if (ignoreTrigger)
			return;

		if (target.gameObject.CompareTag ("Player")) {
			triggerObject.Open();
		}
	}

	[ServerCallback]
	void OnTriggerExit2D(Collider2D target) {
		if (ignoreTrigger || sticky)
			return;

		if (target.gameObject.CompareTag ("Player")) {
			triggerObject.Close();
		}
	}

	[Server]
	public void Toggle(bool value) {
		if (value)
			triggerObject.Open ();
		else
			triggerObject.Close ();
	}

	void OnDrawGizmos(){
		Gizmos.color = ignoreTrigger ? Color.gray : Color.green;

		var bc2d = GetComponent<BoxCollider2D> ();
		var bc2dPOS = bc2d.transform.position;
		var newPos = new Vector2 (bc2dPOS.x + bc2d.offset.x, bc2dPOS.y + bc2d.offset.y);

		Gizmos.DrawWireCube (newPos, new Vector2 (bc2d.size.x, bc2d.size.y));

	}

}
