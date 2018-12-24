using UnityEngine;
using System.Collections;

public class TrapDoor : MonoBehaviour {

    public float resetTime = 1f;

    private Door door;
    private float timer = 0;

    void Start () {
        door = GetComponent<Door>();
    }

    void Update() {
        if (door.state == Door.OPEN)
        {
            if (timer >= resetTime)
            {
                door.Close();
            }

            timer += Time.deltaTime;
        }
    }
}
