using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwitcher : MonoBehaviour
{
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas boardSizeMenu;

    public void ChooseSizeUi()
    {
        mainMenu.enabled = false;
        boardSizeMenu.enabled = true;
    }
    public void HideAllUi()
    {
        mainMenu.enabled = false;
        boardSizeMenu.enabled = false;
    }
}
