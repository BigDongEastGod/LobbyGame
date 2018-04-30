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

    public class NiuNiuCRComponent : Component
    {
        private UI _nnLobby;
        private UI _nnOptionsTspx;
        private UI _nnOptionsGjxx;

        private GameObject _roomPeople6;
        private GameObject _optionsLayout;
        private NiuNiuRule _curretNiuNiuRule;

        private GameObject _createRoomBtn;

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
//            var _roomPeople8 = rc.Get<GameObject>("roomPeople_8");
            _optionsLayout = rc.Get<GameObject>("OptionsLayout");
            var toggleBtn = rc.Get<GameObject>("ToggleBtn");
            _createRoomBtn = rc.Get<GameObject>("CreateRoomBtn");

            #endregion

            _curretNiuNiuRule = NiuNiuRuleInstance.NiuNiuShangZhuang;

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
                        RegChangeModeBtn(toggle, NiuNiuRuleInstance.NiuNiuShangZhuang);
                        break;
                    case "GuDingZhuangJiaTg":
                        RegChangeModeBtn(toggle, NiuNiuRuleInstance.GuDingZhuangJia);
                        break;
                    case "ZiYouQiangZhuangTg":
                        RegChangeModeBtn(toggle, NiuNiuRuleInstance.ZiYouQiangZhuang);
                        break;
                    case "MingPaiQiangZhuangTg":
                        RegChangeModeBtn(toggle, NiuNiuRuleInstance.MingPaiQiangZhuang);
                        break;
                    case "TongBiNiuNiuTg":
                        RegChangeModeBtn(toggle, NiuNiuRuleInstance.TongBiNiuNiu);
                        break;
                }
            }

            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(_createRoomBtn.GetComponent<Button>(), CreatePaiJu);
        }

        public void Start()
        {
            _nnLobby = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuLobby);

            Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuTspx, _optionsLayout.transform.Find("Row5/TeShuPaiXing/OptionsPos"));
            Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuGjxx, _optionsLayout.transform.Find("Row6/GaoJiXuanXiang/OptionsPos"));

            _nnOptionsTspx = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuTspx);
            _nnOptionsGjxx = Game.Scene.GetComponent<UIComponent>().Get(UIType.NiuNiuGjxx);
        }

        /// <summary>
        /// 注册改变模式按钮
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="nnRule"></param>
        private void RegChangeModeBtn(Transform toggle, NiuNiuRule nnRule)
        {
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(toggle.GetComponent<Button>(), () =>
            {
                // 切换显示标签 Background Checkmark
                ChangeToggleMark(toggle);
                // 切换规则内容显示
                _curretNiuNiuRule = nnRule;
                _nnOptionsGjxx.GetComponent<NNGjxxComponent>().RefreshShowItem(_curretNiuNiuRule.ListGaoJiXuanXiang.Count);
                _nnOptionsTspx.GetComponent<NNTspxComponent>().RefreshShowItem(_curretNiuNiuRule.ListTeShuPaiXing.Count);
                ShowRule(_curretNiuNiuRule);
            });
        }

        /// <summary>
        /// 改变Toggle显示
        /// </summary>
        /// <param name="self">Toggle自身的Transform</param>
        private void ChangeToggleMark(Transform self)
        {
            foreach (Transform toggle in self.parent)
            {
                toggle.Find("Background").gameObject.SetActive(true);
                toggle.Find("Checkmark").gameObject.SetActive(false);
            }

            self.Find("Background").gameObject.SetActive(false);
            self.Find("Checkmark").gameObject.SetActive(true);
        }

        /// <summary>
        /// 显示牛牛对应规则
        /// </summary>
        /// <param name="nnRule">牌局模式</param>
        private void ShowRule(NiuNiuRule nnRule)
        {
            foreach (Transform row in _optionsLayout.transform)
            {
                switch (row.name)
                {
                    case "Row1":
                        SetDropDownList(row, nnRule, NnDpType.DiFen);
                        SetDropDownList(row, nnRule, NnDpType.JuShu);
                        break;
                    case "Row2":
                        SetDropDownList(row, nnRule, NnDpType.FangFei);
                        SetDropDownList(row, nnRule, NnDpType.ZiDongKaiZhuo);
                        break;
                    case "Row3":
                        int showCount = 0;
                        if (nnRule.ListXianJiaTuiZhu == null)
                        {
                            row.Find(NnDpType.XianJiaTuiZhu).gameObject.SetActive(false);
                        }
                        else
                        {
                            showCount++;
                            row.Find(NnDpType.XianJiaTuiZhu).gameObject.SetActive(true);
                            SetDropDownList(row, nnRule, NnDpType.XianJiaTuiZhu);
                        }

                        if (nnRule.ListShangZhuangFenShu == null)
                            row.Find(NnDpType.ShangZhuangFenShu).gameObject.SetActive(false);
                        else
                        {
                            showCount++;
                            row.Find(NnDpType.ShangZhuangFenShu).gameObject.SetActive(true);
                            SetDropDownList(row, nnRule, NnDpType.ShangZhuangFenShu);
                        }

                        if (nnRule.ListZuiDaQiangZhuang == null)
                            row.Find(NnDpType.ZuiDaQiangZhuang).gameObject.SetActive(false);
                        else
                        {
                            showCount++;
                            row.Find(NnDpType.ZuiDaQiangZhuang).gameObject.SetActive(true);
                            SetDropDownList(row, nnRule, NnDpType.ZuiDaQiangZhuang);
                        }

                        row.gameObject.SetActive(showCount > 0);

                        break;
                    case "Row4":
                        SetDropDownList(row, nnRule, NnDpType.FanBeiGuiZe);
                        break;
                }
            }
        }

        /// <summary>
        /// 设置规则下拉列表内容
        /// </summary>
        /// <param name="row">规则所在行</param>
        /// <param name="nnRule">牌局模式</param>
        /// <param name="option">选项</param>
        private void SetDropDownList(Transform row, NiuNiuRule nnRule, string option)
        {
            switch (option)
            {
                case NnDpType.DiFen:
                    Dropdown difenDp = row.Find($"{NnDpType.DiFen}/{NnDpType.DiFen}Dp").GetComponent<Dropdown>();
                    difenDp.ClearOptions();
                    difenDp.AddOptions(nnRule.ListDiFen);
                    break;
                case NnDpType.JuShu:
                    Dropdown juShuDp = row.Find($"{NnDpType.JuShu}/{NnDpType.JuShu}Dp").GetComponent<Dropdown>();
                    juShuDp.ClearOptions();
                    juShuDp.AddOptions(nnRule.ListjuShu);
                    break;
                case NnDpType.FangFei:
                    Dropdown fangfeiDp = row.Find($"{NnDpType.FangFei}/{NnDpType.FangFei}Dp").GetComponent<Dropdown>();
                    fangfeiDp.ClearOptions();
                    fangfeiDp.AddOptions(nnRule.ListFangFei);
                    break;
                case NnDpType.ZiDongKaiZhuo:
                    Dropdown zidongkaizhuoDp = row.Find($"{NnDpType.ZiDongKaiZhuo}/{NnDpType.ZiDongKaiZhuo}Dp").GetComponent<Dropdown>();
                    zidongkaizhuoDp.ClearOptions();
                    zidongkaizhuoDp.AddOptions(nnRule.ListZiDongKaiZhuo);
                    break;
                case NnDpType.XianJiaTuiZhu:
                    Dropdown xianjiatuizhuDp = row.Find($"{NnDpType.XianJiaTuiZhu}/{NnDpType.XianJiaTuiZhu}Dp").GetComponent<Dropdown>();
                    xianjiatuizhuDp.ClearOptions();
                    xianjiatuizhuDp.AddOptions(nnRule.ListXianJiaTuiZhu);
                    break;
                case NnDpType.ShangZhuangFenShu:
                    Dropdown shangzhuangfenshuDp = row.Find($"{NnDpType.ShangZhuangFenShu}/{NnDpType.ShangZhuangFenShu}Dp").GetComponent<Dropdown>();
                    shangzhuangfenshuDp.ClearOptions();
                    shangzhuangfenshuDp.AddOptions(nnRule.ListShangZhuangFenShu);
                    break;
                case NnDpType.ZuiDaQiangZhuang:
                    Dropdown zuidaqiangzhuangDp = row.Find($"{NnDpType.ZuiDaQiangZhuang}/{NnDpType.ZuiDaQiangZhuang}Dp").GetComponent<Dropdown>();
                    zuidaqiangzhuangDp.ClearOptions();
                    zuidaqiangzhuangDp.AddOptions(nnRule.ListZuiDaQiangZhuang);
                    zuidaqiangzhuangDp.value = 3;
                    break;
                case NnDpType.FanBeiGuiZe:
                    Dropdown fanbeiguizeDp = row.Find($"{NnDpType.FanBeiGuiZe}/{NnDpType.FanBeiGuiZe}Dp").GetComponent<Dropdown>();
                    fanbeiguizeDp.ClearOptions();
                    fanbeiguizeDp.AddOptions(nnRule.ListFanBeiGuiZe);
                    break;
            }
        }

        /// <summary>
        /// 创建牌局
        /// </summary>
        private async void CreatePaiJu()
        {
            // 创建房间
            var creatRoomResponse = (CreateRoomResponse) await SceneHelperComponent.Instance.Session.Call(
                new CreateRoomRequest() {RoomType = "NN"});

            if (creatRoomResponse.Error == 0)
            {
                Debug.Log("创建房间成功,房间号: " + creatRoomResponse.RoomId);

                SendRuleAndJoinRoom(creatRoomResponse.RoomId, true);
            }
//            else if (creatRoomResponse.Error == -1)
//            {
//                Debug.Log(creatRoomResponse.Message);
//            }
            else if (creatRoomResponse.Error == -2)
            {
                Debug.Log(creatRoomResponse.Message + ",房间ID:" + creatRoomResponse.RoomId);
                SendRuleAndJoinRoom(creatRoomResponse.RoomId, false);
            }
            else
            {
                Debug.Log(creatRoomResponse.Message);
            }
        }

        /// <summary>
        /// 发送规则,加入房间
        /// </summary>
        /// <param name="roomId">房间号</param>
        /// <param name="isJoin">是否加入房间</param>
        private async void SendRuleAndJoinRoom(long roomId, bool isJoin)
        {
            NNChess nnChess = GetCurrentNnChess();
            // 发送规则
            var roomRulesResponse = (RoomRulesResponse) await SceneHelperComponent.Instance.Session.Call(
                new RoomRulesRequest() {RoomId = roomId, Rules = ProtobufHelper.ToBytes(nnChess)});

            Debug.Log("nnChess.Score: " + nnChess.Score);
            Debug.Log("RoomRulesResponse Error code: " + roomRulesResponse.Error);
            Debug.Log("RoomRulesResponse Message: " + roomRulesResponse.Message);

            if (roomRulesResponse.Error == 0)
            {
                Debug.Log("发送规则成功");

                if (isJoin)
                {
                    var roomInfoResponse = (RoomInfoResponse) await SceneHelperComponent.Instance.Session.Call(
                        new RoomInfoRequest() {RoomId = roomId, Message = 0});
                    if (roomInfoResponse.Error == 0)
                    {
                        Debug.Log("加入房间成功,跳转至游戏主场景");

                        Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuLobby);
                        Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuMain, UiLayer.Bottom);
                    }
                    else
                    {
                        Debug.Log("加入房间失败: " + roomInfoResponse.Message);
                    }
                }
                else
                {
                    Debug.Log("已经在房间内,跳转至游戏主场景");

                    Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuLobby);
                    Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuMain, UiLayer.Bottom);
                }
            }
            else
            {
                Debug.Log("发送规则失败");
                Debug.Log(roomRulesResponse.Message);
            }
        }

        /// <summary>
        /// 得到当前选择的规则
        /// </summary>
        /// <returns></returns>
        private NNChess GetCurrentNnChess()
        {
            NNChess nnChess = new NNChess();

            int score = 0, dish = 0, roomRate = 0, playerPush = 0, autoGame = 0, doubleRules = 0;

            foreach (Transform row in _optionsLayout.transform)
            {
                switch (row.name)
                {
                    case "Row1":
                        score = row.Find($"{NnDpType.DiFen}/{NnDpType.DiFen}Dp").GetComponent<Dropdown>().value;
                        dish = row.Find($"{NnDpType.JuShu}/{NnDpType.JuShu}Dp").GetComponent<Dropdown>().value;
                        break;
                    case "Row2":
                        roomRate = row.Find($"{NnDpType.FangFei}/{NnDpType.FangFei}Dp").GetComponent<Dropdown>().value;
                        autoGame = row.Find($"{NnDpType.ZiDongKaiZhuo}/{NnDpType.ZiDongKaiZhuo}Dp").GetComponent<Dropdown>().value;
                        break;
                    case "Row3":
                        if (row.Find(NnDpType.XianJiaTuiZhu).gameObject.activeInHierarchy)
                        {
                            playerPush = row.Find($"{NnDpType.XianJiaTuiZhu}/{NnDpType.XianJiaTuiZhu}Dp").GetComponent<Dropdown>().value;
                        }

                        break;
                    case "Row4":
                        doubleRules = row.Find($"{NnDpType.FanBeiGuiZe}/{NnDpType.FanBeiGuiZe}Dp").GetComponent<Dropdown>().value;
                        break;
                }
            }

            nnChess.Score = _curretNiuNiuRule.Score[score];
            nnChess.Dish = _curretNiuNiuRule.Dish[dish];
            nnChess.RoomRate = _curretNiuNiuRule.RoomRate[roomRate];
            nnChess.PlayerPush = _curretNiuNiuRule.PlayerPush[playerPush];
            nnChess.AutoGame = _curretNiuNiuRule.AutoGame[autoGame];
            nnChess.DoubleRules = _curretNiuNiuRule.DoubleRules[doubleRules];


            nnChess.ShunZiRules = false;
            nnChess.TongHuaRules = false;
            nnChess.HuLuRules = false;
            nnChess.WuHuaRules = false;
            nnChess.ZhaDanRules = false;
            nnChess.WuXiaoRules = false;

            nnChess.ZhongTuJinRuRules = false;
            nnChess.CuoPaiRules = false;
            nnChess.WangLaiRules = false;

            nnChess.MaiMaRules = false;

            foreach (var selected in _nnOptionsTspx.GetComponent<NNTspxComponent>().SelectedOptions)
            {
                switch (selected)
                {
                    case 0:
                        nnChess.ShunZiRules = true;
                        break;
                    case 1:
                        nnChess.TongHuaRules = true;
                        break;
                    case 2:
                        nnChess.HuLuRules = true;
                        break;
                    case 3:
                        nnChess.WuHuaRules = true;
                        break;
                    case 4:
                        nnChess.ZhaDanRules = true;
                        break;
                    case 5:
                        nnChess.WuXiaoRules = true;
                        break;
                }
            }

            foreach (var selected in _nnOptionsGjxx.GetComponent<NNGjxxComponent>().SelectedOptions)
            {
                switch (selected)
                {
                    case 0:
                        nnChess.ZhongTuJinRuRules = true;
                        break;
                    case 1:
                        nnChess.CuoPaiRules = true;
                        break;
                    case 2:
                        nnChess.WangLaiRules = true;
                        break;
                    case 3:
                        nnChess.MaiMaRules = true;
                        break;
                }
            }

            nnChess.PlayerCount = _roomPeople6.GetComponent<Toggle>().isOn ? 6 : 8;

            return nnChess;
        }

        public void LockCreateBtn()
        {
            _createRoomBtn.GetComponent<Button>().enabled = false;
        }

        public void UnLockCreateBtn()
        {
            _createRoomBtn.GetComponent<Button>().enabled = true;
        }
    }
}