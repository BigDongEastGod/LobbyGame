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
            // 挂在Player组件，给Actor使用

            var sessionGatePlayer = session.GetComponent<SessionGatePlayerComponent>();

            if (sessionGatePlayer == null) return;

            var playerComponent = sessionGatePlayer.AddComponent<Player>();
            
            playerComponent.Id = accountId;
            
            playerComponent.UnitId = accountId;

            self.Sessions.Add(session);
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
            return self.Sessions.FirstOrDefault(d => d.GetComponent<SessionGatePlayerComponent>()?.GetComponent<Player>()?.Id == accountId);
        }

        /// <summary>
        /// 根据玩家ID获取Session
        /// </summary>
        /// <param name="self"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static Session GetSession(this GatePlayerManageComponent self, Session session)
        {
            return self.Sessions.FirstOrDefault(d => d.Id == session.Id);
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
            if (!self.Sessions.Contains(session)) return;
            
            if (session.IsDisposed == false) session.Dispose();

            self.Sessions.Remove(session);

            Log.Debug("玩家数量 ：" + self.Sessions.Count);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountId"></param>
        public static void Remove(this GatePlayerManageComponent self, long accountId)
        {
            self.Remove(self.GetSession(accountId));
        }

        #endregion
    }
}