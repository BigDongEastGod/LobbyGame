using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace ETModel
{
    [ObjectSystem]
    public class GatePlayerManageAwakeSystem : AwakeSystem<GatePlayerManageComponent>
    {
        public override void Awake(GatePlayerManageComponent self)
        {
            self.Awake();
        }
    }

    public class GatePlayerManageComponent : Component
    {
        private List<Session> _players;

        public static GatePlayerManageComponent Instance;

        public void Awake()
        {
            _players = new List<Session>();
            
            Instance = this;
        }
        
        public Player Add(Session session,long accountId)
        {
            Session playerSession = null;

            Player player = null;

            // 如果该用户已经登录，返回用户Player组件

            playerSession = _players.FirstOrDefault(d => d == session);

            if (playerSession != null)
            {
                player = playerSession.GetComponent<Player>() ?? playerSession.AddComponent<Player>();

                player.Id = accountId;

                player.UnitId = accountId;

                return player;
            }
            
            // 如果该用户已经登录，但不是本机登录、强制断开用户的连接
            
            _players.FirstOrDefault(d => d.GetComponent<Player>().Id == accountId && d != session)?.Dispose();
            
            // 添加用户到

            player = session.AddComponent<Player>();
            
            player.Id = accountId;

            player.UnitId = accountId;
            
            _players.Add(session);
            
            // 添加到心跳组件

            Game.Scene.GetComponent<PingComponent>().AddSession(session.Id);
            
            return player;
        }

        public Session GetSession(long accountId)
        {
            return _players.FirstOrDefault(d => d.GetComponent<Player>().Id == accountId);
        }
        
        public Player GatePlayer(Session session)
        {
            return _players.FirstOrDefault(d => d == session)?.GetComponent<Player>();
        }

        public long? GetAccountId(Session session)
        {
            return _players.FirstOrDefault(d => d == session)?.GetComponent<Player>().Id;
        }

        public void RemoveSession(Session session)
        {
            _players.Remove(session);

            Game.Scene.GetComponent<PingComponent>()?.RemoveSession(session.Id);
            
            session.Dispose();
        }

        public void RemoveSession(long accountId)
        {
            var session = _players.FirstOrDefault(d => d.GetComponent<Player>().Id == accountId);

            if (session == null) return;
                
            RemoveSession(session);
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            _players.ForEach(d => d.Dispose());

            this._players.Clear();
        }
    }
}