using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public enum GameState
{
    Started,
    Finished
}
public class Board : MonoBehaviour
{
    private BoardState currentState;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private UISwitcher uiSwitcher;
    [SerializeField] private GameObject whiteFigurePrefab;
    [SerializeField] private GameObject blackFigurePrefab;
    private const float cellStartCoordinate = 0.5f;
    private const float borderOffset = 0.08f;
    private const float cameraOffset = 3f;
    private readonly Vector3 borderInitialScale = new Vector3(0.1f, 1, 0.1f);
    private readonly Vector2 cellOffset = new Vector2(0.5f, 0.5f);
    private const string saveFilePath = "Save.json";
    private GameState gameState;
    private void Start()
    {
        enabled = false;
        currentState.figuresOnBoardData = new List<FigureData>();
        currentState.previousWhiteTurnFigures = new List<FigureData>();
        currentState.previousBlackTurnFigures = new List<FigureData>();
    }

    private void Update()
    {
        if(gameState == GameState.Finished)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            var mouseDownPosition = RecordMousePosition();
            if (!BoardLogic.IsAbleToMove(currentState, mouseDownPosition))
            {
                return;
            }
           
            var boardCopyState =
             BoardLogic.SimulateGeneration(currentState, mouseDownPosition.x, mouseDownPosition.y);
            var figuresDataToDestroy = BoardLogic.FindFiguresDataToRemove(boardCopyState);
            bool enemyGroupWillBeDestroyed = false;
            foreach (var item in figuresDataToDestroy)
            {
                if (item.isWhite == currentState.isWhiteTurn)
                {
                    foreach (var figure in figuresDataToDestroy)
                    {
                        if (figure.isWhite != currentState.isWhiteTurn)
                        {
                            enemyGroupWillBeDestroyed = true;
                            break;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                if (enemyGroupWillBeDestroyed)
                {
                    break;
                }
            }
            if (enemyGroupWillBeDestroyed)
            {
                figuresDataToDestroy.RemoveAll(x => x.isWhite == currentState.isWhiteTurn);
            }
            if (BoardLogic.IsRepeatThePosition(currentState.isWhiteTurn,
                                      boardCopyState.figuresOnBoardData, 
                                      currentState.previousBlackTurnFigures, 
                                      currentState.previousWhiteTurnFigures, figuresDataToDestroy))
            {
                return;
            }

            var figureData =
                CreateFigureData(mouseDownPosition.x, mouseDownPosition.y, ref currentState);
            GenerateFigure(figureData);
            var figures = FindObjectsOfType<Figure>();
            foreach (var item in figures)
            {
                if (figuresDataToDestroy.Contains(item.Data))
                {
                    if (item.Data.isWhite)
                    {
                        currentState.whiteDeathCounter++;
                    }
                    else
                    {
                        currentState.blackDeathCounter++;
                    }
                    DestroyFigure(item, ref currentState);
                }
            }
            if (currentState.isWhiteTurn)
            {
                currentState.previousWhiteTurnFigures = 
                    new List<FigureData>(currentState.figuresOnBoardData);
            }
            else
            {
                currentState.previousBlackTurnFigures = 
                    new List<FigureData>(currentState.figuresOnBoardData);
            }
            //Variant of the rules,when after handicap still black Turn
            if (currentState.handicapCounter != 0)
            {
                currentState.handicapCounter--;
            }
            else
            {
                currentState.isWhiteTurn = !currentState.isWhiteTurn;
            }
            currentState.passCounter = 0;
        }
        if (currentState.passCounter > 1)
        {
            gameState = GameState.Finished;
            var scoreDifference = BoardLogic.CalculateScoreDifference(currentState);
        }
    }

    private string Serialization(BoardState boardState)
    {
        string json = JsonUtility.ToJson(boardState);
        return json;
    }

    private BoardState Deserialization(string json)
    {
        var boardState = JsonUtility.FromJson<BoardState>(json);
        return boardState;
    }

    private void SaveToJsonFile(string path,string json)
    {
        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.Write(json);
        }
    }

    private string LoadFromJsonFile(string path)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            string json = reader.ReadToEnd();
            return json;
        }
    }

    private void SaveBoardState(string path,BoardState boardState)
    {
        string json = Serialization(boardState);
        SaveToJsonFile(path, json);
    }

    private BoardState LoadBoardState(string path)
    {
        string json = LoadFromJsonFile(path);
        BoardState boardState = Deserialization(json);
        return boardState;
    }

    public void SaveCurrentState()
    {
        string path  = GetStreamingAssetsPath(saveFilePath);
        SaveBoardState(path, currentState);
       
    }

    public void LoadCurrentState()
    {
        string path = GetStreamingAssetsPath(saveFilePath);
        enabled = true;
        gameState = GameState.Started;
        uiSwitcher.ChooseConrectUi(uiSwitcher.GameMenu);
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
        currentState.passCounter = 0;
        currentState.isWhiteTurn = false;
        gameState = GameState.Started;
        uiSwitcher.ChooseConrectUi(uiSwitcher.GameMenu);
        CreateBoard(currentState);
    }
    public void SetHandicapSize(int handicap)
    {
        currentState.handicapCounter = handicap;
    }
    private string GetStreamingAssetsPath(string path)
    {
        return Path.Combine(Application.streamingAssetsPath, path);
    }

    private void CreateBoard(BoardState boardState)
    {
        float hightofTheCamera = boardState.size + cameraOffset;
        float centerOfTheBoard = (float)boardState.size / 2;
        Vector3 cameraRightPos = new Vector3(centerOfTheBoard, 
                                    hightofTheCamera, centerOfTheBoard);
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
        currentState.previousBlackTurnFigures.Clear();
        currentState.previousWhiteTurnFigures.Clear();
        currentState.handicapCounter = 0;
    }
    public void Pass()
    {
        currentState.isWhiteTurn = !currentState.isWhiteTurn;
        currentState.passCounter++;
    }
    public void Menu()
    {
        DestroyBoard();
        enabled = false;
        uiSwitcher.ChooseConrectUi(uiSwitcher.MainMenu);
    }
    private void GenerateFigure(FigureData figureData)
    {
        Vector3 figurePos = new Vector3(figureData.x, 1, figureData.y);
        GameObject figureGameObject;
        if (figureData.isWhite)
        {
            figureGameObject = 
                Instantiate(whiteFigurePrefab, figurePos, Quaternion.identity, transform);
        }
        else
        {
           figureGameObject = 
                Instantiate(blackFigurePrefab, figurePos, Quaternion.identity,transform);
        }
           
        figureGameObject.GetComponent<Figure>().Data = figureData;
    }

    private FigureData CreateFigureData(int x,int y, ref BoardState boardState)
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

    private Vector2Int RecordMousePosition()
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
    private void DestroyFigure(Figure figure, ref BoardState boardState)
    {
        boardState.figuresOnBoardData.Remove(figure.Data);
        figure.Data = null;
        Destroy(figure.gameObject);
    }


}
