using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Random = UnityEngine.Random;

public class Main_GUI : MonoBehaviour
{
	public static Main_GUI _Main_GUI;

	public Text _Turn;

	[Header("Achievement 成績")]
	public Text _Win;
	public Text _Tie;
	public Text _Lose;

	[Header("Chess 棋子相關")]
	public Sprite[] _Dot; // 0:原始透明, 1:黑棋, 2:白棋
	public Sprite _Player;
	public Sprite _AI;

	[Space(10)]
	public List<Button> _Point_Origin = new List<Button>(); // 棋盤格全按鈕，ReStart時，還原會用到

	[Space(10)]
	public List<Button> _Point = new List<Button>(); // 計算用，每下一手-1
	private List<Button> _PointPos = new List<Button>(); // AI可能會下的位置

	void Start ()
	{
		_Main_GUI = this;

		GameObject[] _Grid = GameObject.FindGameObjectsWithTag("Grid"); // 將棋盤格讀入 _Grid[] 陣列裡

		for (int i = 0; i < _Grid.Length; i++)
		{
			_Point_Origin.Add(_Grid[i].GetComponent<Button>());
			_Point.Add(_Grid[i].GetComponent<Button>());
		}

		_Win.text = (PlayerPrefs.GetInt("Win")).ToString();
		_Tie.text = (PlayerPrefs.GetInt("Tie")).ToString();
		_Lose.text = (PlayerPrefs.GetInt("Lose")).ToString();
	}

	public IEnumerator AI (int x, int y)
	{
		yield return new WaitForSeconds(0.5f);

		if (x == 0 && y == 0) // 第一手:天元
		{
			GameObject.Find("8,8").GetComponent<Button>().interactable = false;
			GameObject.Find("8,8").GetComponent<Image>().sprite = _AI;
			_Point.Remove(GameObject.Find("8,8").GetComponent<Button>());

			_Turn.text = "輪到你";
		}
		else
		{
			/* 檢查8方位的點是否存在 */
			CheckPoint (x, y + 1); // ↑
			CheckPoint (x + 1, y + 1); // ↗
			CheckPoint (x + 1, y); // -→
			CheckPoint (x + 1, y - 1); // ↘
			CheckPoint (x, y - 1); // ↓
			CheckPoint (x - 1, y - 1); // ↙
			CheckPoint (x - 1, y); // ←-
			CheckPoint (x - 1, y + 1); // ↖

			string pos = "";

			if (_PointPos.Count != 0) // 玩家所下的棋子位置，周圍選一點下
			{
				int i = Random.Range(0, _PointPos.Count);

				_PointPos[i].interactable = false;
				_PointPos[i].GetComponent<Image>().sprite = _AI;
				pos = _PointPos[i].name;
				_Point.Remove(_PointPos[i]); /* 先移除掉_Point裡的該格，才能判斷對手還有沒有空格可以下 */
				_PointPos.Clear(); /* 處理完下子後，要Clear List！以便下輪再次利用 */
			}
			else // 玩家所下的棋子位置，周圍已無空位，就隨機下一子
			{
				int i = Random.Range(0, _Point.Count);

				_Point[i].interactable = false;
				_Point[i].GetComponent<Image>().sprite = _AI;
				pos = _Point[i].name;
				_Point.Remove(_Point[i]);
			}

			Judge (_AI, pos.Split(','));
		}
	}

	private void CheckPoint (int x, int y) // 檢查「點」是否存在
	{
		GameObject temp_PointPos = GameObject.Find(string.Format("{0},{1}", x, y));

		if (temp_PointPos != null)
		{
			if (temp_PointPos.GetComponent<Image>().sprite == _Dot[0])
			{
				_PointPos.Add(temp_PointPos.GetComponent<Button>());
			}
		}
	}

	public void Judge (Sprite symbol, string[] pos)
	{
		int x = int.Parse(pos[0]);
		int y = int.Parse(pos[1]);

		if (A(symbol, x, y) + E (symbol, x, y) >= 4 || B(symbol, x, y) + F(symbol, x, y) >= 4 || C(symbol, x, y) + G(symbol, x, y) >= 4 || D(symbol, x, y) + H(symbol, x, y) >= 4)
		{
			if (symbol == _Player)
			{
				WinLose ("O");
			}
			else if (symbol == _AI)
			{
				WinLose ("X");
			}
		}
		else if (_Point.Count == 0) // 全棋盤都下完，依然不分勝負
		{
			WinLose ("△");
		}
		else if (symbol == _Player)
		{
			_Turn.text = "輪到電腦";

			StartCoroutine (AI (x, y));
		}
		else if (symbol == _AI)
		{
			_Turn.text = "輪到你";
		}
	}

