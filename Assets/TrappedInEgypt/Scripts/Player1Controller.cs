using UnityEngine;
using System.Collections;

public class Player1Controller : MovementController
{

	void Start () {
	
	}

    void Update()
    {
        moving.x = Input.GetAxis("Horizontal");
        moving.y = Input.GetAxis("Vertical");
        jumping = Input.GetButtonDown("Jump");
        attacking = Input.GetButtonDown("Attack1");
    }
}
