using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public int currentLives = 3;
    public float currentHP = 100.0f;
    public int maxLives = 3;
    public float maxHP = 100.0f;
    public float invincibleTime = 1.0f;
    public bool isInvincible = false;

    private float timer = 0f;
    
	void Start () {
        currentLives = maxLives;
        currentHP = maxHP;
    }
	
	void Update () {
	    if(timer >= invincibleTime)
        {
            isInvincible = false;
        }

        UpdateHealthUI();
        timer += Time.deltaTime;
	}

    public bool isDead()
    {
        return currentLives < 1;
    }

    public void BeingHit(int damageLives, float damageHP)
    {
        if (isInvincible)
            return;

        currentLives -= damageLives;
        currentHP -= damageHP;
        while (currentHP <= 0 && currentLives > 0)
        {
            currentLives--;
            currentHP += maxHP;
        }

        if(isDead())
        {
            Destroy(gameObject);
        }
        else
        {
            isInvincible = true;
            timer = 0f;
        }
    }

    void UpdateHealthUI()
    {
        if (currentLives < 1)
            transform.Find("Heart1").gameObject.SetActive(false);
        if (currentLives < 2)
            transform.Find("Heart2").gameObject.SetActive(false);
        if (currentLives < 3)
            transform.Find("Heart3").gameObject.SetActive(false);
    }
}
