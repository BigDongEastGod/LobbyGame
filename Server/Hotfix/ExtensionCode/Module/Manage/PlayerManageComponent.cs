using System.Linq;
using System.Threading.Tasks;
using CSharpx;
using ETModel;

namespace ETHotfix
{
    public static class PlayerManageComponentEx
    {
        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountId"></param>
        /// <param name="gateSessionId"></param>
        /// <returns></returns>
        public static async Task<SPlayer> Add(this PlayerManageComponent self, long accountId,long gateSessionId)
        {
            var player = self.Players.FirstOrDefault(d => d.Id == accountId);

            if (player == null)
            {
                player = ComponentFactory.CreateWithId<SPlayer>(accountId);

                self.Players.Add(player);
            }
            else
            {
                // 删除Actor相关组件
                
                await self.RemoveActor(player);
            }
Log.Debug("000000000000");
            player.IsActivity = true;

            player.GateSessionId = gateSessionId;

            return player;
        }

        public static SPlayer GetPlayer(this PlayerManageComponent self, long accountId)
        {
            return self.Players.FirstOrDefault(d => d.Id == accountId);
        }

        public static void RemovePlayer(this PlayerManageComponent self,SPlayer player)
        {
            self.RemovePlayer(player.Id);
        }
        
        public static void RemovePlayer(this PlayerManageComponent self,long id)
        {
            var player = self.Players.FirstOrDefault(d => d.Id == id);

            if (player == null) return;

            player.Dispose();
                
            self.Players.Remove(player);
        }

        /// <summary>
        /// 删除Actor相关操作
        /// </summary>
        /// <param name="self"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static async Task RemoveActor(this PlayerManageComponent self, SPlayer player)
        {
            // 删除Actor

            Game.Scene.GetComponent<ActorProxyComponent>().Remove(player.GateSessionId);

            // 删除在Actor管理组件中

            Game.Scene.GetComponent<ActorManagerComponent>().Remove(player.Id);
                
            // 删除Gate服务器上的位置
                
            await Game.Scene.GetComponent<LocationProxyComponent>().Remove(player.GateSessionId);
                
            // 删除在Location的注册

            if (player.GetComponent<ActorComponent>() != null)
            {
                await player.GetComponent<ActorComponent>().RemoveLocation();
                    
                // 移除ActorComponent组件
                
                player.RemoveComponent<ActorComponent>();
            }
        }


        /// <summary>
        /// 删除SPlayer
        /// </summary>
        /// <param name="self"></param>
        /// <param name="player">SPlayer</param>
        /// <param name="waitTime">等待时间</param>
        /// <returns>如果在一定时间内没有更新该SPlayer就删除</returns>
        public static async Task Remove(this PlayerManageComponent self, SPlayer player, int waitTime = 10000)
        {
            Log.Debug("用户ID：" + player.Id + " 延时" + waitTime + "毫秒后删除");

            player.IsActivity = false;

            await Task.Delay(waitTime);

            Log.Debug("准备删除");

            if (player.IsActivity == false)
            {
                // 删除Actor相关组件
                
                await self.RemoveActor(player);
                
                // 用户管理里删除
                
                self.RemovePlayer(player);
                
                // 找到用户所在房间、如果有就退出该房间

                RoomManageComponent.Instance.GetRommByPlayer(player)?.QuitRoom(player);

                if (player.IsDisposed == false) player.Dispose();

                Log.Debug("删除成功");
            }
            else
            {
                Log.Debug("用户ID：" + player.Id + "撤销删除、因为用户登录了");
            }
        }
    }
}