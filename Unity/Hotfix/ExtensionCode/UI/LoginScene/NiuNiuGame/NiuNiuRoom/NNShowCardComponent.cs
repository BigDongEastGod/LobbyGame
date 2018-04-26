using System.Collections.Generic;
using ETModel;
using UnityEngine;

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
        
        public List<Vector2> sixTableList;           //六人桌的位置
        public List<Vector2> eightTableList;         //八人桌的位置
        public ushort roomPeople;                    //房间人数
        
        
        private List<Vector2> currentTablePosList;   //当前房间位置
        private Transform currentTableObj;           //当前桌子
        private GameObject NNCardPrefab;             //卡牌预设
        private Dictionary<short,UI> HeadUIDict;     //头像列表
        private Dictionary<short,UI> cardUIDict;     //卡牌列表
        private RectTransform mainHeadPos;           //主头像位置
        private RectTransform mainCardPos;           //主卡牌像位置
        private Vector2 LicensingPos;                //发牌位置 

        #endregion
 
        public void Awake()
        {
            sixTableList=new List<Vector2>();
            eightTableList=new List<Vector2>();
            currentTablePosList=new List<Vector2>();
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            mainCardPos = rc.Get<GameObject>("MainCardPos").GetComponent<RectTransform>();
            mainHeadPos = rc.Get<GameObject>("mainHeadPos").GetComponent<RectTransform>();
            var mainTitle = rc.Get<GameObject>("mainTitle");
            var sixTableObj = rc.Get<GameObject>("sixTable");
            var eightTableObj = rc.Get<GameObject>("eightTable");
            LicensingPos = rc.Get<GameObject>("LicensingPos").GetComponent<RectTransform>().anchoredPosition;
            NNCardPrefab= rc.Get<GameObject>("NiuNIuCard");
            GetCurrentTablePos(sixTableObj, eightTableObj);
            
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
        private void GetCurrentTablePos(GameObject sixTable,GameObject eightTable)
        {
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
        
        //当前的字典是否为空
        private void IsListNull<T,A>(Dictionary<T,A> dict)
        {
            if(dict!=null) return;
            dict=new Dictionary<T,A>();
        }

        /// <summary>
        /// 创建本地头像
        /// </summary>
        /// <param name="ChairIndex">椅子索引</param>
        private void CreateHead(int ChairIndex)
        {
            IsListNull<short,UI>(HeadUIDict);
            UI headUI= Game.Scene.GetComponent<UIComponent>().Create(UIType.HeadUIForm,currentTableObj);
            short index;
            if (ChairIndex != -1)
            {
                headUI.GameObject.GetComponent<RectTransform>().anchoredPosition = mainHeadPos.anchoredPosition;
                index = -1;
            }
            else
            {
                headUI.GameObject.transform.localScale=new Vector2(0.7f,0.7f);
                headUI.GameObject.GetComponent<RectTransform>().anchoredPosition = currentTablePosList[ChairIndex];
                index = (short)ChairIndex;
            }
            HeadUIDict.Add(index,headUI);
        }
        
        //取得对应位置的UI
        private UI GetIndexUI(short index)
        {
            UI headUI;
            HeadUIDict.TryGetValue(index, out headUI);
            return headUI;
        }

        #region 调用头像组建的函数

        //显示自己准备
        private void GetReady(short index)
        {
            GetIndexUI(index);
            //调用显示准备的函数 todo...
        }
        
        //显示否抢庄
        private void SHowQingZhuang(short index)
        {
            
        }
        
        //显示加注的金币UI
        private void ShowBetsUI()
        {
            
        }
        
        //设置用户名
        private void SetUserName()
        {
            
        }

        //设置得分
        private void SetScore()
        {
            
        }

        //金币移动动画
        private void MoveCoinAnimation()
        {
            
        }

        #endregion
        
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
        
        
        

    }
}