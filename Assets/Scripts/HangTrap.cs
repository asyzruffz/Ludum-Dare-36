using UnityEngine;
using System.Collections;

public class HangTrap : Triggerable
{

    public GameObject weights;
    public float reloadRate = 1f;
    public bool release = false;

    private float timer = 0;
    private Animator animator;
    
    void Start () {
        animator = GetComponent<Animator>();
    }
	
	void Update () {
        if (release)
        {
            release = false;
            Invoke("OnRelease", 0);
            timer = 0;
        }

        if(timer >= reloadRate)
        {
            animator.SetInteger("AnimState", 0);
        }

        timer += Time.deltaTime;
    }

    void OnRelease()
    {
        animator.SetInteger("AnimState", 1);
        GameObject w = (GameObject)Instantiate(weights, transform.position + new Vector3(1.5f, 0, 0), Quaternion.identity);
        w.transform.parent = this.transform;
    }

    public override void Open()
    {
        release = true;
    }

    public override void Close()
    {
        // not needed
    }
}
