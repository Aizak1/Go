using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardState
{
    public int size;
    public int passCounter;
    public int whiteDeathCounter;
    public int blackDeathCounter;
    public bool isWhiteTurn;
    public List<FigureData> figuresOnBoardData;
    public List<FigureData> previousWhiteTurnFigures;
    public List<FigureData> previousBlackTurnFigures;

}
