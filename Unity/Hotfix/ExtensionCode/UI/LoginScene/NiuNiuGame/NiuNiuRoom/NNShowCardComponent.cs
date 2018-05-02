using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class NNShowCardComponentAwakeSystem : AwakeSystem<NNShowCardComponent>
    {
        public override void Awake(NNShowCardComponent self)
        {
            self.Awake();
        }
    }
    

    public class NNShowCardComponent:Component
    {
        #region variable
        
        public List<Vector2> sixTableList;                         //六人桌的位置
        public List<Vector2> eightTableList;                       //八人桌的位置
        public int roomPeople;                                     //房间人数
        
        private List<Vector2> currentTablePosList;                 //当前房间位置
        private Transform currentTableObj;                         //当前桌子
        private GameObject NNCardPrefab;                           //卡牌预设
        private Dictionary<string,ReferenceCollector> HeadUIDict;  //头像位置列表
        private string[] chairArray;                               //椅子管理数组
        private Dictionary<int,UI> cardUIDict;                     //卡牌位置列表
        private RectTransform mainHeadPos;                         //主头像位置
        private RectTransform mainCardPos;                         //主卡牌像位置
        private Vector2 LicensingPos;                              //发牌位置 
        private ReferenceCollector rc;
        private Dictionary<short,List<UI>> NiuNiuCardDict;         //游戏中生产的卡牌
        private GameObject headUIform;
     

        #endregion
 
        public async void Awake()
        {
            sixTableList=new List<Vector2>();
            eightTableList=new List<Vector2>();
            HeadUIDict=new Dictionary<string, ReferenceCollector>();
            chairArray=new string[8];
            cardUIDict=new Dictionary<int, UI>();
            currentTablePosList=new List<Vector2>();
            NiuNiuCardDict=new Dictionary<short, List<UI>>();
            rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            mainCardPos = rc.Get<GameObject>("MainCardPos").GetComponent<RectTransform>();
            mainHeadPos = rc.Get<GameObject>("mainHeadPos").GetComponent<RectTransform>();
            var mainTitle = rc.Get<GameObject>("mainTitle");
            LicensingPos = rc.Get<GameObject>("LicensingPos").GetComponent<RectTransform>().anchoredPosition;
            NNCardPrefab= rc.Get<GameObject>("NiuNIuCard");
            headUIform=rc.Get<GameObject>("HeadUIForm");
        }
        
        //屏幕适配需要获得的位置
        private List<Vector2> GetChildItem(Transform parentObj,List<Vector2> posList)
        {
            for (int i = 0; i < parentObj.childCount; i++)
            {
                posList.Add(parentObj.GetChild(i).GetComponent<RectTransform>().anchoredPosition);
            }
            return posList;
        }
        
        //获取当前房间出牌，头像的位置
        public void GetCurrentTablePos()
        {
            var sixTable = rc.Get<GameObject>("sixTable");
            var eightTable = rc.Get<GameObject>("eightTable");
            
            switch (roomPeople)
            {
                case 6:
                    currentTablePosList= GetChildItem(sixTable.transform, sixTableList);
                    currentTableObj = sixTable.transform;
                    break;
                case 8:
                    currentTablePosList=GetChildItem(eightTable.transform, eightTableList);
                    currentTableObj = eightTable.transform;
                    break;
            }
        }

        //寻找空闲椅子
        public int FindFreeChair(string userName)
        {
            for (int i = 0; i < chairArray.Length; i++)
            {
                if (chairArray[i] == null)
                {
                    chairArray[i] = userName;
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// 创建本地头像
        /// </summary>
        /// <param name="ChairIndex">椅子索引</param>
        public void CreateHead(int ChairIndex,AccountInfo playerInfo)
        {
            GameObject headObj = UnityEngine.Object.Instantiate(headUIform, currentTableObj);
            
            if (ChairIndex == -1)
            {
                headObj.GetComponent<RectTransform>().anchoredPosition = mainHeadPos.anchoredPosition;
            }
            else
            {
                headObj.transform.localScale=new Vector2(0.7f,0.7f);
                headObj.GetComponent<RectTransform>().anchoredPosition = currentTablePosList[ChairIndex];
            }
           
            SetHeadUIComponent(headObj, playerInfo);
            List<string> chairList= chairArray.ToList<string>();
            chairList.Add(playerInfo.UserName);
            chairArray= chairList.ToArray();
            HeadUIDict.Add(playerInfo.UserName,headObj.GetComponent<ReferenceCollector>());
        }

        
        //设置头像的信息
        private void SetHeadUIComponent(GameObject headItem,AccountInfo playerInfo)
        {
            ReferenceCollector rc = headItem.GetComponent<ReferenceCollector>();
            
            var headMask = rc.Get<GameObject>("headMask");
            var userNameTxt = rc.Get<GameObject>("uesrNameTxt").GetComponent<Text>();
            var scoreTxt = rc.Get<GameObject>("scoreTxt");
            var qiangzhuangImg = rc.Get<GameObject>("qiangzhuangImg");
            var BetsTitleImg = rc.Get<GameObject>("BetsTitleImg");
            var BetsTxt = BetsTitleImg.gameObject.transform.GetChild(0);
            var SelectImg = rc.Get<GameObject>("SelectImg");
            
            userNameTxt.text = playerInfo.UserName;
        }


        //翻牌动画
        private void FlopAniamtion(List<UI> cardList)
        {
            foreach (var card in cardList)
            {
                card.GameObject.GetComponent<Animator>().SetTrigger("IsFlop");
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

        //创建卡牌数据
        private void CreateCard(short chairIndex)
        {
            
        }
        
        //发牌
        private void Licensing()
        {
            
        }

        public void QuitRoom(string userName)
        {
            if (HeadUIDict.ContainsKey(userName))
            {
                UnityEngine.Object.Destroy(HeadUIDict[userName].gameObject);
                HeadUIDict.Remove(userName);
                for (int i = 0; i < chairArray.Length; i++)
                {
                    if (chairArray[i] == userName)
                    {
                        chairArray[i] = null;
                    }
                }
            }

        }

      

    }
}