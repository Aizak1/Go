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
    [SerializeField] private GameObject whiteFigurePrefab;
    [SerializeField] private GameObject blackFigurePrefab;
    private const float cellStartCoordinate = 0.5f;
    private const float borderOffset = 0.01f;
    private const float highOfTheCamera = 7.23f;
    private readonly Vector3 borderInitialScale = new Vector3(0.1f, 1, 0.1f);
    private readonly Vector2 cellOffset = new Vector2(0.5f, 0.5f);
    private void Start()
    {
        enabled = false;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mouseDownPosition = RecordMousePosition();
            if (BoardLogic.IsAbleToMove(currentState, mouseDownPosition))
            {
                var figureData =
                CreateFigureData(mouseDownPosition.x, mouseDownPosition.y, ref currentState);
                GenerateFigure(figureData);
                currentState.isWhiteTurn = !currentState.isWhiteTurn;
            }
        }
    }

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
    public void SaveBoardState(string path,BoardState boardState)
    {
        string json = Serialization(boardState);
        SaveToJsonFile(path, json);
    }
    public BoardState LoadBoardState(string path)
    {
        string json = LoadFromJsonFile(path);
        BoardState boardState = Deserialization(json);
        return boardState;
    }

    public void SaveCurrentState(string path)
    {
        SaveBoardState(path, currentState);
    }

    public void LoadCurrentState(string path)
    {
        enabled = true;
        currentState.figuresOnBoardData = new List<FigureData>();
        DestroyBoard();
        currentState = LoadBoardState(path);
        CreateBoard(currentState);
    }

    public void NewGame(int boardSize)
    {
        enabled = true;
        currentState.size = boardSize;
        currentState.blackDeathCounter = 0;
        currentState.whiteDeathCounter = 0;
        currentState.isWhiteTurn = true;
        currentState.figuresOnBoardData = new List<FigureData>();
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
        int countOfElements = boardState.figuresOnBoardData.Count;
        for (int i = 0; i < countOfElements; i++)
        {
            FigureData figureData = boardState.figuresOnBoardData[i];
            GenerateFigure(figureData);
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
        var figures = FindObjectsOfType<Figure>();
        foreach (var figure in figures)
        {
            Destroy(figure.gameObject);
        }
        currentState.figuresOnBoardData.Clear();
        border.transform.localScale = borderInitialScale;
        border.SetActive(false);
    }
    public void Menu()
    {
        DestroyBoard();
        enabled = false;
        uiSwitcher.ChooseConrectUi(uiSwitcher.MainMenu);
    }
    public void GenerateFigure(FigureData figureData)
    {
        Vector3 figurePos = new Vector3(figureData.x, 1, figureData.y);
        GameObject figureGameObject;
        if (figureData.isWhite)
           figureGameObject = Instantiate(whiteFigurePrefab, figurePos, Quaternion.identity);
        else
           figureGameObject = Instantiate(blackFigurePrefab, figurePos, Quaternion.identity);
        figureGameObject.GetComponent<Figure>().Data = figureData;
    }
    public FigureData CreateFigureData(int x,int y, ref BoardState boardState)
    {
        FigureData data = new FigureData
        {
            x = x,
            y = y,
            isWhite = boardState.isWhiteTurn
        };
        boardState.figuresOnBoardData.Add(data);
        return data;
    }

    public Vector2Int RecordMousePosition()
    {
        Vector2Int mouseDownPosition = Vector2Int.zero - Vector2Int.one;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mouseDownPosition = new Vector2Int((int)(hit.point.x + cellOffset.x),
                                               (int)(hit.point.z + cellOffset.y));
        }
        return mouseDownPosition;
    }

   
}
