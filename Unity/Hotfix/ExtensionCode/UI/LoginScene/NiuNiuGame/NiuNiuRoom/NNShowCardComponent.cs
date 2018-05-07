﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ETModel;
using UnityEngine;
using DG.Tweening;
using DG;
using NPOI.SS.Formula.Functions;
using Text = UnityEngine.UI.Text;

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

        private List<Vector2> _sixTableList;                         //六人桌的位置
        private List<Vector2> _eightTableList;                       //八人桌的位置
        public int RoomPeople;                                     //房间人数
        
        private List<Vector2> _currentTablePosList;                 //当前房间位置
        private Transform _currentTableObj;                         //当前桌子
        private GameObject _nnCardPrefab;                           //卡牌预设
        private Dictionary<string,ReferenceCollector> _headUiDict;  //头像位置列表
        private string[] _chairArray;                               //椅子管理数组
        private Dictionary<int,UI> _cardUiDict;                     //卡牌位置列表
        private RectTransform _mainHeadPos;                         //主头像位置
        private RectTransform _mainCardPos;                         //主卡牌像位置
        private Vector2 _licensingPos;                              //发牌位置 
        private ReferenceCollector _rc;
        private Dictionary<short,List<UI>> _niuNiuCardDict;         //游戏中生产的卡牌
        private GameObject _headUIform;
     

        #endregion
 
        public async void Awake()
        {
            _sixTableList=new List<Vector2>();
            _eightTableList=new List<Vector2>();
            _headUiDict=new Dictionary<string, ReferenceCollector>();
            _chairArray=new string[8];
            _cardUiDict=new Dictionary<int, UI>();
            _currentTablePosList=new List<Vector2>();
            _niuNiuCardDict=new Dictionary<short, List<UI>>();
            _rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            _mainCardPos = _rc.Get<GameObject>("MainCardPos").GetComponent<RectTransform>();
            _mainHeadPos = _rc.Get<GameObject>("mainHeadPos").GetComponent<RectTransform>();
            var mainTitle = _rc.Get<GameObject>("mainTitle");
            _licensingPos = _rc.Get<GameObject>("LicensingPos").GetComponent<RectTransform>().anchoredPosition;
            _nnCardPrefab= _rc.Get<GameObject>("NiuNIuCard");
            _headUIform=_rc.Get<GameObject>("HeadUIForm");
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
            var sixTable = _rc.Get<GameObject>("sixTable");
            var eightTable = _rc.Get<GameObject>("eightTable");
            
            switch (RoomPeople)
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
        /// <param name="chairIndex">椅子索引</param>
        /// <param name="playerInfo"></param>
        public void CreateHead(int chairIndex,AccountInfo playerInfo)
        {
            GameObject headObj = UnityEngine.Object.Instantiate(headUIform, currentTableObj);
            
            if (ChairIndex == -1)
            {
                headObj.GetComponent<RectTransform>().anchoredPosition = _mainHeadPos.anchoredPosition;
            }
            else
            {
                headObj.transform.localScale=new Vector2(0.7f,0.7f);
                headObj.GetComponent<RectTransform>().anchoredPosition = currentTablePosList[ChairIndex];
            }
            //设置头像信息
            SetHeadUIComponent(headObj, playerInfo);
           
            if (ChairIndex != -1)
            {
                var chairList= _chairArray.ToList<string>();
                Debug.Log("UserName"+playerInfo.UserName);
                chairList.Add(playerInfo.UserName);
                _chairArray= chairList.ToArray();
            }
            
            HeadUIDict.Add(playerInfo.UserName,headObj.GetComponent<ReferenceCollector>());
        }

        //获取字典里的值
        private static T2 GetDictValue<T1, T2>(IReadOnlyDictionary<T1, T2> dict,T1 key)
        {
            if (!dict.ContainsKey(key)) return default(T2);
            T2 value;
            dict.TryGetValue(key, out value);
            return value;
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
        
        
        //显示下注分数
        public void ShowBets(string userName,int score)
        {
            if (HeadUIDict.ContainsKey(userName))
            {
                ReferenceCollector rc;
                HeadUIDict.TryGetValue(userName, out rc);
                rc.Get<GameObject>("BetsTitleImg").SetActive(true);
                rc.Get<GameObject>("BetsTitleImg").transform.GetChild(0).GetComponent<Text>().text = score.ToString();
            }
        }


        //翻牌动画
        private void FlopAniamtion(IEnumerable<UI> cardList)
        {
            foreach (var card in cardList)
            {
                card.GameObject.GetComponent<Animator>().SetTrigger("IsFlop");
            }
        }

        //发牌动画
        private void Licensing(Vector2 targetPos,float spacing,IList<GameObject> cardList)
        {
            if (cardList.Count == 0) return;
            cardList[0].GetComponent<RectTransform>().anchoredPosition+=Vector2.Lerp(_licensingPos,targetPos,0.5f);
            if (!(cardList[0].GetComponent<RectTransform>().anchoredPosition.x - targetPos.x <= 0.01)) return;
            cardList[0].GetComponent<RectTransform>().anchoredPosition = targetPos;
            targetPos.x += spacing;
            cardList.RemoveAt(0);
        }

        //创建卡牌数据
        private void CreateCard(short chairIndex)
        {
            
        }
        
        //发牌
        private void Licensing()
        {
            
        }

        //离开房间
        public void QuitRoom(string userName)
        {
            if (!_headUiDict.ContainsKey(userName)) return;
            UnityEngine.Object.Destroy(_headUiDict[userName].gameObject);
            _headUiDict.Remove(userName);
            for (var i = 0; i < _chairArray.Length; i++)
            {
                if (_chairArray[i] == userName)
                {
                    _chairArray[i] = null;
                }
            }

        }

      

    }
}