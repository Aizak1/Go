using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardLogic
{
    public static bool IsAbleToMove(BoardState boardState,Vector2Int finalPosition)
    {
        FigureData[,] board = new FigureData[boardState.size+1, boardState.size+1];
        foreach (var figureData in boardState.figuresOnBoardData)
        {
            board[figureData.x, figureData.y] = figureData;
        }
        if (board[finalPosition.x, finalPosition.y] != null)
            return false;
        return true;
    }
}
