using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardLogic
{
    private const float komi = 6.5f;
    public static bool IsAbleToMove(BoardState boardState,Vector2Int finalPosition)
                                                          
    {
        FigureData[,] board = new FigureData[boardState.size+1, boardState.size+1];
        foreach (var figureData in boardState.figuresOnBoardData)
        {
            board[figureData.x, figureData.y] = figureData;
        }
        if (IsOutOfBounds(boardState, finalPosition))
        {
            return false;
        }
            
        if (board[finalPosition.x, finalPosition.y] != null)
        {
            return false;
        }
        

        return true;
    }

    public static bool IsRepeatThePosition(bool isWhiteTurn,List<FigureData> figuresOnBoardData,
                  List<FigureData> blackPrevioutTurnData,List<FigureData> whitePrevioutTurnData,
                  List<FigureData> figuresDataToDestroy)
    {
        List<FigureData> simulatedFiguresOnBoard = new List<FigureData>();
        foreach (var item in figuresOnBoardData)
        {
            if (!figuresDataToDestroy.Contains(item))
            {
                simulatedFiguresOnBoard.Add(item);
            }
        }
        if (blackPrevioutTurnData.Count>0 && whitePrevioutTurnData.Count>0)
        {
            if (isWhiteTurn)
                return IsPreviousTeamRepeatTheTurn(simulatedFiguresOnBoard, whitePrevioutTurnData);
            else
                return IsPreviousTeamRepeatTheTurn(simulatedFiguresOnBoard, blackPrevioutTurnData);
        }
        return false;
    }

    private static bool IsPreviousTeamRepeatTheTurn(List<FigureData> simulatedFiguresOnBoard,
                                                    List<FigureData> colorFiguresTurnData)
    {
        bool isTheSame = true;
        if (simulatedFiguresOnBoard.Count != colorFiguresTurnData.Count)
            return false;
        for (int i = 0; i < simulatedFiguresOnBoard.Count; i++)
        {
            var item = simulatedFiguresOnBoard[i];
            var historyItem = colorFiguresTurnData[i];
            if (item.isWhite != historyItem.isWhite
                || item.x != historyItem.x || item.y != historyItem.y)
            {
                isTheSame = false;
            }
        }
        if (isTheSame)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool IsOutOfBounds(BoardState boardState, Vector2Int position)
    {
        if (position.x < 0 || position.y < 0 || position.x > boardState.size
        || position.y > boardState.size)
        {
            return true;
        }
        return false;
    }

    public static List<FigureData> FindFiguresDataToRemove(BoardState boardState)
    {
        List<FigureData> figuresDataToRemove = new List<FigureData>();
        FigureData[,] board = new FigureData[boardState.size + 1, boardState.size + 1];
        foreach (var figureData in boardState.figuresOnBoardData)
        {
            board[figureData.x, figureData.y] = figureData;
        }
        List<List<FigureData>> groups = FindGroups(boardState, board);
        foreach (var group in groups)
        {
            if (!GroupWillBeAlive(boardState, group, board))
            {
                figuresDataToRemove.AddRange(group);
            }   
        }

        return figuresDataToRemove;

    }

    private static bool GroupWillBeAlive(BoardState boardState,
                                    List<FigureData> group,FigureData[,] board)
    {
        foreach (var figure in group)
        {
            Vector2Int figurePos = new Vector2Int(figure.x, figure.y);
            var aroundCells = GetAroundCellsCoords(figurePos);
            foreach (var cell in aroundCells)
            {
                if (IsOutOfBounds(boardState, cell))
                {
                    continue;
                }
                if (board[cell.x, cell.y] == null)
                {
                    return true;
                }
                   
            }
        }
        return false;
    }

    private static List<List<FigureData>> FindGroups(BoardState boardState,FigureData[,] board)
    {
        List<List<FigureData>> groups = new List<List<FigureData>>();
        List<FigureData> figuresDataCopy = new List<FigureData>(boardState.figuresOnBoardData);
        for (int i = 0; i < figuresDataCopy.Count; i++)
        {
           
            List<FigureData> group = new List<FigureData>();
            var item = figuresDataCopy[i];
            CreateGroup(boardState, board, new Vector2Int(item.x, item.y), ref group);
            groups.Add(group);
            foreach (var figure in group)
            {
                figuresDataCopy.Remove(figure);
            }
            //To start The Cycle From the beginning
            i = -1;
        }
        
        return groups;
    }

    private static void CreateGroup(BoardState boardState,FigureData[,] board,
                                    Vector2Int currentPosition,ref List<FigureData> group)
    {
        List<Vector2Int> aroundCells = GetAroundCellsCoords(currentPosition);
        bool isSingle = true;
        foreach (var cell in aroundCells)
        {
            if (IsOutOfBounds(boardState, cell))
            {
                continue;
            }   
            if (board[cell.x, cell.y] != null
            && board[cell.x, cell.y].isWhite == board[currentPosition.x, currentPosition.y].isWhite)
            {
                isSingle = false;
                break;
            }
        }
        if (isSingle)
        {
            group.Add(board[currentPosition.x, currentPosition.y]);
            return;
        }
        foreach (var cell in aroundCells)
        {
            if (IsOutOfBounds(boardState, cell))
            {
                continue;
            }
            if(board[cell.x, cell.y]!=null && 
               board[cell.x,cell.y].isWhite == board[currentPosition.x, currentPosition.y].isWhite)
            {
                if (!group.Contains(board[currentPosition.x, currentPosition.y]))
                {
                    group.Add(board[currentPosition.x, currentPosition.y]);
                }
                if (!group.Contains(board[cell.x, cell.y]))
                {
                    CreateGroup(boardState, board, cell, ref group);
                }
                
            }
           
        }
        
    }

    private static List<Vector2Int> GetAroundCellsCoords(Vector2Int finalPosition)
    {
        List<Vector2Int> cellsAround = new List<Vector2Int>()
        {
            new Vector2Int(finalPosition.x, finalPosition.y + 1),
            new Vector2Int(finalPosition.x, finalPosition.y - 1),
            new Vector2Int(finalPosition.x + 1, finalPosition.y),
            new Vector2Int(finalPosition.x - 1, finalPosition.y)
        };
        return cellsAround;
    }

    public static BoardState SimulateGeneration(BoardState currentBoardState,int x,int y)
    {
        BoardState boardStateCopy;
        boardStateCopy.figuresOnBoardData = new List<FigureData>();
        boardStateCopy.figuresOnBoardData.AddRange(currentBoardState.figuresOnBoardData);
        boardStateCopy.size = currentBoardState.size;
        boardStateCopy.isWhiteTurn = currentBoardState.isWhiteTurn;
        boardStateCopy.blackDeathCounter = currentBoardState.blackDeathCounter;
        boardStateCopy.whiteDeathCounter = currentBoardState.whiteDeathCounter;
        boardStateCopy.passCounter = currentBoardState.passCounter;
        boardStateCopy.previousWhiteTurnFigures = currentBoardState.previousWhiteTurnFigures;
        boardStateCopy.previousBlackTurnFigures = currentBoardState.previousBlackTurnFigures;
        FigureData data = new FigureData
        {
            x = x,
            y = y,
            isWhite = currentBoardState.isWhiteTurn
        };
        boardStateCopy.figuresOnBoardData.Add(data);
        return boardStateCopy;
    }
    public static int CalculateScoreDifference(BoardState boardState)
    {
        int blackScore = 0;
        int whiteScore = 0;
        FigureData[,] board = new FigureData[boardState.size + 1, boardState.size + 1];
        List<FigureData> allDirectionElements = new List<FigureData>();
        List<Vector2Int> allDirections = new List<Vector2Int>()
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.right,
            Vector2Int.left,
            new Vector2Int(1,1),
            new Vector2Int(1,-1),
            new Vector2Int(-1,1),
            new Vector2Int(-1,-1)
        };

       
        foreach (var figureData in boardState.figuresOnBoardData)
        {
            board[figureData.x, figureData.y] = figureData;
        }
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                if (board[y, x] != null)
                {
                    continue;
                }
                Vector2Int calculationPoint = new Vector2Int(x, y);
                foreach (var step in allDirections)
                {
                    allDirectionElements.Add(CheckPerpendicular(boardState, board, 
                                                                calculationPoint, step));
                }
                foreach (var item in allDirectionElements)
                {
                    if (item == null)
                    {
                        continue;
                    }  
                    List<FigureData> allOtherDirectionsElems = new List<FigureData>();
                    allOtherDirectionsElems.AddRange(allDirectionElements);
                    allOtherDirectionsElems.Remove(item);
                    List<FigureData> conditionalOtherDirectionsElems = new List<FigureData>();
                    foreach (var elem in allOtherDirectionsElems)
                    {
                        if(elem == null || elem.isWhite == item.isWhite)
                        {
                            conditionalOtherDirectionsElems.Add(elem);
                        }     
                    }
                    if (conditionalOtherDirectionsElems.Count == allOtherDirectionsElems.Count)
                    {
                        if(item.isWhite)
                        {
                            whiteScore++;
                        }
                        else
                        {
                            blackScore++;
                        }
                        break;
                    }
                }
                allDirectionElements.Clear();
               
            }
        }
        whiteScore += boardState.blackDeathCounter;
        blackScore += boardState.whiteDeathCounter;
        Debug.Log(whiteScore);
        Debug.Log(blackScore);
        return Mathf.Abs(whiteScore - blackScore);
    }

    private static FigureData CheckPerpendicular(BoardState boardState,FigureData[,] 
                                                board,Vector2Int calculationPoint,Vector2Int step)
    {
        while (!IsOutOfBounds(boardState,calculationPoint))
        {
            var element = board[calculationPoint.y, calculationPoint.x];
            if (element != null)
            {
                return element;
            }
            calculationPoint += step;
        }
        return null;
    }
}
