using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardState
{
    //Size - in tiles.If size = 4 than board is 5 x 5 points
    public int size;
    public int passCounter;
    public int whiteDeathCounter;
    public int blackDeathCounter;
    public int handicapCounter;
    public bool isWhiteTurn;
    public List<FigureData> figuresOnBoardData;
    public List<FigureData> previousWhiteTurnFigures;
    public List<FigureData> previousBlackTurnFigures;

}
