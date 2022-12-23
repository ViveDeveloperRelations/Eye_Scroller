using GamesTan.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlCanvas : MonoBehaviour
{
    [SerializeField] private Button movieBrowserButton;
    [SerializeField] private Button teleprompterButton;
    [SerializeField] private Button emptyButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private SuperScrollRect startingContent;
    //private GameObject activeContent;
    private SuperScrollRect activeSuperScrollRect;

    private void Awake()
    {
        quitButton.onClick.AddListener(ExitGame);

        ChangeGrid(startingContent);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeGrid(SuperScrollRect grid)
    {
        activeSuperScrollRect?.content.gameObject.SetActive(false);
        grid.content.gameObject.SetActive(true);
        activeSuperScrollRect = grid;
    }

    public SuperScrollRect GetActiveSuperScrollRect()
    {
        return activeSuperScrollRect;
    }

    public Button GetMovieBrowserButton()
    {
        return movieBrowserButton;
    }

    public Button GetTeleprompterButton()
    {
        return teleprompterButton;
    }

    public Button GetQuitButton()
    {
        return quitButton;
    }
}
