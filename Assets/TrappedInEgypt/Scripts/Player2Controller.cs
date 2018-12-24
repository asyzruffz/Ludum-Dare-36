using UnityEngine;
using System.Collections;

public class Player2Controller : MovementController
{

	void Update()
    {
        moving.x = Input.GetAxis("Horizontal2");
        moving.y = Input.GetAxis("Vertical2");
        jumping = Input.GetButtonDown("Jump2");
        attacking = Input.GetButtonDown("Attack2");
    }
}
