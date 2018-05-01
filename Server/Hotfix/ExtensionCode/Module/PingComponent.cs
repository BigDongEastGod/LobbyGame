using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        if ((TimeHelper.ClientNowSeconds() - _sessionTimes.ElementAt(i).Value) > overtime)
                        {
                            var session = GatePlayerManageComponent.Instance.GetSession(_sessionTimes.ElementAt(i).Key);

                            if (session != null)
                            {
                                Log.Debug("有用户退出");
                            
                                try
                                {
                                    // 删除Actor

                                    // 获取GatePlayer组件，并发送给注册的Actor服务器下线消息

                                    var player = GatePlayerManageComponent.Instance.GatePlayer(session);

                                    if (player != null)
                                    {
                                        player.ActorSession?.Send(new ActorQuitRequest() {AccountId = player.Id, ActorId = player.UnitId});

                                        // 删除Actor
                
                                        Game.Scene.GetComponent<ActorProxyComponent>().Remove(player.UnitId);
                                    }
                
                                    Game.Scene.GetComponent<ActorManagerComponent>().Remove(session.Id);
                
                                    Game.Scene.GetComponent<NetOuterComponent>().Remove(session.Id);
                
                                    GatePlayerManageComponent.Instance.Remove(session); 
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e);
                                }
                            }

                            RemoveSession(_sessionTimes.ElementAt(i).Key);
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
            return this._sessionTimes.TryGetValue(id, out var session);
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