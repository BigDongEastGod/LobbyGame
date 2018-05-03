using ProtoBuf;
using ETModel;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
namespace ETHotfix
{
	[Message(HotfixOpcode.C2R_Login)]
	[ProtoContract]
	public partial class C2R_Login: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Account;

		[ProtoMember(2, IsRequired = true)]
		public string Password;

	}

	[Message(HotfixOpcode.R2C_Login)]
	[ProtoContract]
	public partial class R2C_Login: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Address;

		[ProtoMember(2, IsRequired = true)]
		public long Key;

	}

	[Message(HotfixOpcode.C2G_LoginGate)]
	[ProtoContract]
	public partial class C2G_LoginGate: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Key;

	}

	[Message(HotfixOpcode.G2C_LoginGate)]
	[ProtoContract]
	public partial class G2C_LoginGate: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long PlayerId;

	}

	[Message(HotfixOpcode.G2C_TestHotfixMessage)]
	[ProtoContract]
	public partial class G2C_TestHotfixMessage: IMessage
	{
		[ProtoMember(1, IsRequired = true)]
		public string Info;

	}

	[Message(HotfixOpcode.C2M_TestActorRequest)]
	[ProtoContract]
	public partial class C2M_TestActorRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Info;

	}

	[Message(HotfixOpcode.M2C_TestActorResponse)]
	[ProtoContract]
	public partial class M2C_TestActorResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Info;

	}

	[Message(HotfixOpcode.PlayerInfo)]
	[ProtoContract]
	public partial class PlayerInfo: IMessage
	{
	}

	[Message(HotfixOpcode.C2G_PlayerInfo)]
	[ProtoContract]
	public partial class C2G_PlayerInfo: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

	}

	[Message(HotfixOpcode.G2C_PlayerInfo)]
	[ProtoContract]
	public partial class G2C_PlayerInfo: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1)]
		public List<PlayerInfo> PlayerInfos = new List<PlayerInfo>();

	}

// 心跳包
	[Message(HotfixOpcode.PingRequest)]
	[ProtoContract]
	public partial class PingRequest: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

	}

	[Message(HotfixOpcode.PingResponse)]
	[ProtoContract]
	public partial class PingResponse: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

// 用户登录
	[Message(HotfixOpcode.LoginRequest)]
	[ProtoContract]
	public partial class LoginRequest: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string UserName;

		[ProtoMember(2, IsRequired = true)]
		public string Password;

	}

	[Message(HotfixOpcode.LoginResponse)]
	[ProtoContract]
	public partial class LoginResponse: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string Address;

		[ProtoMember(2, IsRequired = true)]
		public long Key;

	}

// 用户注册
	[Message(HotfixOpcode.RegisteredRequest)]
	[ProtoContract]
	public partial class RegisteredRequest: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string UserName;

		[ProtoMember(2, IsRequired = true)]
		public string Password;

	}

	[Message(HotfixOpcode.RegisteredResponse)]
	[ProtoContract]
	public partial class RegisteredResponse: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

// 登录Gate服务器
	[Message(HotfixOpcode.EntherGateRequest)]
	[ProtoContract]
	public partial class EntherGateRequest: IRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long Key;

	}

	[Message(HotfixOpcode.EntherGateResponse)]
	[ProtoContract]
	public partial class EntherGateResponse: IResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

// 获取用户信息
	[Message(HotfixOpcode.AccountInfo)]
	[ProtoContract]
	public partial class AccountInfo
	{
		[ProtoMember(1, IsRequired = true)]
		public string UserName;

		[ProtoMember(2, IsRequired = true)]
		public string Password;

		[ProtoMember(3, IsRequired = true)]
		public long Diamond;

		[ProtoMember(4, IsRequired = true)]
		public long Gold;

		[ProtoMember(5, IsRequired = true)]
		public long RoomId;

		[ProtoMember(6, IsRequired = true)]
		public string RegistrationTime;

		[ProtoMember(7, IsRequired = true)]
		public string LoginTime;

	}

	[Message(HotfixOpcode.GetAccountInfoRequest)]
	[ProtoContract]
	public partial class GetAccountInfoRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

	}

	[Message(HotfixOpcode.GetAccountInfoResponse)]
	[ProtoContract]
	public partial class GetAccountInfoResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public AccountInfo AccountInfo;

	}

// 创建游戏房间
	[Message(HotfixOpcode.CreateRoomRequest)]
	[ProtoContract]
	public partial class CreateRoomRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string RoomType;

	}

	[Message(HotfixOpcode.CreateRoomResponse)]
	[ProtoContract]
	public partial class CreateRoomResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long RoomId;

	}

