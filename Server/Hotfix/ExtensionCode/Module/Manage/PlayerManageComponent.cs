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
        /// <returns></returns>
        public static SPlayer Add(this PlayerManageComponent self, long accountId)
        {
            var player = self.Players.FirstOrDefault(d => d.Id == accountId);

            if (player == null)
            {
                player = ComponentFactory.CreateWithId<SPlayer>(accountId);

                self.Players.Add(player);
            }

            player.IsActivity = true;

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
        /// 删除SPlayer
        /// </summary>
        /// <param name="self"></param>
        /// <param name="player">SPlayer</param>
        /// <param name="waitTime">等待时间</param>
        /// <returns>如果在一定时间内没有更新该SPlayer就删除</returns>
        public static async Task Remove(this PlayerManageComponent self, SPlayer player, int waitTime = 10000)
        {
            Log.Debug("已经做到删除标记");

            player.IsActivity = false;

            await Task.Delay(waitTime);

            Log.Debug("准备删除");

            if (player.IsActivity == false)
            {
                // 删除Actor

                Game.Scene.GetComponent<ActorProxyComponent>().Remove(player.GateSessionId);

                // 删除在Actor管理组件中

                Game.Scene.GetComponent<ActorManagerComponent>()?.Remove(player.Id);
                
                await Game.Scene.GetComponent<LocationProxyComponent>().Remove(player.GateSessionId);
                
                // 删除在Location的注册

                await player.GetComponent<ActorComponent>().RemoveLocation();
                
                self.RemovePlayer(player);

                player.Dispose();

                Log.Debug("删除成功");
            }
            else
            {
                Log.Debug("未能删除成功、因为用户登录了");
            }
        }
    }
}