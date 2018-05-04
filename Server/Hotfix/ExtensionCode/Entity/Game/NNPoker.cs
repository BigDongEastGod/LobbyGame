using System.Collections.Generic;
using System.Linq;
using ETModel;

namespace ETHotfix
{
    public class NNPoker : Poker
    {
        /// <summary>
        /// 转换卡牌为文字
        /// </summary>
        /// <param name="number">数值</param>
        /// <returns>转换成例如：牛1、牛牛等文字</returns>
        private static string Calculate(int number)
        {
            var calculateNumber = number % 10;

            return calculateNumber > 0 ? "牛" + calculateNumber : "牛牛";
        }

        /// <summary>
        /// 转换牌数值
        /// </summary>
        /// <param name="cardNumber">卡牌数值</param>
        /// <returns>如果牌数值大于10就变成10</returns>
        private static int ConvertCard(int cardNumber)
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
        public override PokerCard[] CalculateCards(List<PokerCard> cards)
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

            return sortingCards[4]?.CardNumber > 0 ? sortingCards : null;
        }
    }
}