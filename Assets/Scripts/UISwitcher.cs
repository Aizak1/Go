using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwitcher : MonoBehaviour
{
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas boardSizeMenu;
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas gameMenu;
    [SerializeField] private Board  board;

    public Canvas MainMenu { get => mainMenu; set => mainMenu = value; }
    public Canvas BoardSizeMenu { get => boardSizeMenu; set => boardSizeMenu = value; }
    public Canvas PauseMenu { get => pauseMenu; set => pauseMenu = value; }
    public Canvas GameMenu { get => gameMenu; set => gameMenu = value; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!mainMenu.enabled && !boardSizeMenu.enabled)
            {
                pauseMenu.enabled = !pauseMenu.enabled;
                board.enabled = !board.enabled;
            }   
        }
    }

    public void ChooseConrectUi(Canvas concrectCanvas)
    {
        HideAllUi();
        concrectCanvas.enabled = true;
    }

    public void HideAllUi()
    {
        mainMenu.enabled = false;
        boardSizeMenu.enabled = false;
        pauseMenu.enabled = false;
        gameMenu.enabled = false;
    }
}
