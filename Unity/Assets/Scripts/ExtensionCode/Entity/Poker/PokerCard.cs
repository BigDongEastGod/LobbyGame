using System;
using ProtoBuf;

namespace ETModel
{
    [ProtoContract]
    public class PokerCard
    {
        [ProtoMember(1, IsRequired = true)]
        public int CardNumber;
        [ProtoMember(2, IsRequired = true)]
        public int CardType;
        [ProtoMember(3, IsRequired = true)]
        public bool IsPayout;
    }
}