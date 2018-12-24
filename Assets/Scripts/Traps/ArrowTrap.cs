using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ArrowTrap : Triggerable {

    public GameObject ammoRight;
    public GameObject ammoLeft;
    public float rate = 0.5f;

	private bool shooting = false;
	
	[ServerCallback]
	void Update () {
		if (!isServer)
			return;

	    if(shooting) {
            shooting = false;
            Invoke("OnShoot", rate);
        }
	}

    void OnShoot() {
        GameObject arrowRight = Instantiate(ammoRight, transform.position + new Vector3(0.8f, 0.6f, 0), Quaternion.identity, transform);
		GameObject arrowLeft = Instantiate(ammoLeft, transform.position + new Vector3(-0.8f, 0.6f, 0), Quaternion.identity, transform);
		NetworkServer.Spawn (arrowRight);
		NetworkServer.Spawn (arrowLeft);
	}
    
	[Server]
    public override void Open() {
        shooting = true;
    }

	[Server]
	public override void Close () {
        // not needed
    }
}
