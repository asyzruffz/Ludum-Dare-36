using UnityEngine;
using UnityEngine.Networking;

public class TrapDoor : NetworkBehaviour {

    public float resetTime = 1f;

    private Door door;
    private float timer = 0;

    void Start () {
        door = GetComponent<Door>();
    }

	[ServerCallback]
	void Update() {
		if (!isServer)
			return;

        if (door.state == Door.OPEN) {
            if (timer >= resetTime) {
                door.Close();
            }

            timer += Time.deltaTime;
        }
    }
}
