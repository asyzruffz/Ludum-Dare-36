using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public Transform player1;
    public Transform player2;

    void Start () {
        GameMaster.checkGameStatus(false);
    }
	
	void Update ()
    {
        if(!GameMaster.isGameOver())
        {
            if (player2 == null && player1 != null)
            {
                Debug.Log("Player 1 win!");
                GameMaster.winPlayer = 1;
                GameMaster.checkGameStatus(true);
            }
            else if (player1 == null && player2 != null)
            {
                Debug.Log("Player 2 win!");
                GameMaster.winPlayer = 2;
                GameMaster.checkGameStatus(true);
            }
        }
    }
}

public static class GameMaster
{
    public static int winPlayer;
    private static bool isOver;

    public static void checkGameStatus(bool end)
    {
        isOver = end;
    }

    public static bool isGameOver()
    {
        return isOver;
    }

    public static int checkWinningPlayer()
    {
        return winPlayer;
    }
}
