using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
using DG.Tweening;
using DG;
using UnityEngine.EventSystems;
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

    public class NnShowCardComponentUpdateSystem : UpdateSystem<NnShowCardComponent>
    {
        public override void Update(NnShowCardComponent self)
        {
            self.Update();
        }
    }

    public class NnShowCardComponent:Component
    {
        #region variable

        public int RoomPeople;                                                             //房间人数
        public long RoomId;                                                                //房间号
        public int TipsIndex;                                                              //自己排型的提示
        private GameObject _nnCardPrefab;                                                  //卡牌预设
        private GameObject _headUIform;                                                    //头像预设
        private GameObject _tipsItem;                                                      //提示预设
        private List<Vector2> _sixTableList;                                               //六人桌的位置
        private List<Vector2> _eightTableList;                                             //八人桌的位置
        private List<Vector2> _currentTablePosList;                                        //当前房间位置
        private Transform _currentTableObj;                                                //当前桌子
        private string[] _chairArray;                                                      //椅子管理数组
        private Dictionary<string,ReferenceCollector> _headUiDict;                         //头像位置列表
        private RectTransform _mainHeadPos;                                                //主头像位置
        private RectTransform _mainCardPos;                                                //主卡牌像位置
        private Vector2 _mainTipsPos;                                                      //主提示位置
        private Vector2 _licensingPos;                                                     //发牌位置 
        private ReferenceCollector _rc;                                                    //当前窗口引用类
        private NiuNiuMainComponent _niuNiuMainUi;                                         //牛牛主场景
        private string CurrentUserName{ get; set; }                                        //当前用户名
        //需要变动的容器
        private Dictionary<string,GameObject> _tipsObjList;                                //提示UI的管理列表
        public List<PokerCard> SortedCardList;                                             //自己排序过的卡牌
        private Dictionary<string,List<ReferenceCollector>> _pokerObjList;                 //扑克缓存列表
        private bool IsFlop { get; set; }                                                  //是否可以翻牌
        private GameObject _shuffleMask;
        private GameObject _cardParent;
        private Canvas _gameCanvas;

        #endregion

        public async void Awake(object[] args)
        {
            _sixTableList=new List<Vector2>();
            _eightTableList=new List<Vector2>();
            _tipsObjList=new Dictionary<string, GameObject>();
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
            _shuffleMask = _rc.Get<GameObject>("ShuffleMask");
            _cardParent=_rc.Get<GameObject>("CardParent");
            _gameCanvas = GameObject.FindGameObjectWithTag("GameCanvas").GetComponent<Canvas>();
            AddCardDrag();
        }

        private void AddCardDrag()
        {
            SceneHelperComponent.Instance.MonoEvent.AddEventTrigger(_cardParent.transform.GetChild(0).gameObject,
                EventTriggerType.Drag,(obj)=> OnDragCard(obj,0));
//            for (var i = 0; i < _cardParent.transform.childCount; i++)
//            {
//                SceneHelperComponent.Instance.MonoEvent.AddEventTrigger(_cardParent.transform.GetChild(i).gameObject,
//                    EventTriggerType.BeginDrag,(obj)=> OnDragCard(obj,i));
//            }
        }

        private void OnDragCard(BaseEventData obj,int index)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_gameCanvas.transform as RectTransform,
                Input.mousePosition, null, out position);
            _cardParent.transform.GetChild(index).localPosition = position;
        }


        public void Update()
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_gameCanvas.transform as RectTransform,
                Input.mousePosition, null, out position);
            _cardParent.transform.GetChild(0).localPosition = position;
        }

        //屏幕适配需要获得的位置
        private List<Vector2> GetChildItem(Transform parentObj,List<Vector2> posList)
        {
            for (var i = 0; i < parentObj.childCount; i++)
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
            for (var i = 0; i < _chairArray.Length; i++)
            {
                if (_chairArray[i] != null) continue;
                _chairArray[i] = userName;
                return i;
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
            var headObj = UnityEngine.Object.Instantiate(_headUIform, _currentTableObj);
            
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
        
        //消除准备
        private void RemoveReady()
        {
            foreach (var head in _headUiDict)
            {
                head.Value.Get<GameObject>("headMask").SetActive(false);
            }
        }
        
        //让玩家头像显示准备
        private void ShowReady(string usserName)
        {
            if (!_headUiDict.ContainsKey(usserName)) return;
            GetDictValue(_headUiDict,usserName).Get<GameObject>("headMask").SetActive(true);
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
            }
        }

        private void CreateCard2(IReadOnlyList<PokerCard> porkerList)
        {
            foreach (var poker in _pokerObjList)
            {
                for (var i = 0; i < 5; i++)
                {
                    if (poker.Key == CurrentUserName)
                    {
                        LoadPorkerData(poker.Value[i], porkerList[i]);
                    }
                    poker.Value[i].gameObject.SetActive(true);
                }
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
            var otherList = GetDictValue(_pokerObjList, userName);
            for (var i = 0; i < GetDictValue(_pokerObjList, userName).Count; i++)
            {
                LoadPorkerData(otherList[i], pokerList[i]);
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
                if (TipsIndex == 0) return;
                SortCard(GetDictValue(_pokerObjList, userName));
            }
            else
            {
                //进行位置转换
                tipsItem.GetComponent<RectTransform>().anchoredPosition= GetCardPos(GetChairIndex(userName));
                tipsItem.transform.localScale=new Vector2(0.5f,0.5f);
                tipsItem.sprite = rc.Get<Sprite>("nn_niu" + tipsIndex);
            }
            if(_tipsObjList.ContainsKey(userName)) return;
            _tipsObjList.Add(userName,tipsItem.gameObject);
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
            //如果不是重置游戏
            if (_pokerObjList.Count == 0)
            {
                CreateCard(porkerList);
            }
            else
            {
                CreateCard2(porkerList);
            }

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
            var delay = 0.75f;
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
        
        //显示赢家
        public void ShowWinUi(string userName)
        {
            GetDictValue(_headUiDict,userName).Get<GameObject>("SettlementImage").SetActive(true);
        }
        
        //重新开始游戏
        private void ResetGame()
        {
            //清除所有的数据容器数据
            foreach (var tipsItem in _tipsObjList)
            {
                tipsItem.Value.SetActive(false);
            }
            
            SortedCardList.Clear();
            IsFlop = false;
            //清除场上的牌
            ResetPoker();
            //显示准备按钮
            _niuNiuMainUi.ReadyButtn.gameObject.SetActive(true);
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(_niuNiuMainUi.ReadyButtn, () =>
            {
                
            });
            
        }
        
        //重置开卡牌数据
        private void ResetPoker()
        {
            foreach (var pokerList in _pokerObjList)
            {
                foreach (var poker in pokerList.Value)
                {
                    poker.Get<GameObject>("BackImg").SetActive(true);
                    poker.transform.localScale=new Vector2(0.3f,0.3f);
                    poker.GetComponent<RectTransform>().anchoredPosition = _licensingPos;
                    poker.gameObject.SetActive(false);
                }
            }
        }

        #region 搓牌动作

        //点击搓牌按钮
        public void OnShuffleBUtton()
        {
            JointCardAnim(GetDictValue(_pokerObjList, CurrentUserName));
        }

        //合牌动画并下滑
        private async void JointCardAnim(List<ReferenceCollector> pokerList)
        {
            var targetPos = pokerList[2].GetComponent<RectTransform>().anchoredPosition; //+new Vector2(-960,0);
            
            for (var i = 0; i < pokerList.Count; i++)
            {
                if(i==2) continue;
                await MoveCardTask(pokerList[i].gameObject, targetPos);
                if (i != pokerList.Count-1) continue;
                await Task.Delay(380);
                for (var j = 0; j < 5; j++)
                {
                    DeclineAnim(pokerList);
                }
            }

            await Task.Delay(200);
            _shuffleMask.SetActive(true);
            _shuffleMask.GetComponent<ReferenceCollector>().Get<GameObject>("CardParent").GetComponent<Animator>().SetTrigger("IsCuoPai");
            
        }

        private async Task<TweenCallback> MoveCardTask(GameObject cardObj, Vector2 targetPos)
        {
            await Task.CompletedTask;
            var tempPos = targetPos + new Vector2(-960, 0);
            var pos=new Vector3(tempPos.x,tempPos.y);
            return cardObj.transform.GetComponent<RectTransform>().DOLocalMove(pos,0.4f).onComplete;
        }

        private async void DeclineAnim(List<ReferenceCollector> pokerList)
        {
            for (var i = 0; i < pokerList.Count; i++)   
            {
                await DeclineAnimTaask(pokerList[i].gameObject);
                if(i==pokerList.Count-1) continue;
            }
        }
        
        private async Task<TweenCallback> DeclineAnimTaask(GameObject cardObj)
        {
            await Task.CompletedTask;
            var pos=new Vector3(_niuNiuMainUi.StartPos.x,_niuNiuMainUi.StartPos.y);
            pos+=new Vector3(-960, 0,0);
            return cardObj.transform.GetComponent<RectTransform>().DOLocalMove(pos,1f).onComplete;
        }

        #endregion
        

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