using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class HeadUIFormComponentAwakeSystem : AwakeSystem<HeadUIFormComponent>
    {
        public override void Awake(HeadUIFormComponent self)
        {
            self.Awake();
        }
    }
    
    
    public class HeadUIFormComponent:Component
    {
        public Image headImg;                  //头像
        public Text  userNameTxt;              //用户名
        public Text scoreTxt;                  //分数
        public Image qiangzhuangImg;           //抢庄
        public Image BetsTitleImg;             //下注
        public Text BetsTxt;                   //下注金币 
        public Image SelectImg;                //特效选择
        
        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            
            headImg = rc.Get<GameObject>("head").GetComponent<Image>();
            userNameTxt = rc.Get<GameObject>("uesrNameTxt").GetComponent<Text>();
            scoreTxt = rc.Get<GameObject>("scoreTxt").GetComponent<Text>();
            qiangzhuangImg = rc.Get<GameObject>("qiangzhuangImg").GetComponent<Image>();
            BetsTitleImg = rc.Get<GameObject>("BetsTitleImg").GetComponent<Image>();
            BetsTxt = BetsTitleImg.gameObject.transform.GetChild(0).GetComponent<Text>();
            SelectImg = rc.Get<GameObject>("SelectImg").GetComponent<Image>();
        }
    }
}