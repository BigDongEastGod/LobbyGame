using System.Collections.Generic;

namespace ETModel
{
    public abstract class Room :ETModel.Entity
    {
        public readonly Dictionary<SPlayer, bool> Players = new Dictionary<SPlayer, bool>();
        
        public byte[] Rules;

        public virtual long PlayerIsInRomm(SPlayer player){return 0;}  // 判断用户是否在该房间中
        
        public virtual void AddRules(byte[] rules){}  // 添加规则
        
        public virtual int Prepare(SPlayer player){return 0;}  // 准备游戏

        public virtual int JionRoom(SPlayer player){return 0;}  // 加入房间

        public virtual void QuitRoom(SPlayer player){}  // 退出房间
        
        public virtual void StartGame(){} // 开始游戏

        public virtual void EndGame(){}  // 结束游戏

        public virtual void DissolveRoom(){} // 解散房间
    }
}