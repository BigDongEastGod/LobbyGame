﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class PingAwakeSystem : AwakeSystem<PingComponent, int, long>
    {
        public override void Awake(PingComponent self, int a, long b)
        {
            self.Awake(a, b);
        }
    }

    public class PingComponent : Component
    {
        private readonly Dictionary<long, long> _sessionTimes = new Dictionary<long, long>();

        public async void Awake(int waitTime, long overtime)
        {
            while (true)
            {
                try
                {
                    await Task.Delay(waitTime);

                    // 检查所有Session

                    for (var i = 0; i < _sessionTimes.Count; i++)
                    {
                        if ((TimeHelper.ClientNowSeconds() - _sessionTimes.ElementAt(i).Value) <= overtime) continue;
                        
                        GatePlayerManageComponent.Instance.Remove(_sessionTimes.ElementAt(i).Key);

                        RemoveSession(_sessionTimes.ElementAt(i).Key);
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
                UpdateSession(id);
            }
            else
            {
                _sessionTimes.Add(id, TimeHelper.ClientNowSeconds());
            }
        }

        public void RemoveSession(long id)
        {
            if (_sessionTimes.ContainsKey(id)) _sessionTimes.Remove(id);
        }

        public bool IsInSession(long id)
        {
            return _sessionTimes.ContainsKey(id);
        }

        public void UpdateSession(long id)
        {
            if (_sessionTimes.ContainsKey(id)) _sessionTimes[id] = TimeHelper.ClientNowSeconds();
        }

        public override void Dispose()
        {
            if (IsDisposed) return;

            base.Dispose();

            _sessionTimes.Clear();
        }
    }
}