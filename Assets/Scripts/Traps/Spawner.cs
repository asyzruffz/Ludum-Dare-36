using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : NetworkBehaviour {

	public GameObject objectToSpawn;
	public float startDelay = 0f;
	[MinMax]
	public Vector2 rate = Vector2.one;

	void Start () {
		if (isServer) {
			StartCoroutine (SpawnCoroutine ());
		}
	}
	
	IEnumerator SpawnCoroutine () {
		yield return new WaitForSeconds (startDelay);

		Vector3 pos = new Vector3 (transform.position.x, transform.position.y, 0);
		while (true) {
			GameObject spawnedObj = Instantiate (objectToSpawn, pos, Quaternion.identity) as GameObject;
			NetworkServer.Spawn (spawnedObj);
			
			yield return new WaitForSeconds (Random.Range (rate.x, rate.y));
		}
	}
}
