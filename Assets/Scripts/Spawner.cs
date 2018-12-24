using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public GameObject objectToSpawn;
    public float startDelay = 0f;
    public float rate = 1f;

    void Start () {
        Invoke("Spawn", startDelay);
    }

    void Spawn()
    {
        GameObject obj = (GameObject)Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        obj.transform.parent = transform;
        Invoke("Spawn", rate);
    }
}
