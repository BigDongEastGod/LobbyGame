namespace ETModel
{
	[ObjectSystem]
	public class PlayerSystem : AwakeSystem<Player, string>
	{
		public override void Awake(Player self, string a)
		{
			self.Awake(a);
		}
	}

	public sealed class Player : Entity
	{
		public string Account { get; private set; }
		
		public long UnitId { get; set; }

		public void Awake(string account)
		{
			this.Account = account;
		}
		
		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();
		}

		/// <summary>
		/// 获取注册的Actor服务器的Session
		/// </summary>
		public Session ActorSession => GetActorProxy == null ? null : Game.Scene.GetComponent<NetInnerComponent>().Get(GetActorProxy.Address);

		/// <summary>
		/// 获取ActorProxy
		/// </summary>
		public ActorProxy GetActorProxy => Game.Scene.GetComponent<ActorProxyComponent>().Get(this.UnitId);
	}
}