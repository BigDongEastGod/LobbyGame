using System.Collections.Generic;
using System.Linq;
using CSharpx;
using ETModel;

namespace ETHotfix
{
    public class NNPoker : Poker
    {
        /// <summary>
        /// 计算牛几
        /// </summary>
        /// <param name="number">数值</param>
        /// <returns>转换成例如：牛1、牛牛等文字</returns>
        public override int Calculate(int number)
        {
            var calculateNumber = number % 10;

            return calculateNumber > 0 ? calculateNumber : 10;
        }

        /// <summary>
        /// 转换牌数值
        /// </summary>
        /// <param name="cardNumber">卡牌数值</param>
        /// <returns>如果牌数值大于10就变成10</returns>
        private int ConvertCard(int cardNumber)
        {
            return cardNumber > 10 ? 10 : cardNumber;
        }

        /// <summary>
        /// 计算卡牌
        /// </summary>
        /// <param name="cards">卡牌数组</param>
        /// /
        /// <returns>卡牌花色</returns>
        /// <summary>
        ///     计算是否有牛出现、如果没有牛返回为Null
        /// </summary>
        /// <param name="cards">卡牌数组</param>
        /// <returns>计算是否有牛出现、如果没有牛返回为Null</returns>
        public override PlayerPokerCards CalculateCards(List<PokerCard> cards)
        {
            var cardsCount = cards.Select(d => ConvertCard(d.CardNumber)).Sum();

            // 如果总数小于12、表示不可能有牛出现、直接返回Null

            if (cardsCount < 12) return null;

            // 定义一个数组、用于存放计算过的卡牌

            var sortingCards = new PokerCard[5];

            // 判断牌型、从牌总数减去30、20、10

            for (var n = 3; n > 0; n--)
            {
                var number = cardsCount - 10 * n;

                // 如果小于2表示没牛、直接跳过当前循环

                if (number < 2) continue;

                #region 计算牌型

                for (var i = 0; i < cards.Count(); i++)
                {
                    for (var j = 0; j < cards.Count(); j++)
                    {
                        if (i == j || ConvertCard(cards.ElementAt(i).CardNumber) +
                            ConvertCard(cards.ElementAt(j).CardNumber)
                            != number) continue;

                        #region 计算最后两张牌、并排序

                        var remainderCards = new PokerCard[2];

                        remainderCards[0] = cards.ElementAt(i);

                        remainderCards[1] = cards.ElementAt(j);

                        remainderCards = remainderCards.OrderBy(d => d.CardNumber).ToArray();

                        for (var k = 0; k < remainderCards.Length; k++) sortingCards[3 + k] = remainderCards[k];

                        #endregion

                        #region 计算前面三张牌、并排序

                        var skip = 0;

                        remainderCards = new PokerCard[3];

                        for (var k = 0; k < cards.Count(); k++)
                        {
                            if (k == i || k == j) continue;

                            remainderCards[skip] = cards.ElementAt(k);

                            skip++;
                        }

                        remainderCards = remainderCards.OrderBy(d => d.CardNumber).ToArray();

                        for (var k = 0; k < remainderCards.Length; k++)  sortingCards[k] = remainderCards[k];

                        #endregion

                        break;
                    }

                    if (sortingCards[4]?.CardNumber > 0) break;
                }

                #endregion
            }
            
            // 如果有牛、就计算出牛几

            if (sortingCards[4]?.CardNumber > 0)
            {
                return new PlayerPokerCards()
                {
                    CardTypeNumber =
                        Calculate(
                            ConvertCard(sortingCards[3].CardNumber) + ConvertCard(sortingCards[4].CardNumber)),
                    GradeCount = sortingCards.Sum(d => d.Grade),
                    PokerCards = sortingCards.ToList()
                };
            }

            return null;
        }
        
        #region 特殊牌型检测

        public PlayerPokerCards SpecialCards(List<PokerCard> cards)
        {
            var cardsCount = cards.Select(d => ConvertCard(d.CardNumber)).Sum();
            
            var sortingCards = new PokerCard[5];
            
            // 五小牛

            if (cardsCount < 10 && cards.GroupBy(d => d.CardNumber < 5).Count() == 1)
            {
                return new PlayerPokerCards() {CardTypeNumber = 16, PokerCards = cards.OrderBy(d => d.CardNumber).ToList()};
            }
            
            // 炸弹牛

            var groupCards = cards.GroupBy(d => d.CardNumber).ToList();
            
            if (groupCards.Count() == 2 && groupCards.ElementAt(0).Count()==4)
            {
                for (var i = 0; i < groupCards.ElementAt(0).Count(); i++) sortingCards[i] = groupCards.ElementAt(0).ElementAt(i);
                
                sortingCards[4] = groupCards.ElementAt(1).ElementAt(0);

                return new PlayerPokerCards() {CardTypeNumber = 15, PokerCards = sortingCards.ToList()};
            }
            
            // 葫芦牛

            if (groupCards.Count() == 2 && groupCards.ElementAt(0).Count() == 3)
            {
                for (var i = 0; i < groupCards.ElementAt(0).Count(); i++) sortingCards[i] = groupCards.ElementAt(0).ElementAt(i);

                for (var i = 1; i <= groupCards.ElementAt(1).Count(); i++) sortingCards[i + 2] = groupCards.ElementAt(1).ElementAt(i);
                
                return new PlayerPokerCards() {CardTypeNumber = 14, PokerCards = sortingCards.ToList()};
            }

            // 五花牛

            groupCards = cards.GroupBy(d => d.CardType).ToList();

            if (groupCards.Count() == 1 && groupCards.ElementAt(0).Count(d => d.CardNumber > 10) == 5)
            {
                return new PlayerPokerCards() {CardTypeNumber = 13, PokerCards = cards.OrderBy(d => d.CardNumber).ToList()};
            }
            
            // 同花牛

            if (groupCards.Count() == 1)
            {
                return new PlayerPokerCards() {CardTypeNumber = 12, PokerCards = cards.OrderBy(d => d.CardNumber).ToList()};
            }
            
            // 顺子牛

            var cardnumber = cards.OrderBy(d => d.CardNumber).First().CardNumber;

            for (var i = 0; i < 5; i++) cardnumber += i;

            if (cardsCount == cardnumber)
            {
                return new PlayerPokerCards() {CardTypeNumber = 11, PokerCards = cards.OrderBy(d => d.CardNumber).ToList()};
            }

            return null;
        }

        #endregion
    }
}