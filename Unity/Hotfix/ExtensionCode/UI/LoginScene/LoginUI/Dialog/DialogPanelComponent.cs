using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class DialogPanelComponentAwakeSystem : AwakeSystem<DialogPanelComponent>
    {
        public override void Awake(DialogPanelComponent self)
        {
            self.Awake();
        }
    }


    public class DialogPanelComponent : Component
    {
        private GameObject _dialogPanel;
        private GameObject _dialogBoxText;
        private GameObject _dialogOkBtn;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            // 弹窗提示
            _dialogPanel = rc.Get<GameObject>("DialogPanel");
            _dialogBoxText = rc.Get<GameObject>("DialogBoxText");
            _dialogOkBtn = rc.Get<GameObject>("DialogOKBtn");

            // 消息框确定按钮
            SceneHelperComponent.Instance.MonoEvent.AddButtonClick(_dialogOkBtn.GetComponent<Button>(),
                () =>
                {
                    _dialogPanel.SetActive(false);
                    _dialogBoxText.GetComponent<Text>().text = "";
                });
        }


        /// <summary>
        /// 消息提示框
        /// </summary>
        /// <param name="message"></param>
        public void ShowDialogBox(string message)
        {
            _dialogBoxText.GetComponent<Text>().text = message;
        }
    }
}