using UnityEngine;
using System.Collections;

public class ArrowTrap : Triggerable
{

    public bool shooting = false;
    public GameObject ammoRight;
    public GameObject ammoLeft;
    public float rate = 0.5f;

	void Start () {
        
	}
	
	void Update () {
	    if(shooting)
        {
            shooting = false;
            Invoke("OnShoot", rate);
        }
	}

    void OnShoot()
    {
        GameObject arrowRight = (GameObject)Instantiate(ammoRight, transform.position + new Vector3(0.8f, 0.6f, 0), Quaternion.identity);
        GameObject arrowLeft = (GameObject)Instantiate(ammoLeft, transform.position + new Vector3(-0.8f, 0.6f, 0), Quaternion.identity);
        arrowRight.transform.parent = transform;
        arrowLeft.transform.parent = transform;
    }
    
    public override void Open()
    {
        shooting = true;
    }

    public override void Close()
    {
        // not needed
    }
}
