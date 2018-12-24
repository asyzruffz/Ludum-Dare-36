using UnityEngine;
using System.Collections;

public class TrapManager : MonoBehaviour {

    public Switch[] switches;
    public ObjectTrigger[] triggers;
    public int averageNumToActivate = 10;
    public int randomRange = 5;

    void Start () {
        
    }
	
	void Update () {
        foreach (Switch s in switches)
        {
            if(s.isDown())
            {
                int dir = Random.Range(0, 1) * 2 - 1;
                int numberToActivate = Random.Range(1, randomRange) * dir;
                numberToActivate += averageNumToActivate;
                Debug.Log("Switch trigger " + numberToActivate);

                for (int i = 0; i < numberToActivate; i++)
                {
                    int indexActivated = Random.Range(0, triggers.Length);
                    Debug.Log("Triggering " + indexActivated);
                    triggers[indexActivated].Toggle(true);
                }
            }
        }
	}
}
