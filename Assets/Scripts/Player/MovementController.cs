using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MovementController : NetworkBehaviour {

	public Vector2 moving = new Vector2();
    public bool jumping = false;
    public bool attacking = false;

	[ClientCallback]
	void Update () {
		if (!isLocalPlayer)
			return;

		moving.x = Input.GetAxis ("Horizontal");
		moving.y = Input.GetAxis ("Vertical");
		jumping = Input.GetButtonDown ("Jump");
		attacking = Input.GetButtonDown ("Fire1");
	}
}
