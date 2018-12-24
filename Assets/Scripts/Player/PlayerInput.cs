using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player2D))]
public class PlayerInput : MonoBehaviour {

	private Player2D player;

	void Start () {
		player = GetComponent<Player2D>();
	}

	void Update () {
		Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player.SetDirectionalInput (directionalInput);

		if (Input.GetButtonDown ("Jump")) {
			player.OnJumpInputDown ();
		}
		if (Input.GetButtonUp ("Jump")) {
			player.OnJumpInputUp ();
		}
	}
}
