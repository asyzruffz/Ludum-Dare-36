using UnityEngine;
using System.Collections;

public class Pain : MonoBehaviour {

    //public BodyPart bodyPart;
    //public int totalParts;

    private Health playerHealth;
    
	void Start () {
        playerHealth = GetComponent<Health>();

    }
	
	void Update () {
	    //if(playerHealth.isDead())
            //Destroy(gameObject);
    }

	void OnTriggerEnter2D(Collider2D target){
		if (target.gameObject.tag == "Deadly") {
            var damage = target.gameObject.GetComponent<AttackDamage>();
            playerHealth.BeingHit(damage.damageLives, damage.damageHP);
		}

	}

	void OnCollisionEnter2D(Collision2D target){
		if (target.gameObject.tag == "Deadly") {
            var damage = target.gameObject.GetComponent<AttackDamage>();
            playerHealth.BeingHit(damage.damageLives, damage.damageHP);
		}
		
	}
}
