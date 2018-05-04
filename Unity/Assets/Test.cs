using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

	public GameObject card;
	public Vector2 LicensingPos;
	public Vector2[] posArray = new Vector2[10];
	
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}


	private void CreareCard()
	{
		for (int i = 0; i < 5; i++)
		{
			UnityEngine.Object.Instantiate(card);
			
		}
	}

	//发牌动画
	private void Licensing(Vector2 targetPos,float spacing,List<GameObject> cardList)
	{
		if (cardList.Count != 0)
		{
			cardList[0].GetComponent<RectTransform>().anchoredPosition=Vector2.Lerp(LicensingPos,targetPos,0.5f);
			if (cardList[0].GetComponent<RectTransform>().anchoredPosition.x - targetPos.x <= 0.01)
			{
				cardList[0].GetComponent<RectTransform>().anchoredPosition = targetPos;
				targetPos.x += spacing;
				cardList.RemoveAt(0);
			}
		}
	}
}
