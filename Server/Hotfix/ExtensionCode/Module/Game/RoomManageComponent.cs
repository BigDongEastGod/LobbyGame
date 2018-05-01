using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CSharpx;
using ETModel;
using Model.ExtensionCode.DB;

namespace ETHotfix
{
    /// <summary>
    /// 房间组件扩展类
    /// </summary>
    public static class RoomManageComponentEx
    {
        /// <summary>
        /// 根据用户获取所有创建的房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static List<Room> GetRooms(this RoomManageComponent self, SPlayer player)
        {
            return self.Rooms.Where(d => d.Key == player.Id).Select(d => d.Value).ToList();
        }

        #region 获取玩家列表

        /// <summary>
        /// 获取玩家信息列表
        /// </summary>
        /// <param name="self"></param>
        /// <param name="room"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static async Task<List<AccountInfo>> GetRoomPlayers(this RoomManageComponent self, Room room,SPlayer player)
        {
            var accountInfos = new List<AccountInfo>();

            var playerKeys = room.Players.Where(d => d != player);
            
            foreach (var playersKey in playerKeys)
            {
                var account = await Game.Scene.GetComponent<DBProxyComponent>().Query<Account>(playersKey.Id);

                accountInfos.Add(new AccountInfo()
                {
                    UserName = account.UserName,
                    Password = null,
                    Diamond = account.Diamond,
                    Gold = account.Gold,
                    RegistrationTime = account.RegistrationTime.ToString(),
                    LoginTime = account.LoginTime.ToString()
                });
            }

            return accountInfos;
        }
        
        /// <summary>
        /// 获取客人信息列表
        /// </summary>
        /// <param name="self"></param>
        /// <param name="room"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static async Task<List<AccountInfo>> GetRoomGuests(this RoomManageComponent self, Room room,SPlayer player)
        {
            var accountInfos = new List<AccountInfo>();

            var playerKeys = room.Guest.Where(d => d != player);
            
            foreach (var playersKey in playerKeys)
            {
                var account = await Game.Scene.GetComponent<DBProxyComponent>().Query<Account>(playersKey.Id);

                accountInfos.Add(new AccountInfo()
                {
                    UserName = account.UserName,
                    Password = account.Password,
                    Diamond = account.Diamond,
                    Gold = account.Gold,
                    RegistrationTime = account.RegistrationTime.ToString(),
                    LoginTime = account.LoginTime.ToString()
                });
            }

            return accountInfos;
        }
        
        #endregion

        #region 获取房间
        
        /// <summary>
        /// 获取用户所在的房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="player">用户SPlayer</param>
        /// <returns></returns>
        public static Room GetRommByPlayer(this RoomManageComponent self,SPlayer player)
        {
            return self.Rooms.FirstOrDefault(d => d.Value.PlayerIsInRomm(player) != 0).Value;
        }
        
        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="playerId">玩家ID</param>
        /// <param name="roomType">房间类型</param>
        /// <returns></returns>
        public static Room Add(this RoomManageComponent self,long playerId,string roomType)
        {
            // 随机生成房间号
            
            var roomId = long.Parse(new Random(int.Parse(DateTime.Now.ToString("HHmmssfff"))).Next(100000, 999999).ToString());
            
            while (true)
            {
                if (self.GetRoom(roomId) == null) break;
                    
                roomId = long.Parse(new Random(int.Parse(DateTime.Now.ToString("HHmmssfff"))).Next(100000, 999999).ToString());
            }
            
            // 创建房间  

            var room = CreateRoom(roomType, roomId);

            if (room != null) self.Rooms.Add(playerId, room);

            return room;
        }

        /// <summary>
        /// 获取房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roomId">房间ID</param>
        /// <returns></returns>
        public static Room GetRoom(this RoomManageComponent self, long roomId)
        {
            var room = self.Rooms.FirstOrDefault(d => d.Value.Id == roomId);

            return room.Equals(default(KeyValuePair<long, Room>)) ? null : room.Value;
        }

        /// <summary>
        /// 获取房间创建者ID
        /// </summary>
        /// <param name="self"></param>
        /// <param name="room">房间</param>
        /// <returns></returns>
        public static long GetCreateRoomPlayerId(this RoomManageComponent self,Room room)
        {
            var player = self.Rooms.FirstOrDefault(d => d.Value == room);

            return player.Equals(default(KeyValuePair<long, Room>)) ? 0 : player.Key;
        }
        
        #endregion

        #region 删除房间

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="room">房间</param>
        public static void Remove(this RoomManageComponent self,Room room)
        {
            var removeroom = self.Rooms.FirstOrDefault(d => d.Value == room);

            if (removeroom.Equals(default(KeyValuePair<long, Room>))) return;
            
            removeroom.Value.DissolveRoom();

            self.Rooms.Remove(removeroom.Key);
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roomId">房间ID</param>
        public static void Remove(this RoomManageComponent self,long roomId)
        {
            var removeroom = self.Rooms.FirstOrDefault(d => d.Value.Id == roomId);

            if (removeroom.Equals(default(KeyValuePair<long, Room>))) return;

            self.Rooms.Remove(removeroom.Key);
        }

        #endregion

        #region 创建房间

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="roomType"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        private static Room CreateRoom(string roomType,long roomId)
        {
            Room room = null;
            
            switch (roomType)
            {
                case "NN":
                    room = ComponentFactory.CreateWithId<NNRoom>(roomId);
                    room.RoomType = RoomType.NN;
                    break;
                case "DDZ":
                    break;
                case "SXMZ":
                    break;
            }

            return room;
        }

        #endregion
    }
}