	#region 8方位計算-連鎖棋子數
	private int A (Sprite symbol, int x, int y) // ↑
	{
		int sum = 0;

		for (int i = 1; i < 5; i++)
		{
			if (GameObject.Find(string.Format("{0},{1}", x, y + i)) != null)
			{
				if (GameObject.Find(string.Format("{0},{1}", x, y + i)).GetComponent<Image>().sprite == symbol)
				{
					sum = sum + 1;
				}
				else
				{
					return sum;
				}
			}
			else
			{
				return sum;
			}
		}

		return sum;
	}

	private int B (Sprite symbol, int x, int y) // ↗
	{
		int sum = 0;

		for (int i = 1; i < 5; i++)
		{
			if (GameObject.Find(string.Format("{0},{1}", x + i, y + i)) != null)
			{
				if (GameObject.Find(string.Format("{0},{1}", x + i, y + i)).GetComponent<Image>().sprite == symbol)
				{
					sum = sum + 1;
				}
				else
				{
					return sum;
				}
			}
			else
			{
				return sum;
			}
		}

		return sum;
	}

	private int C (Sprite symbol, int x, int y) // -→
	{
		int sum = 0;

		for (int i = 1; i < 5; i++)
		{
			if (GameObject.Find(string.Format("{0},{1}", x + i, y)) != null)
			{
				if (GameObject.Find(string.Format("{0},{1}", x + i, y)).GetComponent<Image>().sprite == symbol)
				{
					sum = sum + 1;
				}
				else
				{
					return sum;
				}
			}
			else
			{
				return sum;
			}
		}

		return sum;
	}

	private int D (Sprite symbol, int x, int y) // ↘
	{
		int sum = 0;

		for (int i = 1; i < 5; i++)
		{
			if (GameObject.Find(string.Format("{0},{1}", x + i, y - i)) != null)
			{
				if (GameObject.Find(string.Format("{0},{1}", x + i, y - i)).GetComponent<Image>().sprite == symbol)
				{
					sum = sum + 1;
				}
				else
				{
					return sum;
				}
			}
			else
			{
				return sum;
			}
		}

		return sum;
	}

	private int E (Sprite symbol, int x, int y) // ↓
	{
		int sum = 0;

		for (int i = 1; i < 5; i++)
		{
			if (GameObject.Find(string.Format("{0},{1}", x, y - i)) != null)
			{
				if (GameObject.Find(string.Format("{0},{1}", x, y - i)).GetComponent<Image>().sprite == symbol)
				{
					sum = sum + 1;
				}
				else
				{
					return sum;
				}
			}
			else
			{
				return sum;
			}
		}

		return sum;
	}

	private int F (Sprite symbol, int x, int y) // ↙
	{
		int sum = 0;

		for (int i = 1; i < 5; i++)
		{
			if (GameObject.Find(string.Format("{0},{1}", x - i, y - i)) != null)
			{
				if (GameObject.Find(string.Format("{0},{1}", x - i, y - i)).GetComponent<Image>().sprite == symbol)
				{
					sum = sum + 1;
				}
				else
				{
					return sum;
				}
			}
			else
			{
				return sum;
			}
		}

		return sum;
	}

	private int G (Sprite symbol, int x, int y) // ←-
	{
		int sum = 0;

		for (int i = 1; i < 5; i++)
		{
			if (GameObject.Find(string.Format("{0},{1}", x - i, y)) != null)
			{
				if (GameObject.Find(string.Format("{0},{1}", x - i, y)).GetComponent<Image>().sprite == symbol)
				{
					sum = sum + 1;
				}
				else
				{
					return sum;
				}
			}
			else
			{
				return sum;
			}
		}

		return sum;
	}

	private int H (Sprite symbol, int x, int y) // ↖
	{
		int sum = 0;

		for (int i = 1; i < 5; i++)
		{
			if (GameObject.Find(string.Format("{0},{1}", x - i, y + i)) != null)
			{
				if (GameObject.Find(string.Format("{0},{1}", x - i, y + i)).GetComponent<Image>().sprite == symbol)
				{
					sum = sum + 1;
				}
				else
				{
					return sum;
				}
			}
			else
			{
				return sum;
			}
		}

		return sum;
	}

	private void WinLose (string symbol)
	{
		if (symbol == "△") /* 平手 */
		{
			_Turn.text = "<color=#008000ff>平手</color>";
			_Tie.text = (int.Parse(_Tie.text) + 1).ToString();
			PlayerPrefs.SetInt("Tie", int.Parse(_Tie.text));
		}
		else if (symbol == "O") /* 勝利 */
		{
			_Turn.text = "<color=#ff0000ff>你贏了</color>";
			_Win.text = (int.Parse(_Win.text) + 1).ToString();
			PlayerPrefs.SetInt("Win", int.Parse(_Win.text));
		}
		else if (symbol == "X") /* 失敗 */
		{
			_Turn.text = "<color=#000080ff>你GG了</color>";
			_Lose.text = (int.Parse(_Lose.text) + 1).ToString();
			PlayerPrefs.SetInt("Lose", int.Parse(_Lose.text));
		}
	}
	#endregion

