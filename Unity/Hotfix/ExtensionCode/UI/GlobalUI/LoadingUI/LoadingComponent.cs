using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class LoadingComponentAwakeSystem : AwakeSystem<LoadingComponent>
    {
        public override void Awake(LoadingComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class LoadingComponentUpdateSystem : UpdateSystem<LoadingComponent>
    {
        public override void Update(LoadingComponent self)
        {
            self.Update();
        }
    }


    public class LoadingComponent : Component
    {
        public bool ReLoadingStart = false;
//        private int timeCount = 500;

        public void Awake()
        {
        }

        public void Update()
        {
//            if (timeCount <= 0)
//            {
//                GameTools.ShowDialogMessage("重新连接失败,请重新登录!", "LoginCanvas");
//                ReLoadingStart = false;
//                timeCount = 500;
//            }
//
//            if (ReLoadingStart)
//            {
//                timeCount--;
//            }
        }
    }
}