using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using ETModel;
using MongoDB.Driver;

namespace ETHotfix
{
    public static class GatePlayerManageComponentEx
    {
        #region 添加用户到Session列表里

        /// <summary>
        /// 添加用户到Session列表里
        /// </summary>
        /// <param name="self"></param>
        /// <param name="session">Session</param>
        /// <param name="accountId">玩家ID</param>
        /// <returns></returns>
        public static void Add(this GatePlayerManageComponent self, Session session, long accountId)
        {
            // 如果有跟用户ID想关联的Session、就删除该Session

            if (self.Sessions.TryGetValue(accountId,out var playerSession))
            {
                self.Players.Remove(playerSession.Id);
                
                self.Sessions.Remove(accountId);
                
                playerSession.Dispose();
            }
            
            // 创建Player
            
            var player = ComponentFactory.CreateWithId<Player>(accountId);

            player.UnitId = accountId;
            
            self.Players.Add(session.Id, player);

            self.Sessions.Add(accountId, session);
            
            // 挂在Player组件，给Actor使用

            var playerComponent = session.AddComponent<Player>();
            
            playerComponent.UnitId = accountId;
        }

        #endregion

        #region 获取信息

        /// <summary>
        /// 根据玩家ID获取Session
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static Session GetSession(this GatePlayerManageComponent self, long accountId)
        {
            self.Sessions.TryGetValue(accountId, out var session);

            return session;
        }

        /// <summary>
        /// 根据Session获取玩家
        /// </summary>
        /// <param name="self"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static Player GatePlayer(this GatePlayerManageComponent self, Session session)
        {
            self.Players.TryGetValue(session.Id, out var player);

            return player;
        }

        #endregion

        #region 移除操作

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="self"></param>
        /// <param name="session"></param>
        public static void Remove(this GatePlayerManageComponent self, Session session)
        {
            if (!self.Players.TryGetValue(session.Id, out var player)) return;
            
            self.Sessions[player.Id].Dispose();
            
            self.Sessions.Remove(player.Id);
            
            self.Players.Remove(session.Id);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountId"></param>
        /// <param name="isRemovePlayer"></param>
        public static void Remove(this GatePlayerManageComponent self, long accountId, bool isRemovePlayer = true)
        {
            if (!self.Sessions.TryGetValue(accountId, out var session)) return;

            if (isRemovePlayer) self.Players.Remove(session.Id);

            session.Dispose();

            self.Sessions.Remove(accountId);
        }

        #endregion
    }
}