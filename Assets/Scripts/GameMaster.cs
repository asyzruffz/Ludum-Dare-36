using System.Collections.Generic;
using UnityEngine;

public static class GameMaster {
	public static GameManager GManager = null;
	public static int winPlayer;
	public static List<NetworkPlayerCharacter> sPlayers = new List<NetworkPlayerCharacter> ();

	private static bool isOver;

	public static void SetGameEndStatus (bool end) {
		isOver = end;
	}

	public static bool IsGameOver () {
		return isOver;
	}

	public static int CheckWinningPlayer () {
		return winPlayer;
	}
}
