using System;
using UnityEngine;

namespace ETHotfix
{
    public static class GameTools
    {
        private static int _reloadingCount = 0;

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
            _reloadingCount++;

            if (_reloadingCount > 7)
            {
                ShowDialogMessage("重新连接失败!请重新登录再试!", canvasName, true);
            }

            var loadingUI = Game.Scene.GetComponent<UIComponent>().Get(UIType.LoadingPanel);

            Transform parent = GameObject.Find("Global/UI/" + canvasName + "/TopMost").transform;

            loadingUI.GameObject.transform.SetParent(parent);

            loadingUI.GameObject.SetActive(true);

            loadingUI.GetComponent<LoadingComponent>().SetText("正在尝试第" + _reloadingCount + "次重连,请稍候...");

            try
            {
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

                    // 连接网关服务器
                    await SceneHelperComponent.Instance.CreateGateSession(response.Address, response.Key);

                    loadingUI.GameObject.SetActive(false);
                    _reloadingCount = 0;
                }
                else
                {
                    // 重连失败
                    ShowDialogMessage(response.Message, canvasName);
                }
            }
            catch (Exception e)
            {
                ShowDialogMessage(e.Message, canvasName);
            }
        }
    }
}