using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook 
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
		NetworkCharacterInfo character = gamePlayer.GetComponent<NetworkCharacterInfo> ();
		Health healthInfo = gamePlayer.GetComponent<Health> ();

		character.name = lobby.name;
		character.color = lobby.playerColor;
		character.score = 0;
		character.life = healthInfo.maxLives;
		character.hp = healthInfo.maxHP;
	}
}
