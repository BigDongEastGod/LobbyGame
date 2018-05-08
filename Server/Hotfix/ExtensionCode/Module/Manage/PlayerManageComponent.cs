using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class PlayerManageAwakeSystem : AwakeSystem<PlayerManageComponent>
    {
        public override void Awake(PlayerManageComponent self)
        {
            self.Awake();
        }
    }

    public class PlayerManageComponent : Component
    {
        public List<SPlayer> Players;

        public static PlayerManageComponent Instance;

        public void Awake()
        {
            Players = new List<SPlayer>();

            Instance = this;
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            Players.ForEach(d => d.Dispose());

            this.Players.Clear();
        }

        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="gateSessionId"></param>
        /// <returns></returns>
        public async Task<SPlayer> Add(long accountId, long gateSessionId)
        {
            var player = Players.FirstOrDefault(d => d.Id == accountId);

            if (player == null)
            {
                player = ComponentFactory.CreateWithId<SPlayer>(accountId);

                Players.Add(player);
            }
            else
            {
                // 删除Actor相关组件

                await RemoveActor(player);
            }

            player.IsActivity = true;

            player.GateSessionId = gateSessionId;

            return player;
        }

        public SPlayer GetPlayer(long accountId)
        {
            return Players.FirstOrDefault(d => d.Id == accountId);
        }

        public void RemovePlayer(SPlayer player)
        {
            RemovePlayer(player.Id);
        }

        public void RemovePlayer(long id)
        {
            var player = Players.FirstOrDefault(d => d.Id == id);

            if (player == null) return;

            player.Dispose();

            Players.Remove(player);
        }

        /// <summary>
        /// 删除Actor相关操作
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task RemoveActor(SPlayer player)
        {
            // 删除Actor

            Game.Scene.GetComponent<ActorProxyComponent>().Remove(player.GateSessionId);

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
        /// <param name="player">SPlayer</param>
        /// <returns>如果在一定时间内没有更新该SPlayer就删除</returns>
        public async Task Remove(SPlayer player)
        {
            Log.Debug("准备删除");
            
            // 删除Actor相关组件

            await RemoveActor(player);

            // 用户管理里删除

            RemovePlayer(player);

            // 找到用户所在房间、如果有就退出该房间

            RoomManageComponent.Instance.GetRommByPlayer(player)?.QuitRoom(player);
            
            // Dispose

            if (player.IsDisposed == false) player.Dispose();

            Log.Debug("删除成功");
        }
    }
}