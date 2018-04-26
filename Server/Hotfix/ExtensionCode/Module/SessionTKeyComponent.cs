using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    public class SessionTKeyComponent : Component
    {
        private readonly Dictionary<long, long> sessionKey = new Dictionary<long, long>();

        public long Add(long id)
        {
           return Add(RandomHelper.RandInt64(), id);
        }
        
        public long Add(long key, long id)
        {
            this.sessionKey.Add(key, id);
            
            this.TimeoutRemoveKey(key);

            return key;
        }

        public long Get(long key)
        {
            this.sessionKey.TryGetValue(key, out var id);
            
            return id;
        }

        public void Remove(long key)
        {
            this.sessionKey.Remove(key);
        }

        private async void TimeoutRemoveKey(long key)
        {
            await Game.Scene.GetComponent<TimerComponent>().WaitAsync(20000);
            
            this.sessionKey.Remove(key);
        }
    }
}