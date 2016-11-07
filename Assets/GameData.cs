using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData
{
	public int _PlayerChessCloor; // 玩家的棋子顏色 - 1:黑棋, 2:白棋
	public int _AIChessCloor; // 電腦的棋子顏色

	public int _AllChessCount; // 棋子總數量
	public List<Unit> _AllChessData = new List<Unit>(); // 棋局的每顆棋子狀態
}

public class Unit
{
	public string _Coordinate; // 棋子的位置座標
	public string _Own; // 這顆棋屬於誰
}