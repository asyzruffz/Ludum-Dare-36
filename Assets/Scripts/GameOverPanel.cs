using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverPanel : MonoBehaviour {

    public Sprite p1Win;
    public Sprite p2Win;

    private Image image;
    
    void Start() {
        image = GetComponent<Image>();
    }

	void Update () {
	    if(GameMaster.checkWinningPlayer() == 1)
        {
            image.sprite = p1Win;
        }
        else
        {
            image.sprite = p2Win;
        }
	}
}
