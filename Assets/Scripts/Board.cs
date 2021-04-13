using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private BoardState initialState;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private UISwitcher uiSwitcher;
    private const float cellStartCoordinate = 0.5f;
    private const float borderOffset = 0.01f;
    private const float highOfTheCamera = 7.23f;
    public void NewGame(int borderSize)
    {
        Camera.main.orthographicSize = borderSize;
        float centerOfTheBoard = (float)borderSize / 2;
        Camera.main.transform.position = new Vector3(centerOfTheBoard, highOfTheCamera, centerOfTheBoard);

        border.SetActive(true);
        initialState.size = borderSize;
        border.transform.position = new Vector3(centerOfTheBoard, transform.position.y, centerOfTheBoard);
        for (int i = 0; i < initialState.size; i++)
        {
            for (int j = 0; j < initialState.size; j++)
            {
                Instantiate(cellPrefab, new Vector3(cellStartCoordinate+i, 1, cellStartCoordinate+j), Quaternion.identity);
            }
        }
        border.transform.localScale = new Vector3(initialState.size * border.transform.localScale.x + borderOffset, 1, initialState.size * border.transform.localScale.z + borderOffset);
        uiSwitcher.HideAllUi();
    }
}
