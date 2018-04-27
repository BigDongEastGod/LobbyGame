namespace ETModel
{
    public abstract class Room :ETModel.Entity
    {
        public virtual void AddRules(byte[] rules){}  // 添加规则
        
        public virtual void Prepare(SPlayer player){}  // 准备游戏
        
        public virtual void JionRoom(SPlayer player){}  // 加入房间

        public virtual void QuitRoom(SPlayer player){}  // 退出房间
        
        public virtual void StartGame(){} // 开始游戏

        public virtual void EndGame(){}  // 结束游戏

        public virtual void DissolveRoom(){} // 解散房间
    }
}