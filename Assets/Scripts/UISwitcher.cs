using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISwitcher : MonoBehaviour
{
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas boardSizeMenu;
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas gameMenu;
    [SerializeField] private Board  board;
    [SerializeField] private Canvas winMenu;
    [SerializeField] private TextMeshProUGUI winText;
    public Canvas MainMenu { get => mainMenu; set => mainMenu = value; }
    public Canvas BoardSizeMenu { get => boardSizeMenu; set => boardSizeMenu = value; }
    public Canvas PauseMenu { get => pauseMenu; set => pauseMenu = value; }
    public Canvas GameMenu { get => gameMenu; set => gameMenu = value; }
    public Canvas WinMenu { get => winMenu; set => winMenu = value; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!mainMenu.enabled && !boardSizeMenu.enabled)
            {
                pauseMenu.enabled = !pauseMenu.enabled;
                gameMenu.enabled = !gameMenu.enabled;
                board.enabled = !board.enabled;
            }   
        }
    }

    public void ChooseConrectUi(Canvas concrectCanvas)
    {
        HideAllUi();
        concrectCanvas.enabled = true;
    }
    public void SetWinText(GameResult gameResult)
    {
        winText.text = gameResult.ToString();
    }

    public void HideAllUi()
    {
        var canvases = FindObjectsOfType<Canvas>();
        foreach (var item in canvases)
        {
            item.enabled = false;
        }
    }
}
