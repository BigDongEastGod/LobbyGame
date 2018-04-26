using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETModel
{
    [ObjectSystem]
    public class PingAwakeSystem : AwakeSystem<PingComponent, long, long>
    {
        public override void Awake(PingComponent self, long a, long b)
        {
            self.Awake(a, b);
        }
    }

    public class PingComponent : Component
    {
        private readonly Dictionary<long, long> _sessionTimes = new Dictionary<long, long>();

        public async void Awake(long waitTime, long overtime)
        {
            var timerComponent = Game.Scene.GetComponent<TimerComponent>();

            while (true)
            {
                try
                {
                    Log.Info("在线人数 ：" + _sessionTimes.Count.ToString());

                    await timerComponent.WaitAsync(waitTime);

                    // 检查所有Session

                    for (var i = 0; i < _sessionTimes.Count; i++)
                    {
                        if ((TimeHelper.ClientNowSeconds() - _sessionTimes.ElementAt(i).Value) > overtime)
                        {
                            GatePlayerManageComponent.Instance.RemoveSession(_sessionTimes.ElementAt(i).Key);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
        }

        public void AddSession(long id)
        {
            if (IsInSession(id))
            {
                UndateSession(id);
            }
            else
            {
                _sessionTimes.Add(id, TimeHelper.ClientNowSeconds());
            }
        }

        public void RemoveSession(long id)
        {
            if(_sessionTimes.ContainsKey(id)) _sessionTimes.Remove(id);
        }

        public bool IsInSession(long id)
        {
            return this._sessionTimes.TryGetValue(id, out var session);
        }

        public void UndateSession(long id)
        {
            if (_sessionTimes.ContainsKey(id)) _sessionTimes[id] = TimeHelper.ClientNowSeconds();
        }
    }
}