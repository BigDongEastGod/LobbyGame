﻿using System;
using System.Collections.Generic;
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
        [ProtoMember(4, IsRequired = true)]
        public int Grade;
    }

    [ProtoContract]
    public class PlayerPokerCards
    {
        [ProtoMember(1, IsRequired = true)]
        public int CardTypeNumber;
        
        [ProtoMember(2, IsRequired = true)]
        public int GradeCount;
        
        [ProtoMember(3, IsRequired = true)]
        public List<PokerCard> PokerCards;
    }
    
    // 游戏状态实体类
        
    public class PlayerState
    {
        public bool IsBanker;
            
        public bool IsReceive;

        public bool IsReady;

        public int Score;

        public int Bet;

        public List<PokerCard> Cards;
            
        public PlayerPokerCards CalculateCards;
    }
}