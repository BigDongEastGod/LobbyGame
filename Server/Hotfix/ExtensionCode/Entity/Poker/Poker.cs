using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;

namespace ETHotfix
{
    /*
         * 1: 黑桃 2: 红桃 3: 梅花 4：方块 5:王
         */
    public abstract class Poker : Entity
    {
        #region 创建卡牌

        /// <summary>
        ///  创建卡牌
        /// </summary>
        /// <param name="batch">几副牌</param>
        /// <param name="isRandom">是否随机排序</param>
        /// <param name="isCreateKing">是否生成王</param>
        /// <returns>扑克牌规则是54张</returns>
        public List<PokerCard> CreateCards(int batch, bool isRandom = false, bool isCreateKing = true)
        {
            var cardsWarehousing = new List<PokerCard>();

            for (var b = 0; b < batch; b++)
            {
                // 生成卡牌

                for (var i = 1; i <= 4; i++)
                {
                    for (var j = 1; j <= 13; j++)
                        cardsWarehousing.Add(new PokerCard {CardType = i, CardNumber = j, Grade = j + (i * 10), IsPayout = false});
                }
            }

            if (!isCreateKing) return isRandom ? RandomCards(cardsWarehousing) : cardsWarehousing;
            
            // 生成大小王
            
            var king = new PokerCard {CardType = 5, IsPayout = false, CardNumber = 1, Grade = 51};

            cardsWarehousing.Add(king);

            king.CardNumber = 2;

            king.Grade = 52;

            cardsWarehousing.Add(king);

            return isRandom ? RandomCards(cardsWarehousing) : cardsWarehousing;
        }

        /// <summary>
        /// 随机排序卡牌
        /// </summary>
        /// <param name="cards">卡牌数组</param>
        /// <returns></returns>
        public List<PokerCard> RandomCards(List<PokerCard> cards)
        {
            return cards.OrderBy(d => Guid.NewGuid()).ToList();
        }

        #endregion
        
        #region 发送卡牌

        /// <summary>
        /// 发牌
        /// </summary>
        /// <param name="cards">卡牌数组</param>
        /// <param name="players">玩家数量</param>
        /// <param name="cardCount">每个玩家的牌数量</param>
        /// <param name="isRandom">是否随机发牌</param>
        /// <returns>根据玩家数量发给玩家一定数量的卡牌</returns>
        public Dictionary<int, List<PokerCard>> Deal(List<PokerCard> cards, int players, int cardCount,bool isRandom = false)
        {
            var playerCards = new Dictionary<int, List<PokerCard>>();
            
            for (var i = 0; i < players; i++)
            {
                var card = new List<PokerCard>();

                for (var j = 0; j < cardCount; j++)
                {
                    var cardScreen = cards.Where(d => !d.IsPayout).ToList();
                    
                    if (!cardScreen.Any()) continue;
                    
                    var currentCard = isRandom
                        ? cardScreen.ElementAt(new Random().Next(0, cardScreen.Count()))
                        : cardScreen.ElementAt(0);

                    card.Add(currentCard);

                    currentCard.IsPayout = true;
                }

                playerCards.Add(i, card);
            }

            return playerCards;
        }

        #endregion

        public virtual int Calculate(int number)
        {
            return 0;
        }
        
        public virtual PlayerPokerCards CalculateCards(List<PokerCard> cards)
        {
            return null;
        }
    }
}