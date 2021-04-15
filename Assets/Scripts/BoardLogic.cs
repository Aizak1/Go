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
        if (IsOutOfBounds(boardState, finalPosition))
            return false;

        if (board[finalPosition.x, finalPosition.y] != null)
            return false;

        List<Vector2Int> cellsAround = new List<Vector2Int>() 
        {
            new Vector2Int(finalPosition.x, finalPosition.y + 1),
            new Vector2Int(finalPosition.x, finalPosition.y - 1),
            new Vector2Int(finalPosition.x + 1, finalPosition.y),
            new Vector2Int(finalPosition.x - 1, finalPosition.y)
        };
        foreach (var cell in cellsAround)
        {
            if (IsOutOfBounds(boardState, cell))
                continue;
            if (board[cell.x, cell.y] == null
            || board[cell.x, cell.y].isWhite == boardState.isWhiteTurn)
                return true;

        }
        return false;
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
}
