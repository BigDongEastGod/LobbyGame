using System;
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
    
    [ObjectSystem]
    public class NnShowCardupdate : UpdateSystem<NnShowCardComponent>
    {
        public override void Update(NnShowCardComponent self)
        {
            self.Update();
        }
    }
    
    public class NnShowCardComponent:Component
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
        private Dictionary<string,List<ReferenceCollector>> _pokerObjList;                     //扑克缓存列表
        private bool IsFlop { get; set; }                                                      //是否可以翻牌
        private string CurrentUserName{ get; set; }                                            //当前用户名
 
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
        
        public void Update()
        {
            
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
 
        //创建头像
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
        
        //保存其他玩家的位置
        private void SaveOtherPlayerChair(string userName)
        {
            for (var i = 0; i < _chairArray.Length; i++)
            {
                if (_chairArray[i] != null) continue;
                _chairArray[i] = userName;
                return;
            }
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
        
        //显示庄家头像
        public void ShowZhuangJiaIcon(string userName)
        {
            var rc = GetDictValue(_headUiDict, userName);
            var zhuangjiaImg=rc.Get<GameObject>("zhuangjiaImg");
            zhuangjiaImg.SetActive(true);
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
    
        //创建卡牌
        private void CreateCard(List<PokerCard> porkerList)
        {
 
            for (var i = 0; i < _headUiDict.Count; i++)
            {
                List<ReferenceCollector> tempCardList=new List<ReferenceCollector>();
                for (var j = 0; j < 5; j++)
                {
                    var cardObj= UnityEngine.Object.Instantiate(_nnCardPrefab);
                    cardObj.transform.SetParent(_currentTableObj,false);
                    cardObj.transform.localScale=new Vector2(0.3f,0.3f);
                    cardObj.GetComponent<RectTransform>().anchoredPosition = _licensingPos;
                    //如果是本家
                    if (i == 0)
                    {
                        //加载扑克数据
                        LoadPorkerData(cardObj.GetComponent<ReferenceCollector>(), porkerList[j]);
                    }
                    tempCardList.Add(cardObj.GetComponent<ReferenceCollector>());
                }
 
                var name = i == 0 ? CurrentUserName : _chairArray[i - 1];
                _pokerObjList.Add(i == 0 ? CurrentUserName : _chairArray[i-1], tempCardList);
                Debug.Log("添加的账号名是:"+name+"当前的索引是"+i);
            }
        }
    
        //加载扑克的数据
        private static void LoadPorkerData(ReferenceCollector rc, PokerCard data)
        {
            var jokerImg = rc.Get<GameObject>("jokerImg").GetComponent<Image>();
            var cardNumber = rc.Get<GameObject>("cardNumber").GetComponent<Image>();
            var flowerColor = rc.Get<GameObject>("FlowerColor").GetComponent<Image>();
            var bigFlowerColor1 = rc.Get<GameObject>("bigFlowerColor1").GetComponent<Image>();
            var bigFlowerColor2 = rc.Get<GameObject>("bigFlowerColor2").GetComponent<Image>();
            //如果不是王
            if (data.CardType != 4)
            {
                //获得牌的点数
                cardNumber.sprite = rc.Get<Sprite>("num_" + data.CardNumber);
                //获得卡牌的颜色
                switch (data.CardType)
                {
                    case 0:
                    case 1: //红桃 方块
                        cardNumber.GetComponent<Image>().color = Color.red;
                        break;
                    case 2:
                    case 3: //梅花 黑桃
                        cardNumber.GetComponent<Image>().color = Color.black;
                        break;
                }

                //如果不是J Q K
                if (data.CardNumber < 10)
                {
                    flowerColor.sprite = rc.Get<Sprite>("t_" + data.CardType);
                    bigFlowerColor1.sprite = rc.Get<Sprite>("t_" + data.CardType);
                }
                //如果是J Q K
                else
                {
                    //获得王的标志
                    cardNumber.sprite = rc.Get<Sprite>("joker");
                    //大小王的颜色设置
                    cardNumber.GetComponent<Image>().color = data.CardNumber == 1 ? Color.red : Color.white;

                    bigFlowerColor2.gameObject.SetActive(true);
                    bigFlowerColor1.gameObject.SetActive(false);
                    flowerColor.sprite = rc.Get<Sprite>("k_" + data.CardNumber);
                    bigFlowerColor2.sprite = rc.Get<Sprite>("k_" + data.CardNumber);
                }
            }
        }
        
        //发牌
        public void Licensing(List<PokerCard> porkerList)
            {
                //创建
                CreateCard(porkerList);
            
                for (var i = 0; i < _headUiDict.Count; i++)
                {
                    List<ReferenceCollector> rcList;
                    if (i == 0)
                    {
                        if (!_headUiDict.ContainsKey(CurrentUserName)) continue;
                        rcList = GetDictValue(_pokerObjList, CurrentUserName);
                        SendFiveCards(true, rcList);
                    }
                    else
                    {
                        if (i == _headUiDict.Count - 1) IsFlop = true;
                        if (!_headUiDict.ContainsKey(_chairArray[i-1])) continue;
                        rcList = GetDictValue(_pokerObjList, _chairArray[i - 1]);
                        var targerPos= (GetCardPos(i - 1));
                        //发五张牌
                        SendFiveCards(false, rcList,targerPos);
                    }
                }
            }
    
        //异步发五张牌
        private async void SendFiveCards(bool isSelf,List<ReferenceCollector> rcList,Vector2 tempPos=default(Vector2))
        {
            float delay = 0.75f;
            for (var i = 0; i < 5; i++)
            {
          
                if (isSelf)
                {
                    //获得对应的位置
                    var targerPos = _mainCardPos.anchoredPosition + new Vector2(170 * i, 0);
                
                    await LicensingAnim(rcList[i].gameObject, targerPos,delay);
                    rcList[i].gameObject.transform.localScale=new Vector2(1,1);
                
                }
                else
                {
                    //获得对应的位置
                    var targerPos=tempPos+ new Vector2(30 * i, 0);
                    await LicensingAnim(rcList[i].gameObject, targerPos,delay);
                    rcList[i].gameObject.transform.localScale=new Vector2(0.5f,0.5f);
                    if (i == 4)
                    {
                        if (IsFlop)
                            FlopAniamtion(GetDictValue(_pokerObjList, CurrentUserName));
                    }
                }
                delay += 0.05f;
            }
        }
            
        //获得对应的位置卡牌的出牌位置
        private Vector2 GetCardPos(int chairIndex)
        {
            if (RoomPeople == 6)
            {
                chairIndex += 5;
            }
            else 
            {
                chairIndex += 7;
            }
            return _currentTablePosList[chairIndex];
        }

        //发牌动画
        private async Task<TweenCallback> LicensingAnim(GameObject obj,Vector2 targetPos,float delay)
        {
            await Task.CompletedTask;
            var tempPos = targetPos + new Vector2(-960, 0);
            var pos=new Vector3(tempPos.x,tempPos.y);
            return obj.transform.GetComponent<RectTransform>().DOLocalMove(pos,delay).onComplete;
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