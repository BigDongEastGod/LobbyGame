using System;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class NiuNiuJoinComponentAwakeSystem : AwakeSystem<NiuNiuJoinRoomComponent>
    {
        public override void Awake(NiuNiuJoinRoomComponent self)
        {
            self.Awake();
        }
    }


    public class NiuNiuJoinRoomComponent : Component
    {
        private GameObject _nnjrShowNumber;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            // 加入房间
            var nnJoinRoom = rc.Get<GameObject>("NiuNiuJoinRoom");
            var nnjrInput = rc.Get<GameObject>("NNJR_Input");
            _nnjrShowNumber = rc.Get<GameObject>("NNJR_ShowNumber");
            var nnjrJoinBtn = rc.Get<GameObject>("NNJR_JoinBtn");
            var nnjrCloseBtn = rc.Get<GameObject>("NNJR_CloseBtn");


            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnjrCloseBtn.GetComponent<Button>(), () => { nnJoinRoom.SetActive(false); });
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnJoinRoom.GetComponent<Button>(), () => { nnJoinRoom.SetActive(false); });
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(nnjrJoinBtn.GetComponent<Button>(), JoinPaiJu);

            foreach (Transform numBtn in nnjrInput.transform)
            {
                switch (numBtn.name)
                {
                    case "NNJR_Number_1":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () => RefreshShowNumber(1));
                        break;
                    case "NNJR_Number_2":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () => RefreshShowNumber(2));
                        break;
                    case "NNJR_Number_3":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () => RefreshShowNumber(3));
                        break;
                    case "NNJR_Number_4":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () => RefreshShowNumber(4));
                        break;
                    case "NNJR_Number_5":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () => RefreshShowNumber(5));
                        break;
                    case "NNJR_Number_6":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () => RefreshShowNumber(6));
                        break;
                    case "NNJR_Number_7":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () => RefreshShowNumber(7));
                        break;
                    case "NNJR_Number_8":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () => RefreshShowNumber(8));
                        break;
                    case "NNJR_Number_9":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () => RefreshShowNumber(9));
                        break;
                    case "NNJR_Number_0":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () => RefreshShowNumber(0));
                        break;
                    case "NNJR_Resume":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () =>
                        {
                            foreach (Transform numberText in _nnjrShowNumber.transform)
                            {
                                numberText.GetChild(0).GetComponent<Text>().text = "";
                            }
                        });
                        break;
                    case "NNJR_Delete":
                        SceneHelperComponent.Instance.MonoEvent.AddButtonClick(numBtn.GetComponent<Button>(), () =>
                        {
                            for (int index = _nnjrShowNumber.transform.childCount - 1; index >= 0; index--)
                            {
                                if (!String.IsNullOrEmpty(_nnjrShowNumber.transform.GetChild(index).GetChild(0).GetComponent<Text>().text))
                                {
                                    _nnjrShowNumber.transform.GetChild(index).GetChild(0).GetComponent<Text>().text = "";
                                    break;
                                }
                            }
                        });
                        break;
                }
            }
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        private async void JoinPaiJu()
        {
            // TODO 加入牌局
            string paijuNumber = "";
            int count = 0;
            foreach (Transform numberText in _nnjrShowNumber.transform)
            {
                if (!String.IsNullOrEmpty(numberText.GetChild(0).GetComponent<Text>().text))
                {
                    paijuNumber += numberText.GetChild(0).GetComponent<Text>().text;
                    count++;
                }
            }

            if (count < 6)
            {
                GameTools.ShowDialogMessage("请输入6位房间号!","GameCanvas");
                return;
            }
            Debug.Log("加入房间:" + paijuNumber);
            
            var joinRoomResponse = (JoinRoomResponse) await SceneHelperComponent.Instance.Session.Call(
                new JoinRoomRequest() {RoomId = Convert.ToInt64(paijuNumber)});
            
            if (joinRoomResponse.Error == 0)
            {
                Debug.Log("加入房间成功,跳转至游戏主场景");

                Game.Scene.GetComponent<UIComponent>().Create(UIType.NiuNiuMain, UiLayer.Bottom, paijuNumber, false);
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.NiuNiuLobby);
            }
            else
            {
                Debug.Log("加入房间失败: " + joinRoomResponse.Message);
                GameTools.ShowDialogMessage("没有这个房间,请重新输入!", "GameCanvas");
            }
        }

        /// <summary>
        /// 刷新显示的房间号码
        /// </summary>
        private void RefreshShowNumber(int num)
        {
            foreach (Transform numberText in _nnjrShowNumber.transform)
            {
                if (String.IsNullOrEmpty(numberText.GetChild(0).GetComponent<Text>().text))
                {
                    numberText.GetChild(0).GetComponent<Text>().text = num.ToString();
                    break;
                }
            }
        }
    }
}