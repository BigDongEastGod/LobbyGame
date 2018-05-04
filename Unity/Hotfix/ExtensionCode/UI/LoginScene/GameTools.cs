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

            Transform parent = GameObject.Find("Global/UI/" + canvasName + "/TopMost").transform;
            dialogUI.GameObject.transform.SetParent(parent);

            dialogUI.GameObject.SetActive(true);
        }

        public static void ShowLoadingPanel(string uiType, string canvasName)
        {
            
        }
        
        
    }
}