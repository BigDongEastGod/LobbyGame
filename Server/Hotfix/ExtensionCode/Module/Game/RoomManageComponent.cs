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
            return self.Rooms.Where(d => d.PlayerId == player.Id).Select(d => d.Room).ToList();
        }

        /// <summary>
        /// 获取玩家信息列表
        /// </summary>
        /// <param name="self"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        public static async Task<List<AccountInfo>> GetRoomPlayers(this RoomManageComponent self, Room room)
        {
            var accountInfos = new List<AccountInfo>();
            
            foreach (var playersKey in room.Players.Keys)
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
        
        /// <summary>
        /// 获取用户所在的房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="player">用户SPlayer</param>
        /// <returns></returns>
        public static RoomManageComponent.RoomModel GetRommByPlayer(this RoomManageComponent self,SPlayer player)
        {
            return self.Rooms.FirstOrDefault(d => d.Room.PlayerIsInRomm(player) != 0);
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

            if (room != null) self.Rooms.Add(new RoomManageComponent.RoomModel() {PlayerId = playerId, Room = room});

            return room;
        }

        /// <summary>
        /// 获取房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roomId">房间ID</param>
        /// <returns></returns>
        public static RoomManageComponent.RoomModel GetRoom(this RoomManageComponent self,long roomId)
        {
            return self.Rooms.FirstOrDefault(d => d.Room.Id == roomId);
        }

        /// <summary>
        /// 获取房间ID
        /// </summary>
        /// <param name="self"></param>
        /// <param name="room">房间</param>
        /// <returns></returns>
        public static long GetRoomId(this RoomManageComponent self,Room room)
        {
            return self.Rooms.FirstOrDefault(d => d.Room == room)?.Room.Id ?? 0;
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="room">房间</param>
        public static void Remove(this RoomManageComponent self,Room room)
        {
            var removeroom = self.Rooms.FirstOrDefault(d => d.Room == room);

            if (removeroom == null) return;
            
            removeroom.Room.DissolveRoom();

            self.Rooms.Remove(removeroom);
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="roomId">房间ID</param>
        public static void Remove(this RoomManageComponent self,long roomId)
        {
            var removeroom = self.Rooms.FirstOrDefault(d => d.Room.Id == roomId);

            if (removeroom == null) return;

            self.Rooms.Remove(removeroom);
        }
        
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
                    break;
                case "DDZ":
                    break;
                case "SXMZ":
                    break;
            }

            return room;
        }
    }
}