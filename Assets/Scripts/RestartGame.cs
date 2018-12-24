using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RestartGame : MonoBehaviour {

    private ShowPanels showPanels;                      //Reference to the ShowPanels script used to hide and show UI panels
    private StartOptions startScript;

    void Awake()
    {
        //Get a component reference to ShowPanels attached to this object, store in showPanels variable
        showPanels = GetComponent<ShowPanels>();
        //Get a component reference to StartButton attached to this object, store in startScript variable
        startScript = GetComponent<StartOptions>();
    }
    
	void Update () {
        if (GameMaster.isGameOver() && !startScript.inMainMenu)
        {
            //Call the DoPause function to pause the game
            DoPause();
        }

        if (Input.GetButtonDown("Cancel") && !startScript.inMainMenu)
        {
            UnPause();
            SceneManager.LoadScene(0);
            Destroy(gameObject);
        }
    }

    public void DoPause()
    {
        //Set time.timescale to 0, this will cause animations and physics to stop updating
        Time.timeScale = 0;
        //call the ShowPausePanel function of the ShowPanels script
        showPanels.ShowGameOverPanel();
    }

    public void UnPause()
    {
        //Set time.timescale to 1, this will cause animations and physics to continue updating at regular speed
        Time.timeScale = 1;
        //call the HidePausePanel function of the ShowPanels script
        showPanels.HideGameOverPanel();
    }

    public void DoRestart ()
    {
        //Set time.timescale to 1, this will cause animations and physics to continue updating at regular speed
        Time.timeScale = 1;
        GameMaster.checkGameStatus(false);
        //call the HidePausePanel function of the ShowPanels script
        showPanels.HideGameOverPanel();
        SceneManager.LoadScene("Arena");
    }
}
