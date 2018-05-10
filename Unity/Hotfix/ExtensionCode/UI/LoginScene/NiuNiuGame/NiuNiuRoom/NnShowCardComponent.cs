using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
using DG.Tweening;
using DG;
using UnityEngine.UI;
using Text = UnityEngine.UI.Text;
 
namespace ETHotfix
{
    [ObjectSystem]
    public class NnShowCardComponentAwakeSystem : AwakeSystem<NnShowCardComponent,object[]>
    {
        public override void Awake(NnShowCardComponent self,object[] args)
        {
            self.Awake(args);
        }
    }

    
    public class NnShowCardComponent:Component
    {
        #region variable

        private List<Vector2> _sixTableList;                                               //六人桌的位置
        private List<Vector2> _eightTableList;                                             //八人桌的位置
        public int RoomPeople;                                                             //房间人数
        public long RoomId { get; set; }                                                   //房间号
        public int TipsIndex { get; set; }                                                 //自己排型的提示
        public List<PokerCard> SortedCardList;                                             //自己排序过的卡牌

        private List<Vector2> _currentTablePosList;                                        //当前房间位置
        private Transform _currentTableObj;                                                //当前桌子
        private GameObject _nnCardPrefab;                                                  //卡牌预设
        private Dictionary<string,ReferenceCollector> _headUiDict;                         //头像位置列表
        private string[] _chairArray;                                                      //椅子管理数组
        private RectTransform _mainHeadPos;                                                //主头像位置
        private RectTransform _mainCardPos;                                                //主卡牌像位置
        private Vector2 _mainTipsPos;                                                      //主提示位置
        private Vector2 _licensingPos;                                                     //发牌位置 
        private ReferenceCollector _rc;                                                    //当前窗口引用类
        private GameObject _headUIform;                                                    //头像预设
        private Dictionary<string,List<ReferenceCollector>> _pokerObjList;                 //扑克缓存列表
        private bool IsFlop { get; set; }                                                  //是否可以翻牌
        private string CurrentUserName{ get; set; }                                        //当前用户名
        private NiuNiuMainComponent _niuNiuMainUi;                                         //牛牛主场景
        private List<GameObject> _tipsObjList;                                             //提示UI的管理列表
        private GameObject _tipsItem;                                                      //提示预设
   
        #endregion

        public async void Awake(object[] args)
        {
            _sixTableList=new List<Vector2>();
            _eightTableList=new List<Vector2>();
            _tipsObjList=new List<GameObject>();
            _headUiDict=new Dictionary<string, ReferenceCollector>();
            _pokerObjList=new Dictionary<string, List<ReferenceCollector>>();
            SortedCardList=new List<PokerCard>();
            _chairArray=new string[8];
            _currentTablePosList=new List<Vector2>();
            _rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            _mainCardPos = _rc.Get<GameObject>("MainCardPos").GetComponent<RectTransform>();
            _mainHeadPos = _rc.Get<GameObject>("mainHeadPos").GetComponent<RectTransform>();
            var mainTitle = _rc.Get<GameObject>("mainTitle");
            _licensingPos = _rc.Get<GameObject>("LicensingPos").GetComponent<RectTransform>().anchoredPosition;
            _nnCardPrefab= _rc.Get<GameObject>("NiuNIuCard");
            _headUIform=_rc.Get<GameObject>("HeadUIForm");
            _mainTipsPos=_rc.Get<GameObject>("MainTipsPos").GetComponent<RectTransform>().anchoredPosition;
            _tipsItem=_rc.Get<GameObject>("TipsItem");
            _niuNiuMainUi = (NiuNiuMainComponent)args[0];
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
                    _currentTablePosList= GetChildItem(sixTable.transform, _sixTableList);
                    _currentTableObj = sixTable.transform;
                    break;
                case 8:
                    _currentTablePosList=GetChildItem(eightTable.transform, _eightTableList);
                    _currentTableObj = eightTable.transform;
                    break;
            }
        }
    
        //寻找空闲椅子
        public int FindFreeChair(string userName)
        {
            for (int i = 0; i < _chairArray.Length; i++)
            {
                if (_chairArray[i] == null)
                {
                    _chairArray[i] = userName;
                    return i;
                }
            }
            return -1;
        }
        
