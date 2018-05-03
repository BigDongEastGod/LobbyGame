using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class PingAwakeSystem : AwakeSystem<PingComponent, long, SessionWrap, Action>
    {
        public override void Awake(PingComponent self, long a, SessionWrap s, Action b)
        {
            self.Awake(a, s, b);
        }
    }
    
    public class PingComponent: Component
    {
        #region 成员变量

        /// <summary>
        /// 发送时间
        /// </summary>
        private SessionWrap _session;

        /// <summary>
        /// 发送时间
        /// </summary>
        private long _sendTimer;

        /// <summary>
        /// 接收时间
        /// </summary>
        private long _receiveTimer;

        /// <summary>
        /// 延时
        /// </summary>
        public long Ping = 0;

        private bool IsRun = false;

        /// <summary>
        /// 心跳协议包
        /// </summary>
        private readonly PingRequest _request = new PingRequest();

        #endregion

        #region Awake

        public async void Awake(long waitTime, SessionWrap _sessionWrap, Action action)
        {
            var timerComponent = ETModel.Game.Scene.GetComponent<TimerComponent>();

            this._session = _sessionWrap;

            IsRun = true;

            while (IsRun)
            {
                try
                {
                    if (this._session == null)
                    {
                        // 执行断线后的操作

                        action?.Invoke();

                        Debug.Log("断线了");
                        
                        break;
                    }

                    _sendTimer = TimeHelper.ClientNowSeconds();

                    await _session.Call(_request);

                    _receiveTimer = TimeHelper.ClientNowSeconds();

                    // 计算延时

                    Ping = ((_receiveTimer - _sendTimer) / 2) < 0 ? 0 : (_receiveTimer - _sendTimer) / 2;
                }
                catch (Exception e)
                {
                    // 执行断线后的操作

                    action?.Invoke();

                    Debug.Log("断线了");
                }

                await timerComponent.WaitAsync(waitTime);
            }
        }

        #endregion

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            IsRun = false;

            _session?.Dispose();

            _session = null;
        }
    }
}