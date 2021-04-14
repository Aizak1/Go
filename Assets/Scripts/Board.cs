using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Board : MonoBehaviour
{
    private BoardState currentState;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private UISwitcher uiSwitcher;
    private const float cellStartCoordinate = 0.5f;
    private const float borderOffset = 0.01f;
    private const float highOfTheCamera = 7.23f;
    private readonly Vector3 borderInitialScale = new Vector3(0.1f, 1, 0.1f);

    public string Serialization(BoardState boardState)
    {
        string json = JsonUtility.ToJson(boardState);
        return json;
    }

    public BoardState Deserialization(string json)
    {
        var boardState = JsonUtility.FromJson<BoardState>(json);
        return boardState;
    }

    public void SaveToJsonFile(string path,string json)
    {
        path = Path.Combine(Application.streamingAssetsPath, path);
        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.Write(json);
        }
    }
    public string LoadFromJsonFile(string path)
    {
        string validpath = Path.Combine(Application.streamingAssetsPath, path);
        using (StreamReader reader = new StreamReader(validpath))
        {
            string json = reader.ReadToEnd();
            return json;
        }
    }
    public void Save(string path,BoardState boardState)
    {
        string json = Serialization(boardState);
        SaveToJsonFile(path, json);
    }
    public BoardState Load(string path)
    {
        string json = LoadFromJsonFile(path);
        BoardState boardState = Deserialization(json);
        return boardState;
    }

    public void SaveCurrentState(string path)
    {
        Save(path, currentState);
    }

    public void LoadCurrentState(string path)
    {
        DestroyBoard();
        currentState = Load(path);
        CreateBoard(currentState);
    }

    public void NewGame(int borderSize)
    {
        currentState.size = borderSize;
        currentState.blackDeathCounter = 0;
        currentState.whiteDeathCounter = 0;
        CreateBoard(currentState);
    }

    private void CreateBoard(BoardState boardState)
    {
        Camera.main.orthographicSize = boardState.size;
        float centerOfTheBoard = (float)boardState.size / 2;
        Vector3 cameraRightPos = new Vector3(centerOfTheBoard, highOfTheCamera, centerOfTheBoard);
        Camera.main.transform.position = cameraRightPos;

        border.SetActive(true);
        Vector3 borderRightPos = new Vector3(
            centerOfTheBoard, transform.position.y, centerOfTheBoard
            );
        border.transform.position = borderRightPos;
        for (int i = 0; i < boardState.size; i++)
        {
            for (int j = 0; j < boardState.size; j++)
            {
                Vector3 spawnPos = new Vector3(cellStartCoordinate + i, 1, cellStartCoordinate + j);
                Instantiate(cellPrefab, spawnPos, Quaternion.identity,transform);
            }
        }
        border.transform.localScale = new Vector3(
            boardState.size * borderInitialScale.x + borderOffset,
            1,
            boardState.size * borderInitialScale.z + borderOffset
            );
        uiSwitcher.HideAllUi();
    }

    private void DestroyBoard()
    {
        var cells = GameObject.FindGameObjectsWithTag("Cell");
        foreach (var cell in cells)
        {
            Destroy(cell.gameObject);
        }
        border.transform.localScale = borderInitialScale;
        border.SetActive(false);
    }
    public void Menu()
    {
        DestroyBoard();
        uiSwitcher.ChooseConrectUi(uiSwitcher.MainMenu);
    }
}
