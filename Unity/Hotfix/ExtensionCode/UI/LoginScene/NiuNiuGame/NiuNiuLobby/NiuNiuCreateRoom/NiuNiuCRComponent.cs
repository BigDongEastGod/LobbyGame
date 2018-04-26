using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class NiuNiuCRComponentAwakeSystem : AwakeSystem<NiuNiuCRComponent>
    {
        public override void Awake(NiuNiuCRComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class NiuNiuCRComponentStartSystem : StartSystem<NiuNiuCRComponent>
    {
        public override void Start(NiuNiuCRComponent self)
        {
            self.Start();
        }
    }

    enum NNPaiJuType
    {
        NNSZ,
        GDZJ,
        ZYQZ,
        MPQZ,
        TBNN
    }

    public class NNDpType
    {
        public const string DiFen = "DiFen";
        public const string JuShu = "JuShu";
        public const string FangFei = "FangFei";
        public const string ZiDongKaiZhuo = "ZiDongKaiZhuo";
        public const string XianJiaTuiZhu = "XianJiaTuiZhu";
        public const string ShangZhuangFenShu = "ShangZhuangFenShu";
        public const string ZuiDaQiangZhuang = "ZuiDaQiangZhuang";
        public const string FanBeiGuiZe = "FanBeiGuiZe";
    }

    public class NiuNiuRule
    {
        public List<string> ListDiFen { get; }
        public List<string> ListjuShu { get; }
        public List<string> ListFangFei { get; }
        public List<string> ListZiDongKaiZhuo { get; }
        public List<string> ListXianJiaTuiZhu { get; }
        public List<string> ListShangZhuangFenShu { get; }
        public List<string> ListZuiDaQiangZhuang { get; }
        public List<string> ListFanBeiGuiZe { get; }
        public List<string> ListTeShuPaiXing { get; }
        public List<string> ListGaoJiXuanXiang { get; }

        public NiuNiuRule(List<string> listDiFen, List<string> listjuShu, List<string> listFangFei, List<string> listZiDongKaiZhuo, List<string> listXianJiaTuiZhu, List<string> listShangZhuangFenShu, List<string> listZuiDaQiangZhuang, List<string> listFanBeiGuiZe, List<string> listTeShuPaiXing, List<string> listGaoJiXuanXiang)
        {
            ListDiFen = listDiFen;
            ListjuShu = listjuShu;
            ListFangFei = listFangFei;
            ListZiDongKaiZhuo = listZiDongKaiZhuo;
            ListXianJiaTuiZhu = listXianJiaTuiZhu;
            ListShangZhuangFenShu = listShangZhuangFenShu;
            ListZuiDaQiangZhuang = listZuiDaQiangZhuang;
            ListFanBeiGuiZe = listFanBeiGuiZe;
            ListTeShuPaiXing = listTeShuPaiXing;
            ListGaoJiXuanXiang = listGaoJiXuanXiang;
        }
    }

    public class NiuNiuCRComponent : Component
    {
        private NiuNiuRule NiuNiuShangZhuang;
        private NiuNiuRule GuDingZhuangJia;
        private NiuNiuRule ZiYouQiangZhuang;
        private NiuNiuRule MingPaiQiangZhuang;
        private NiuNiuRule TongBiNiuNiu;

        private UI _nnLobby;
        private UI _nnosb;

        private GameObject _roomPeople6;
        private GameObject _roomPeople8;
        private GameObject _optionsLayout;
        private NNPaiJuType _curretNnPaiJuType;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            #region 得到游戏物体实例

            //创建房间页面
            var nnCreateRoom = rc.Get<GameObject>("NiuNiuCreateRoom");

            // 创建房间返回大厅按钮
            var nncrCloseBtn = rc.Get<GameObject>("NNCR_CloseBtn");

            // 房间人数Toggle
            _roomPeople6 = rc.Get<GameObject>("RoomPeople_6");
            _roomPeople8 = rc.Get<GameObject>("roomPeople_8");
            _optionsLayout = rc.Get<GameObject>("OptionsLayout");
            var toggleBtn = rc.Get<GameObject>("ToggleBtn");
            var nnOptionItem = rc.Get<GameObject>("NN_OptionItem");
            var createRoomBtn = rc.Get<GameObject>("CreateRoomBtn");
            var gaojixuanxiangDp = rc.Get<GameObject>("GaoJiXuanXiangDp");
            var teshupaixingDp = rc.Get<GameObject>("TeShuPaiXingDp");

            #endregion

            InitRule();
            
            // 返回牛牛大厅按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nncrCloseBtn.GetComponent<Button>(), () =>
            {
                nnCreateRoom.SetActive(false);
                _nnLobby.GameObject.SetActive(true);
            });

            // 规则切换按钮注册
            foreach (Transform toggle in toggleBtn.transform)
            {
                switch (toggle.name)
                {
                    case "NiuNiuShangZhuangTg":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(toggle.GetComponent<Button>(), () =>
                        {
                            // 切换显示标签 Background Checkmark
                            ChangeToggleMark(toggleBtn.transform, toggle);
                            // 切换规则内容显示
                            ShowRule(NiuNiuShangZhuang);
                            _curretNnPaiJuType = NNPaiJuType.NNSZ;
                        });
                        break;
                    case "GuDingZhuangJiaTg":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(toggle.GetComponent<Button>(), () =>
                        {
                            // 切换显示标签 Background Checkmark
                            ChangeToggleMark(toggleBtn.transform, toggle);
                            // 切换规则内容显示
                            ShowRule(GuDingZhuangJia);
                            _curretNnPaiJuType = NNPaiJuType.GDZJ;
                        });
                        break;
                    case "ZiYouQiangZhuangTg":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(toggle.GetComponent<Button>(), () =>
                        {
                            // 切换显示标签 Background Checkmark
                            ChangeToggleMark(toggleBtn.transform, toggle);
                            // 切换规则内容显示
                            ShowRule(ZiYouQiangZhuang);
                            _curretNnPaiJuType = NNPaiJuType.ZYQZ;
                        });
                        break;
                    case "MingPaiQiangZhuangTg":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(toggle.GetComponent<Button>(), () =>
                        {
                            // 切换显示标签 Background Checkmark
                            ChangeToggleMark(toggleBtn.transform, toggle);
                            // 切换规则内容显示
                            ShowRule(MingPaiQiangZhuang);
                            _curretNnPaiJuType = NNPaiJuType.MPQZ;
                        });
                        break;
                    case "TongBiNiuNiuTg":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(toggle.GetComponent<Button>(), () =>
                        {
                            // 切换显示标签 Background Checkmark
                            ChangeToggleMark(toggleBtn.transform, toggle);
                            // 切换规则内容显示
                            ShowRule(TongBiNiuNiu);
                            _curretNnPaiJuType = NNPaiJuType.TBNN;
                        });
                        break;
                }
            }

            // 特殊牌型
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(teshupaixingDp.GetComponent<Button>(), () =>
            {
                Vector2 osbPos = teshupaixingDp.transform.Find("OptionsPos").GetComponent<RectTransform>().anchoredPosition;
//                switch (_curretNnPaiJuType)
//                {
//                    case NNPaiJuType.NNSZ:
//                        break;
//                    case NNPaiJuType.GDZJ:
//                        break;
//                    case NNPaiJuType.ZYQZ:
//                        break;
//                    case NNPaiJuType.MPQZ:
//                        break;
//                    case NNPaiJuType.TBNN:
//                        break;
//                }
                _nnosb.GetComponent<NiuNiuOSBComponent>().InitPosAndSize(osbPos);
                _nnosb.GameObject.SetActive(true);
            });
            // 高级选项
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(gaojixuanxiangDp.GetComponent<Button>(), () =>
            {
                Vector2 osbPos = teshupaixingDp.transform.Find("OptionsPos").GetComponent<RectTransform>().anchoredPosition;
                _nnosb.GetComponent<NiuNiuOSBComponent>().InitPosAndSize(osbPos);
                _nnosb.GameObject.SetActive(true);
            });


            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(createRoomBtn.GetComponent<Button>(), CreatePaiJu);
        }

        public void Start()
        {
            _nnLobby = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuLobby);
            _nnosb = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuOSB);
        }

        private void ChangeToggleMark(Transform parent, Transform self)
        {
            foreach (Transform toggle in parent)
            {
                toggle.Find("Background").gameObject.SetActive(true);
                toggle.Find("Checkmark").gameObject.SetActive(false);
            }

            self.Find("Background").gameObject.SetActive(false);
            self.Find("Checkmark").gameObject.SetActive(true);
        }

        private void ShowRule(NiuNiuRule nnRule)
        {
            foreach (Transform row in _optionsLayout.transform)
            {
                switch (row.name)
                {
                    case "Row1":
                        SetDropDownList(row, nnRule, NNDpType.DiFen);
                        SetDropDownList(row, nnRule, NNDpType.JuShu);
                        break;
                    case "Row2":
                        SetDropDownList(row, nnRule, NNDpType.FangFei);
                        SetDropDownList(row, nnRule, NNDpType.ZiDongKaiZhuo);
                        break;
                    case "Row3":
                        int showCount = 0;
                        if (nnRule.ListXianJiaTuiZhu == null)
                        {
                            row.Find(NNDpType.XianJiaTuiZhu).gameObject.SetActive(false);
                        }
                        else
                        {
                            showCount++;
                            row.Find(NNDpType.XianJiaTuiZhu).gameObject.SetActive(true);
                            SetDropDownList(row, nnRule, NNDpType.XianJiaTuiZhu);
                        }

                        if (nnRule.ListShangZhuangFenShu == null)
                            row.Find(NNDpType.ShangZhuangFenShu).gameObject.SetActive(false);
                        else
                        {
                            showCount++;
                            row.Find(NNDpType.ShangZhuangFenShu).gameObject.SetActive(true);
                            SetDropDownList(row, nnRule, NNDpType.ShangZhuangFenShu);
                        }

                        if (nnRule.ListZuiDaQiangZhuang == null)
                            row.Find(NNDpType.ZuiDaQiangZhuang).gameObject.SetActive(false);
                        else
                        {
                            showCount++;
                            row.Find(NNDpType.ZuiDaQiangZhuang).gameObject.SetActive(true);
                            SetDropDownList(row, nnRule, NNDpType.ZuiDaQiangZhuang);
                        }

                        row.gameObject.SetActive(showCount > 0);

                        break;
                    case "Row4":
                        SetDropDownList(row, nnRule, NNDpType.FanBeiGuiZe);
                        break;
                }
            }
        }

        private void SetDropDownList(Transform row, NiuNiuRule nnRule, string option)
        {
            switch (option)
            {
                case NNDpType.DiFen:
                    Dropdown difenDp = row.Find($"{NNDpType.DiFen}/{NNDpType.DiFen}Dp").GetComponent<Dropdown>();
                    difenDp.ClearOptions();
                    difenDp.AddOptions(nnRule.ListDiFen);
                    break;
                case NNDpType.JuShu:
                    Dropdown juShuDp = row.Find($"{NNDpType.JuShu}/{NNDpType.JuShu}Dp").GetComponent<Dropdown>();
                    juShuDp.ClearOptions();
                    juShuDp.AddOptions(nnRule.ListjuShu);
                    break;
                case NNDpType.FangFei:
                    Dropdown fangfeiDp = row.Find($"{NNDpType.FangFei}/{NNDpType.FangFei}Dp").GetComponent<Dropdown>();
                    fangfeiDp.ClearOptions();
                    fangfeiDp.AddOptions(nnRule.ListFangFei);
                    break;
                case NNDpType.ZiDongKaiZhuo:
                    Dropdown zidongkaizhuoDp = row.Find($"{NNDpType.ZiDongKaiZhuo}/{NNDpType.ZiDongKaiZhuo}Dp").GetComponent<Dropdown>();
                    zidongkaizhuoDp.ClearOptions();
                    zidongkaizhuoDp.AddOptions(nnRule.ListZiDongKaiZhuo);
                    break;
                case NNDpType.XianJiaTuiZhu:
                    Dropdown xianjiatuizhuDp = row.Find($"{NNDpType.XianJiaTuiZhu}/{NNDpType.XianJiaTuiZhu}Dp").GetComponent<Dropdown>();
                    xianjiatuizhuDp.ClearOptions();
                    xianjiatuizhuDp.AddOptions(nnRule.ListXianJiaTuiZhu);
                    break;
                case NNDpType.ShangZhuangFenShu:
                    Dropdown shangzhuangfenshuDp = row.Find($"{NNDpType.ShangZhuangFenShu}/{NNDpType.ShangZhuangFenShu}Dp").GetComponent<Dropdown>();
                    shangzhuangfenshuDp.ClearOptions();
                    shangzhuangfenshuDp.AddOptions(nnRule.ListShangZhuangFenShu);
                    break;
                case NNDpType.ZuiDaQiangZhuang:
                    Dropdown zuidaqiangzhuangDp = row.Find($"{NNDpType.ZuiDaQiangZhuang}/{NNDpType.ZuiDaQiangZhuang}Dp").GetComponent<Dropdown>();
                    zuidaqiangzhuangDp.ClearOptions();
                    zuidaqiangzhuangDp.AddOptions(nnRule.ListZuiDaQiangZhuang);
                    zuidaqiangzhuangDp.value = 3;
                    break;
                case NNDpType.FanBeiGuiZe:
                    Dropdown fanbeiguizeDp = row.Find($"{NNDpType.FanBeiGuiZe}/{NNDpType.FanBeiGuiZe}Dp").GetComponent<Dropdown>();
                    fanbeiguizeDp.ClearOptions();
                    fanbeiguizeDp.AddOptions(nnRule.ListFanBeiGuiZe);
                    break;
            }
        }

        /// <summary>
        /// 牛牛规则
        /// </summary>
        private void InitRule()
        {
            NiuNiuShangZhuang = new NiuNiuRule(new List<string>() {"1/2", "2/4", "4/8", "5/10"},
                new List<string>() {"10局", "20局"},
                new List<string>() {"房主支付(       4)  ", "AA支付(每人      1)  "},
                new List<string>() {"手动开桌", "满5人开", "满6人开", "满7人开", "满8人开"},
                new List<string>() {"无", "5倍", "10倍", "20倍"},
                null,
                null,
                new List<string>() {"牛牛x4 牛九x3 牛八x2 牛七x2", "牛牛x3 牛九x2 牛八x2"},
                new List<string>() {"顺子牛(5倍)", "同花牛(5倍)", "葫芦牛(6倍)", "五花牛(5倍)", "炸弹牛(6倍)", "五小牛(8倍)"},
                new List<string>() {"中途禁入", "禁止搓牌", "王癞玩法"});

            GuDingZhuangJia = new NiuNiuRule(new List<string>() {"1/2", "2/4", "4/8", "5/10"},
                new List<string>() {"10局", "20局"},
                new List<string>() {"房主支付(       3)  ", "AA支付(每人      1)  "},
                new List<string>() {"手动开桌", "满4人开", "满5人开", "满5人开"},
                new List<string>() {"无", "5倍", "10倍", "20倍"},
                new List<string>() {"无", "100", "150", "200"},
                null,
                new List<string>() {"牛牛x4 牛九x3 牛八x2 牛七x2", "牛牛x3 牛九x2 牛八x2"},
                new List<string>() {"顺子牛(5倍)", "同花牛(5倍)", "葫芦牛(6倍)", "五花牛(5倍)", "炸弹牛(6倍)", "五小牛(8倍)"},
                new List<string>() {"中途禁入", "禁止搓牌", "王癞玩法", "闲家买码"});

            ZiYouQiangZhuang = new NiuNiuRule(new List<string>() {"1/2", "2/4", "4/8", "5/10"},
                new List<string>() {"10局", "20局"},
                new List<string>() {"房主支付(       3)  ", "AA支付(每人      1)  "},
                new List<string>() {"手动开桌", "满4人开", "满5人开", "满5人开"},
                new List<string>() {"无", "5倍", "10倍", "20倍"},
                null,
                null,
                new List<string>() {"牛牛x4 牛九x3 牛八x2 牛七x2", "牛牛x3 牛九x2 牛八x2"},
                new List<string>() {"顺子牛(5倍)", "同花牛(5倍)", "葫芦牛(6倍)", "五花牛(5倍)", "炸弹牛(6倍)", "五小牛(8倍)"},
                new List<string>() {"中途禁入", "禁止搓牌", "王癞玩法", "闲家买码", "下注限制", "暗抢庄家,下注加倍"});

            MingPaiQiangZhuang = new NiuNiuRule(new List<string>() {"1/2", "2/4", "4/8", "5/10"},
                new List<string>() {"10局", "20局"},
                new List<string>() {"房主支付(       3)  ", "AA支付(每人      1)  "},
                new List<string>() {"手动开桌", "满4人开", "满5人开", "满5人开"},
                new List<string>() {"无", "5倍", "10倍", "20倍"},
                null,
                new List<string>() {"1倍", "2倍", "3倍"},
                new List<string>() {"牛牛x4 牛九x3 牛八x2 牛七x2", "牛牛x3 牛九x2 牛八x2"},
                new List<string>() {"顺子牛(5倍)", "同花牛(5倍)", "葫芦牛(6倍)", "五花牛(5倍)", "炸弹牛(6倍)", "五小牛(8倍)"},
                new List<string>() {"中途禁入", "禁止搓牌", "王癞玩法", "闲家买码", "下注限制", "暗抢庄家,下注加倍"});

            TongBiNiuNiu = new NiuNiuRule(new List<string>() {"1/2", "2/4", "4/8", "5/10"},
                new List<string>() {"10局", "20局"},
                new List<string>() {"房主支付(       3)  ", "AA支付(每人      1)  "},
                new List<string>() {"手动开桌", "满4人开", "满5人开", "满5人开"},
                null,
                null,
                null,
                new List<string>() {"牛牛x4 牛九x3 牛八x2 牛七x2", "牛牛x3 牛九x2 牛八x2"},
                new List<string>() {"顺子牛(5倍)", "同花牛(5倍)", "葫芦牛(6倍)", "五花牛(5倍)", "炸弹牛(6倍)", "五小牛(8倍)"},
                new List<string>() {"中途禁入", "禁止搓牌", "王癞玩法"});
        }


        private async void CreatePaiJu()
        {
            Debug.Log("CreatePaiJu");

            var response =
                (CreateRoomResponse) await SceneHelperComponent.Instance.Session.Call(
                    new CreateRoomRequest() {RoomType = "NN"});


            Debug.Log("123");

            if (response.Error == 0)
            {
                Debug.Log(response.RoomId);

//                Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuLobby);
//                Game.Scene.GetComponent<UIComponent>().Create("NiuNiuMain");
            }
            else if (response.Error == -1)
            {
                Debug.Log(response.Message);
            }
            else if (response.Error == -2)
            {
                Debug.Log(response.Message);
            }
        }
    }
}