// 牛牛棋牌规则
	[Message(HotfixOpcode.NNChess)]
	[ProtoContract]
	public partial class NNChess
	{
		[ProtoMember(1, IsRequired = true)]
		public string Score;

		[ProtoMember(2, IsRequired = true)]
		public int Dish;

		[ProtoMember(3, IsRequired = true)]
		public int RoomRate;

		[ProtoMember(4, IsRequired = true)]
		public int AutoGame;

		[ProtoMember(5, IsRequired = true)]
		public int PlayerPush;

		[ProtoMember(6)]
		public List<string> DoubleRules = new List<string>();

		[ProtoMember(7, IsRequired = true)]
		public bool ShunZiRules;

		[ProtoMember(8, IsRequired = true)]
		public bool TongHuaRules;

		[ProtoMember(9, IsRequired = true)]
		public bool HuLuRules;

		[ProtoMember(10, IsRequired = true)]
		public bool WuHuaRules;

		[ProtoMember(11, IsRequired = true)]
		public bool ZhaDanRules;

		[ProtoMember(12, IsRequired = true)]
		public bool WuXiaoRules;

		[ProtoMember(13, IsRequired = true)]
		public bool ZhongTuJinRuRules;

		[ProtoMember(14, IsRequired = true)]
		public bool CuoPaiRules;

		[ProtoMember(15, IsRequired = true)]
		public bool WangLaiRules;

		[ProtoMember(16, IsRequired = true)]
		public bool MaiMaRules;

		[ProtoMember(17, IsRequired = true)]
		public int PlayerCount;

	}

// 游戏房间规则
	[Message(HotfixOpcode.RoomRulesRequest)]
	[ProtoContract]
	public partial class RoomRulesRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long RoomId;

		[ProtoMember(2, IsRequired = true)]
		public byte[] Rules;

	}

	[Message(HotfixOpcode.RoomRulesResponse)]
	[ProtoContract]
	public partial class RoomRulesResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

// 加入游戏房间
	[Message(HotfixOpcode.JoinRoomRequest)]
	[ProtoContract]
	public partial class JoinRoomRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long RoomId;

	}

	[Message(HotfixOpcode.JoinRoomResponse)]
	[ProtoContract]
	public partial class JoinRoomResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

// 退出游戏房间
	[Message(HotfixOpcode.QuitRoomRequest)]
	[ProtoContract]
	public partial class QuitRoomRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long RoomId;

	}

	[Message(HotfixOpcode.QuitRoomResponse)]
	[ProtoContract]
	public partial class QuitRoomResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

// 准备游戏房间
	[Message(HotfixOpcode.PrepareGameRequest)]
	[ProtoContract]
	public partial class PrepareGameRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long RoomId;

	}

	[Message(HotfixOpcode.PrepareGameResponse)]
	[ProtoContract]
	public partial class PrepareGameResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

// 房间信息
	[Message(HotfixOpcode.RoomInfoRequest)]
	[ProtoContract]
	public partial class RoomInfoRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long RoomId;

	}

	[Message(HotfixOpcode.RoomInfoResponse)]
	[ProtoContract]
	public partial class RoomInfoResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long RoomId;

		[ProtoMember(2, IsRequired = true)]
		public byte[] Rules;

		[ProtoMember(3, IsRequired = true)]
		public string StartGameUserName;

		[ProtoMember(4)]
		public List<AccountInfo> Players = new List<AccountInfo>();

	}

// 开始房间游戏
	[Message(HotfixOpcode.StartGameRequest)]
	[ProtoContract]
	public partial class StartGameRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long RoomId;

	}

	[Message(HotfixOpcode.StartGameResponse)]
	[ProtoContract]
	public partial class StartGameResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

// 房间消息通告
// 提示消息 0:加入房间 1:坐下（准备） 2:离开房间 3:更改玩家开始游戏权限
	[Message(HotfixOpcode.RoomInfoAnnunciate)]
	[ProtoContract]
	public partial class RoomInfoAnnunciate: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string UserName;

		[ProtoMember(2, IsRequired = true)]
		public int Message;

	}

// 游戏消息通告
// 提示消息 0:下注 1:下注完成 2:离开房间 3:更改玩家开始游戏权限
	[Message(HotfixOpcode.GameInfoAnnunciate)]
	[ProtoContract]
	public partial class GameInfoAnnunciate: IActorMessage
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public string UserName;

		[ProtoMember(2, IsRequired = true)]
		public int Message;

		[ProtoMember(3, IsRequired = true)]
		public byte[] Arg;

	}

// 牛牛游戏下注
	[Message(HotfixOpcode.BetGameRequest)]
	[ProtoContract]
	public partial class BetGameRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(1, IsRequired = true)]
		public long RoomId;

		[ProtoMember(1, IsRequired = true)]
		public int Bet;

	}

	[Message(HotfixOpcode.BetGameResponse)]
	[ProtoContract]
	public partial class BetGameResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

	}

// 游戏房间
//message Room
//{
//required int32 RoomId = 1;  // 局数
//required string PlayingMethod = 2;  // 玩法
//}
// 游戏房间信息
	[Message(HotfixOpcode.RoomListRequest)]
	[ProtoContract]
	public partial class RoomListRequest: IActorRequest
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(93, IsRequired = true)]
		public long ActorId { get; set; }

		[ProtoMember(2, IsRequired = true)]
		public int Dish;

	}

	[Message(HotfixOpcode.RoomListResponse)]
	[ProtoContract]
	public partial class RoomListResponse: IActorResponse
	{
		[ProtoMember(90, IsRequired = true)]
		public int RpcId { get; set; }

		[ProtoMember(91, IsRequired = true)]
		public int Error { get; set; }

		[ProtoMember(92, IsRequired = true)]
		public string Message { get; set; }

		[ProtoMember(3)]
		public List<AccountInfo> Players = new List<AccountInfo>();

	}

}
