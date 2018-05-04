using System;
using UnityEngine;

namespace ETHotfix
{
    public static class GameTools
    {
        
        /// <summary>
        /// 显示提示框
        /// </summary>
        /// <param name="message">提醒消息内容</param>
        /// <param name="canvasTransform">当前UI所在画布的Transform</param>
        public static void ShowDialogMessage(string message, string canvasName)
        {
            var dialogUI = Game.Scene.GetComponent<UIComponent>().Get(UIType.DialogPanel);
            
            dialogUI.GetComponent<DialogPanelComponent>().ShowDialogBox(message);

            Transform parent = GameObject.Find("Global/UI/" + canvasName + "/Top").transform;
            
            dialogUI.GameObject.transform.SetParent(parent);

            dialogUI.GameObject.SetActive(true);
        }

        /// <summary>
        /// 断线重连
        /// </summary>
        /// <param name="canvasName">当前的画布名称</param>
        public static async void ReLoading(string canvasName)
        {
            var loadingUI = Game.Scene.GetComponent<UIComponent>().Get(UIType.LoadingPanel);
            
            Transform parent = GameObject.Find("Global/UI/" + canvasName + "/TopMost").transform;
            
            loadingUI.GameObject.transform.SetParent(parent);
            
            loadingUI.GameObject.SetActive(true);
            
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

                    Debug.Log("重连成功");

                    loadingUI.GameObject.SetActive(false);
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