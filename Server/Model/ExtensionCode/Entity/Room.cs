using System.Collections.Generic;

namespace ETModel
{
    public enum RoomType
    {
        NN =0,
        DDZ =1
    }
    
    public abstract class Room : Entity
    {
        public readonly List<SPlayer> Players = new List<SPlayer>();

        public readonly List<SPlayer> Guest = new List<SPlayer>();
        
        public byte[] Rules;  // 房间规则

        public RoomType RoomType;  // 房间类型

        public virtual long PlayerIsInRomm(SPlayer player){return 0;}  // 判断用户是否在该房间中
        
        public virtual void AddRules(byte[] rules){}  // 添加规则
        
        public virtual string Prepare(SPlayer player){return null;}  // 准备游戏

        public virtual string JionRoom(SPlayer player){return null;}  // 加入房间

        public virtual void QuitRoom(SPlayer player){}  // 退出房间
        
        public virtual void StartGame(){} // 开始游戏

        public virtual void EndGame(){}  // 结束游戏

        public virtual void DissolveRoom(){} // 解散房间
    }
}