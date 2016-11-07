using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Chess : MonoBehaviour, IPointerClickHandler
{
	public void OnPointerClick (PointerEventData data)
	{
		if (Main_GUI._Main_GUI._Turn.text == "輪到你" && this.GetComponent<Button>().interactable == true)
		{
			this.GetComponent<Button>().interactable = false;
			this.GetComponent<Image>().sprite = Main_GUI._Main_GUI._Player;
			Main_GUI._Main_GUI._Point.Remove(this.GetComponent<Button>()); /* 先移除掉_Point裡的該格，才能判斷對手還有沒有空格可以下 */

			Main_GUI._Main_GUI.Judge (Main_GUI._Main_GUI._Player, this.name.Split(','));
		}
	}
}