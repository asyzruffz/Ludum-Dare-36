using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour {

    public GameObject[] spawnPrefabs;

	void Awake () {
		GameMaster.GManager = this;
	}

	void Start () {
        GameMaster.SetGameEndStatus(false);
    }

	[ServerCallback]
	void Update ()
    {
        if(GameMaster.IsGameOver()) {
			StartCoroutine (ReturnToLoby ());
		} else {
			/*if (player2 == null && player1 != null)
            {
                Debug.Log("Player 1 win!");
                GameMaster.winPlayer = 1;
                GameMaster.SetGameEndStatus(true);
            }
            else if (player1 == null && player2 != null)
            {
                Debug.Log("Player 2 win!");
                GameMaster.winPlayer = 2;
                GameMaster.SetGameEndStatus(true);
            }*/
		}
	}

	public override void OnStartClient () {
		base.OnStartClient ();

		foreach (GameObject obj in spawnPrefabs) {
			ClientScene.RegisterPrefab (obj);
		}
	}

	IEnumerator ReturnToLoby () {
		GameMaster.SetGameEndStatus (true);
		yield return new WaitForSeconds (3.0f);
		LobbyManager.s_Singleton.ServerReturnToLobby ();
	}

	public IEnumerator WaitForRespawn (NetworkPlayerCharacter player) {
		yield return new WaitForSeconds (4.0f);

		player.Respawn ();
	}
}