	#region UI 介面
	public void UI_WhoGoFirst (string name)
	{
		for (int i = 0; i < _Point_Origin.Count; i++)
		{
			_Point_Origin[i].interactable = true;
		}

		if (name == "Player")
		{
			_Player = _Dot[1]; // 先手為黑棋
			_AI = _Dot[2];
			_Turn.text = "輪到你";
		}
		else if (name == "AI")
		{
			_AI = _Dot[1]; // 先手為黑棋
			_Player = _Dot[2];
			_Turn.text = "輪到電腦";

			StartCoroutine (AI (0, 0));
		}
	}

	public void UI_Afresh ()
	{
		PlayerPrefs.SetInt("Win", 0);
		PlayerPrefs.SetInt("Tie", 0);
		PlayerPrefs.SetInt("Lose", 0);

		_Win.text = (PlayerPrefs.GetInt("Win")).ToString();
		_Tie.text = (PlayerPrefs.GetInt("Tie")).ToString();
		_Lose.text = (PlayerPrefs.GetInt("Lose")).ToString();
	}

	public void UI_Restart ()
	{
		_Turn.text = "";

		_Point.Clear();

		for (int i = 0; i < _Point_Origin.Count; i++)
		{
			_Point_Origin[i].interactable = false;
			_Point_Origin[i].GetComponent<Image>().sprite = _Dot[0];
			_Point.Add(_Point_Origin[i]);
		}
	}

	public void UI_Save ()
	{
		GameData savaData = new GameData();

		if (_Player == _Dot[1]) // 玩家先手，是黑棋(棋子顏色 - 1:黑棋, 2:白棋)
		{
			savaData._PlayerChessCloor = 1;
			savaData._AIChessCloor = 2;
		}
		else // 玩家後手，是白棋
		{
			savaData._PlayerChessCloor = 2;
			savaData._AIChessCloor = 1;
		}

		for (int i = 0; i < _Point_Origin.Count; i++)
		{
			if (_Point_Origin[i].GetComponent<Image>().sprite != _Dot[0])
			{
				Unit chessData = new Unit();

				chessData._Coordinate = _Point_Origin[i].name;

				if (_Point_Origin[i].GetComponent<Image>().sprite == _Player)
				{
					chessData._Own = "Player";
				}
				else
				{
					chessData._Own = "AI";
				}

				savaData._AllChessData.Add(chessData);
			}
		}

		savaData._AllChessCount = savaData._AllChessData.Count;

		/* 如果資料夾不存在，則自動創建一個 */
		if (!Directory.Exists(Application.dataPath + "/Save")) 
		{
			Directory.CreateDirectory(Application.dataPath + "/Save");
		}

		JsonWriter jsonWriter = new JsonWriter();
		jsonWriter.PrettyPrint = true; // PrettyPrint 完美打印:會自動縮排對齊
		//jsonWriter.IndentValue = 4; // IndentValue 縮排值
		JsonMapper.ToJson(savaData, jsonWriter);
		File.WriteAllText(Application.dataPath + "/Save/Record.txt", jsonWriter.ToString()); // 檔案創建+寫入
	}

	public void UI_Load ()
	{
		string path = Application.dataPath + "/Save/Record.txt";

		/* 如果存檔檔案不存在，直接return掉 */
		if (!File.Exists(path))
		{
			return;
		}

		/* 確定有存檔檔案後... */
		/* 1. 先清空棋盤、_Point，回到開局狀態 */
		_Point.Clear();

		for (int i = 0; i < _Point_Origin.Count; i++)
		{
			_Point_Origin[i].interactable = true;
			_Point_Origin[i].GetComponent<Image>().sprite = _Dot[0];
			_Point.Add(_Point_Origin[i]);
		}

		/* 2. 資料讀取+匯出 */
		string json = File.ReadAllText (Application.dataPath + "/Save/Record.txt"); // 檔案讀取
		JsonData loadData = JsonMapper.ToObject (json); // 資料匯出

		_Player = _Dot[(int)loadData["_PlayerChessCloor"]];
		_AI = _Dot[(int)loadData["_AIChessCloor"]];

		for (int i = 0; i < (int)loadData["_AllChessCount"]; i++)
		{
			GameObject point = GameObject.Find((loadData["_AllChessData"][i]["_Coordinate"]).ToString());

			if ((loadData["_AllChessData"][i]["_Own"]).ToString() == "Player")
			{
				point.GetComponent<Image>().sprite = _Player;
			}
			else
			{
				point.GetComponent<Image>().sprite = _AI;
			}

			point.GetComponent<Button>().interactable = false;
			_Point.Remove(point.GetComponent<Button>());
		}

		/* 3. 因為能「存、讀檔」時，一定是玩家的回合，因此... */
		_Turn.text = "輪到你";
	}

	public void UI_Quit ()
	{
		Application.Quit();
	}
	#endregion
}