        //在椅子座位上添加玩家
        public void AddSeatInfo(int index,string userName)
        {
            _chairArray[index] = userName;
        }

        //找到这个玩家的椅子位置
        private int GetChairIndex(string userName)
        {
            for (var i = 0; i < _chairArray.Length; i++)
            {
                if (userName == _chairArray[i]) return i;
            }
            return -1;
        }
        
        //创建头像
        public void CreateHead(int chairIndex,AccountInfo playerInfo)
        {
            GameObject headObj = UnityEngine.Object.Instantiate(_headUIform, _currentTableObj);
            
            if (chairIndex == -1)
            {
                headObj.GetComponent<RectTransform>().anchoredPosition = _mainHeadPos.anchoredPosition;
                CurrentUserName = playerInfo.UserName;
            }
            else
            {
                headObj.transform.localScale=new Vector2(0.7f,0.7f);
                headObj.GetComponent<RectTransform>().anchoredPosition = _currentTablePosList[chairIndex];
            }
            //设置头像信息
            SetHeadUiComponent(headObj, playerInfo);
           
            _headUiDict.Add(playerInfo.UserName,headObj.GetComponent<ReferenceCollector>());
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
        private static void SetHeadUiComponent(GameObject headItem,AccountInfo playerInfo)
        {
            var rc = headItem.GetComponent<ReferenceCollector>();
            
            var headMask = rc.Get<GameObject>("headMask");
            var userNameTxt = rc.Get<GameObject>("uesrNameTxt").GetComponent<Text>();
            var scoreTxt = rc.Get<GameObject>("scoreTxt");
            var qiangzhuangImg = rc.Get<GameObject>("qiangzhuangImg");
            var betsTitleImg = rc.Get<GameObject>("BetsTitleImg");
            var betsTxt = betsTitleImg.gameObject.transform.GetChild(0);
            var selectImg = rc.Get<GameObject>("SelectImg");
            userNameTxt.text = playerInfo.UserName;

        }
        
        //显示庄家头像
        public async void ShowZhuangJiaIcon(string userName)
        {
            var rc = GetDictValue(_headUiDict, userName);
            var zhuangjiaImg=rc.Get<GameObject>("zhuangjiaImg");
            zhuangjiaImg.SetActive(true);
            var betsResponse =(GameBankerResponse) await SceneHelperComponent.Instance.Session.Call(new GameBankerRequest(){RoomId = RoomId});
        }
    
        //显示下注分数
        public void ShowBets(string userName,int score)
        {
            if (!_headUiDict.ContainsKey(userName)) return;
            ReferenceCollector rc;
            _headUiDict.TryGetValue(userName, out rc);
            if (rc == null) return;
            rc.Get<GameObject>("BetsTitleImg").SetActive(true);
            rc.Get<GameObject>("BetsTitleImg").transform.GetChild(0).GetComponent<Text>().text =score.ToString();
        }

        //翻牌
        public void FlopCard(string userName,bool isShowTipsButton)
        {
            FlopAniamtion(GetDictValue(_pokerObjList, userName));
            if(! isShowTipsButton) return;
            _niuNiuMainUi.SwitchFlopCard(false);
            _niuNiuMainUi.SwitchTipsCard(true);
        }

        //翻牌动画
        private static void FlopAniamtion(IEnumerable<ReferenceCollector> cardList)
        {
            foreach (var card in cardList)
            {
                card.GetComponent<Animator>().SetTrigger("IsFlop");
            }
        }
    
        //创建卡牌
        private void CreateCard(IReadOnlyList<PokerCard> porkerList)
        {
            Debug.Log("  在创建卡牌的时候_headUiDict的长度是"+_headUiDict.Count);
            Debug.Log("  porkerList的长度是"+porkerList.Count);
            
            for (var i = 0; i < _headUiDict.Count; i++)
            {
                
                var tempCardList=new List<ReferenceCollector>();
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

                _pokerObjList.Add(i == 0 ? CurrentUserName : _chairArray[i-1], tempCardList);
                var name = i == 0 ? CurrentUserName : _chairArray[i - 1];
                Debug.Log("  _pokerObjList添加了"+i+"次，添加的key是"+name);
            }
        }
        
        //修改自己卡牌的顺序
        private void SortCard(List<ReferenceCollector> cardList)
        {
            for (var i = 0; i < SortedCardList.Count; i++)
            {
                LoadPorkerData(cardList[i], SortedCardList[i]);
            }
        }

        //显示其他玩家的牌
        public void LoadOtherCard(List<PokerCard> pokerList,string userName)
        {
            for (var i = 0; i < GetDictValue(_pokerObjList, userName).Count; i++)
            {
                LoadPorkerData(GetDictValue(_pokerObjList, userName)[i], pokerList[i]);
            }
            FlopCard(userName,false);
        }

        //显示提示牌UI
        public void ShowTipsUi(string userName,int tipsIndex=0)
        {
            var tipsItem = UnityEngine.Object.Instantiate(_tipsItem).GetComponent<Image>();
            var rc = tipsItem.GetComponent<ReferenceCollector>();
            tipsItem.transform.SetParent(_currentTableObj,false);
        
            if (userName==CurrentUserName)
            {
                tipsItem.GetComponent<RectTransform>().anchoredPosition = _mainTipsPos;
                tipsItem.sprite = rc.Get<Sprite>("nn_niu" + TipsIndex);
            }
            else
            {
                //进行位置转换
                tipsItem.GetComponent<RectTransform>().anchoredPosition= GetCardPos(GetChairIndex(userName));
                tipsItem.transform.localScale=new Vector2(0.5f,0.5f);
                tipsItem.sprite = rc.Get<Sprite>("nn_niu" + tipsIndex);
            }

            SortCard(GetDictValue(_pokerObjList, userName));
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
            if (data.CardType != 5)
            {
                if (jokerImg.gameObject.activeInHierarchy)
                {
                    jokerImg.gameObject.SetActive(false);
                    cardNumber.gameObject.SetActive(true);
                    flowerColor.gameObject.SetActive(true);
                }

                //获得牌的点数
                cardNumber.sprite = rc.Get<Sprite>("num_" + data.CardNumber);
                flowerColor.sprite = rc.Get<Sprite>("t_" + data.CardType);
              
                //如果不是J Q K
                if (data.CardNumber < 11)
                {
                    if (bigFlowerColor2.gameObject.activeInHierarchy)
                    {
                        bigFlowerColor2.gameObject.SetActive(false);
                        bigFlowerColor1.gameObject.SetActive(true);
                    }

                    bigFlowerColor1.sprite = rc.Get<Sprite>("t_" + data.CardType);
                }
                //如果是J Q K
                else
                {
                    if (bigFlowerColor1.gameObject.activeInHierarchy)
                    {
                        bigFlowerColor2.gameObject.SetActive(true);
                        bigFlowerColor1.gameObject.SetActive(false);
                    }

                    bigFlowerColor2.sprite = rc.Get<Sprite>(data.CardType +"_" +data.CardNumber);
                }

                if (data.CardNumber >11) return;
                
                //获得卡牌的颜色
                switch (data.CardType)
                {
                    case 1:
                    case 3: //红桃 方块
                        cardNumber.GetComponent<Image>().color = Color.black;
                        break;
                    case 2:
                    case 4: //梅花 黑桃
                        cardNumber.GetComponent<Image>().color = Color.red;
                        break;
                }
            }
            else
            {
                if (cardNumber.gameObject.activeInHierarchy)
                {
                    cardNumber.gameObject.SetActive(false);
                    flowerColor.gameObject.SetActive(false);
                    //获得王的标志
                    jokerImg.gameObject.SetActive(true);
                }
            
                //大小王的颜色设置
                jokerImg.GetComponent<Image>().color = data.CardNumber == 1 ? Color.black : Color.red;

                if (bigFlowerColor1.gameObject.activeInHierarchy)
                {
                    bigFlowerColor1.gameObject.SetActive(false);
                    bigFlowerColor2.gameObject.SetActive(true);
                }

                bigFlowerColor2.sprite = rc.Get<Sprite>("k_" + data.CardNumber);
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
                            _niuNiuMainUi.ShowFlopCardButton();
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