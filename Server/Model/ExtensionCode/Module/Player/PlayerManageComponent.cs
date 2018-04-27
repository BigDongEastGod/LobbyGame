using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace ETModel
{
    [ObjectSystem]
    public class PlayerManageAwakeSystem : AwakeSystem<PlayerManageComponent>
    {
        public override void Awake(PlayerManageComponent self)
        {
            self.Awake();
        }
    }
    
    public class PlayerManageComponent: Component
    {
        private List<SPlayer> _players;

        public static PlayerManageComponent Instance;

        public void Awake()
        {
            _players = new List<SPlayer>();
            
            Instance = this;
        }
        
        public SPlayer Add(long accountId)
        {
            var player = ComponentFactory.CreateWithId<SPlayer>(accountId);
            
            _players.Add(player);
            
            return player;
        }

        public SPlayer GetPlayer(long accountId)
        {
            return _players.FirstOrDefault(d => d.Id == accountId);
        }
        
        public long? GetAccountId(SPlayer player)
        {
            return _players.FirstOrDefault(d => d == player)?.Id;
        }

        public void RemovePlayer(SPlayer player)
        {
            RemovePlayer(player.Id);
        }
        
        public void RemovePlayer(long id)
        {
            var player = _players.FirstOrDefault(d => d.Id == id);

            if (player == null) return;
            
            player.Dispose();
                
            _players.Remove(player);
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
            
            _players.ForEach(d=>d.Dispose());

            this._players.Clear();
        }
    }
}