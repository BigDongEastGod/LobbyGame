using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    public static class GameTools
    {
        private static int _reloadingCount = 0;
        private static bool _startReloading = false;

        /// <summary>
        /// 显示提示框
        /// </summary>
        /// <param name="message">提醒消息内容</param>
        /// <param name="canvasTransform">当前UI所在画布的Transform</param>
        public static void ShowDialogMessage(string message, string canvasName, bool isQuit = false)
        {
            var dialogUI = Game.Scene.GetComponent<UIComponent>().Get(UIType.DialogPanel);

            dialogUI.GetComponent<DialogPanelComponent>().ShowDialogBox(message);

            Transform parent = GameObject.Find("Global/UI/" + canvasName + "/Top").transform;

            dialogUI.GameObject.transform.SetParent(parent);

            dialogUI.GameObject.SetActive(true);

            dialogUI.GetComponent<DialogPanelComponent>().IsQuit = isQuit;
        }

        /// <summary>
        /// 断线重连
        /// </summary>
        /// <param name="canvasName">当前的画布名称</param>
        public static async void ReLoading(string canvasName)
        {
            if (_startReloading)
            {
                return;
            }

            _startReloading = true;
            _reloadingCount++;

            if (_reloadingCount > 5)
            {
                ShowDialogMessage("重新连接失败!请重新登录再试!", canvasName, true);
                return;
            }

            var loadingUI = Game.Scene.GetComponent<UIComponent>().Get(UIType.LoadingPanel);

            Transform parent = GameObject.Find("Global/UI/" + canvasName + "/TopMost").transform;

            loadingUI.GameObject.transform.SetParent(parent);

            loadingUI.GameObject.SetActive(true);

            loadingUI.GetComponent<LoadingComponent>().SetText("正在尝试第" + _reloadingCount + "次重连,请稍候...");

            try
            {
                SceneHelperComponent.Instance.Session.Dispose();
                SceneHelperComponent.Instance.Session = null;

                var session = SceneHelperComponent.Instance.CreateRealmSession();

                var response = (LoginResponse) await session.Call(
                    new LoginRequest()
                    {
                        UserName = PlayerPrefs.GetString("username"),
                        Password = PlayerPrefs.GetString("password")
                    });

                if (response.Error == 0)
                {
                    session.Dispose();

                    Debug.Log("重连成功");
                    // 连接网关服务器
                    await SceneHelperComponent.Instance.CreateGateSession(response.Address, response.Key);

                    loadingUI.GameObject.SetActive(false);
                    _reloadingCount = 0;
                    _startReloading = false;
                }
                else
                {
                    // 重连失败
                    ShowDialogMessage(response.Message, canvasName);

                    await Task.Delay(2000);
                    ReLoading(canvasName);
                }
            }
            catch (Exception e)
            {
                _startReloading = false;
                ShowDialogMessage(e.Message, canvasName);
            }
        }

//        /// <summary>
//        /// 断线重连委托注册
//        /// </summary>
//        /// <param name="canvasName">要重连的CanvasName</param>
//        /// <param name="action">载入房间之前状态的函数</param>
//        public static void SetReloadDelegate(string canvasName, Action action)
//        {
//            Game.Scene.GetComponent<PingComponent>().PingBackCall = () =>
//            {
//                ReLoading(canvasName);
//                action.Invoke();
//            };
//        }
//
//        /// <summary>
//        /// 断线重连委托注册
//        /// </summary>
//        /// <param name="canvasName">要重连的CanvasName<</param>
//        public static void SetReloadDelegate(string canvasName)
//        {
//            Game.Scene.GetComponent<PingComponent>().PingBackCall = () => { ReLoading(canvasName); };
//        }
//
//        public static void RemoveReloadDelegate(Action action)
//        {
//            Game.Scene.GetComponent<PingComponent>().PingBackCall -= action;
//        }
    }
}