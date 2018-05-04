using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;

namespace ETHotfix
{
    public abstract class Poker : Entity
    {
        #region 创建卡牌

        /// <summary>
        ///     创建卡牌
        /// </summary>
        /// <param name="batch">几副牌</param>
        /// <param name="isRandom">是否随机排序</param>
        /// <returns>扑克牌规则是54张</returns>
        public List<PokerCard> CreateCards(int batch, bool isRandom = false)
        {
            var cardsWarehousing = new List<PokerCard>();

            for (var b = 0; b < batch; b++)
            {
                // 生成卡牌

                for (var i = 0; i < 4; i++)
                {
                    for (var j = 1; j <= 13; j++) cardsWarehousing.Add(new PokerCard {CardType = i, CardNumber = j, IsPayout = false});
                }

                // 生成大小王

                var king = new PokerCard {CardType = 4, IsPayout = false};

                king.CardNumber = 1;

                cardsWarehousing.Add(king);
                
                king.CardNumber = 2;

                cardsWarehousing.Add(king);
            }

            return isRandom ? RandomCards(cardsWarehousing) : cardsWarehousing;
        }

        /// <summary>
        ///     随机排序卡牌
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
        
        public virtual PokerCard[] CalculateCards(List<PokerCard> cards)
        {
            return null;
        }
    }
}