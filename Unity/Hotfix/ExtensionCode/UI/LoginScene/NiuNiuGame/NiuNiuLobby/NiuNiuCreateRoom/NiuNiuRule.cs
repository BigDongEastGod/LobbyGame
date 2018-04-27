using System.Collections.Generic;

namespace ETHotfix
{
    public class NiuNiuRule
    {
        public List<string> ListDiFen { get; }
        public List<string> ListjuShu { get; }
        public List<string> ListFangFei { get; }
        public List<string> ListZiDongKaiZhuo { get; }
        public List<string> ListXianJiaTuiZhu { get; }
        public List<string> ListShangZhuangFenShu { get; }
        public List<string> ListZuiDaQiangZhuang { get; }
        public List<string> ListFanBeiGuiZe { get; }
        public List<string> ListTeShuPaiXing { get; }
        public List<string> ListGaoJiXuanXiang { get; }


        public List<string> Score; //底分
        public List<int> Dish; //局数
        public List<int> RoomRate; //局费
        public List<int> AutoGame; //自动开桌
        public List<int> PlayerPush; //闲家推注

        public List<List<string>> DoubleRules; //翻倍规则
        bool ShunZiRules; //顺子牛
        bool TonghuaRules; //同花牛
        bool HuLuRules; //葫芦牛
        bool WuHuaRules; //五花牛
        bool ZhaDanRules; //炸弹牛
        bool WuXiaoRules; //五小牛
        bool ZhongTuJinRuRules; //中途禁入
        bool CuoPaiRules; //禁止搓牌
        bool WangLaiRules; //王癞玩法
        bool MaiMaRules; //闲家买码

        public NiuNiuRule(List<string> listDiFen, List<string> listjuShu, List<string> listFangFei,
            List<string> listZiDongKaiZhuo, List<string> listXianJiaTuiZhu, List<string> listShangZhuangFenShu,
            List<string> listZuiDaQiangZhuang, List<string> listFanBeiGuiZe, List<string> listTeShuPaiXing,
            List<string> listGaoJiXuanXiang)
        {
            ListDiFen = listDiFen;
            ListjuShu = listjuShu;
            ListFangFei = listFangFei;
            ListZiDongKaiZhuo = listZiDongKaiZhuo;
            ListXianJiaTuiZhu = listXianJiaTuiZhu;
            ListShangZhuangFenShu = listShangZhuangFenShu;
            ListZuiDaQiangZhuang = listZuiDaQiangZhuang;
            ListFanBeiGuiZe = listFanBeiGuiZe;
            ListTeShuPaiXing = listTeShuPaiXing;
            ListGaoJiXuanXiang = listGaoJiXuanXiang;
        }
    }

    public static class NnDpType
    {
        public const string DiFen = "DiFen";
        public const string JuShu = "JuShu";
        public const string FangFei = "FangFei";
        public const string ZiDongKaiZhuo = "ZiDongKaiZhuo";
        public const string XianJiaTuiZhu = "XianJiaTuiZhu";
        public const string ShangZhuangFenShu = "ShangZhuangFenShu";
        public const string ZuiDaQiangZhuang = "ZuiDaQiangZhuang";
        public const string FanBeiGuiZe = "FanBeiGuiZe";
    }

    public static class NiuNiuRuleInstance
    {
        #region 牛牛上庄

        public static NiuNiuRule NiuNiuShangZhuang = new NiuNiuRule(new List<string>() {"1/2", "2/4", "4/8", "5/10"},
            new List<string>() {"10局", "20局"},
            new List<string>() {"房主支付(       4)  ", "AA支付(每人      1)  "},
            new List<string>() {"手动开桌", "满5人开", "满6人开", "满7人开", "满8人开"},
            new List<string>() {"无", "5倍", "10倍", "20倍"},
            null,
            null,
            new List<string>() {"牛牛x4 牛九x3 牛八x2 牛七x2", "牛牛x3 牛九x2 牛八x2"},
            new List<string>() {"顺子牛(5倍)", "同花牛(5倍)", "葫芦牛(6倍)", "五花牛(5倍)", "炸弹牛(6倍)", "五小牛(8倍)"},
            new List<string>() {"中途禁入", "禁止搓牌", "王癞玩法"})
        {
            Score = new List<string>() {"1/2", "2/4", "4/8", "5/10"},
            Dish = new List<int>() {10, 20},
            RoomRate = new List<int>() {4, 1},
            AutoGame = new List<int>() {0, 5, 6, 7, 8},
            PlayerPush = new List<int>() {0, 5, 10, 20},
            DoubleRules = new List<List<string>>()
            {
                new List<string>() {"0/4", "9/3", "8/2", "7/2"},
                new List<string>() {"0/3", "9/2", "8/2"}
            }
        };

        #endregion

        #region 固定庄家

        public static NiuNiuRule GuDingZhuangJia = new NiuNiuRule(new List<string>() {"1/2", "2/4", "4/8", "5/10"},
            new List<string>() {"10局", "20局"},
            new List<string>() {"房主支付(       3)  ", "AA支付(每人      1)  "},
            new List<string>() {"手动开桌", "满4人开", "满5人开", "满6人开"},
            new List<string>() {"无", "5倍", "10倍", "20倍"},
            new List<string>() {"无", "100", "150", "200"},
            null,
            new List<string>() {"牛牛x4 牛九x3 牛八x2 牛七x2", "牛牛x3 牛九x2 牛八x2"},
            new List<string>() {"顺子牛(5倍)", "同花牛(5倍)", "葫芦牛(6倍)", "五花牛(5倍)", "炸弹牛(6倍)", "五小牛(8倍)"},
            new List<string>() {"中途禁入", "禁止搓牌", "王癞玩法", "闲家买码"})
        {
            Score = new List<string>() {"1/2", "2/4", "4/8", "5/10"},
            Dish = new List<int>() {10, 20},
            RoomRate = new List<int>() {3, 1},
            AutoGame = new List<int>() {0, 4, 5, 6},
            PlayerPush = new List<int>() {0, 5, 10, 20},
            DoubleRules = new List<List<string>>()
            {
                new List<string>() {"0/4", "9/3", "8/2", "7/2"},
                new List<string>() {"0/3", "9/2", "8/2"}
            }
        };

        #endregion

        #region 自由抢庄

        public static NiuNiuRule ZiYouQiangZhuang = new NiuNiuRule(new List<string>() {"1/2", "2/4", "4/8", "5/10"},
            new List<string>() {"10局", "20局"},
            new List<string>() {"房主支付(       3)  ", "AA支付(每人      1)  "},
            new List<string>() {"手动开桌", "满4人开", "满5人开", "满6人开"},
            new List<string>() {"无", "5倍", "10倍", "20倍"},
            null,
            null,
            new List<string>() {"牛牛x4 牛九x3 牛八x2 牛七x2", "牛牛x3 牛九x2 牛八x2"},
            new List<string>() {"顺子牛(5倍)", "同花牛(5倍)", "葫芦牛(6倍)", "五花牛(5倍)", "炸弹牛(6倍)", "五小牛(8倍)"},
            new List<string>() {"中途禁入", "禁止搓牌", "王癞玩法", "闲家买码", "下注限制", "暗抢庄家,下注加倍"})
        {
            Score = new List<string>() {"1/2", "2/4", "4/8", "5/10"},
            Dish = new List<int>() {10, 20},
            RoomRate = new List<int>() {3, 1},
            AutoGame = new List<int>() {0, 4, 5, 6},
            PlayerPush = new List<int>() {0, 5, 10, 20},
            DoubleRules = new List<List<string>>()
            {
                new List<string>() {"0/4", "9/3", "8/2", "7/2"},
                new List<string>() {"0/3", "9/2", "8/2"}
            }
        };

        #endregion

        #region 明牌抢庄

        public static NiuNiuRule MingPaiQiangZhuang = new NiuNiuRule(new List<string>() {"1/2", "2/4", "4/8", "5/10"},
            new List<string>() {"10局", "20局"},
            new List<string>() {"房主支付(       3)  ", "AA支付(每人      1)  "},
            new List<string>() {"手动开桌", "满4人开", "满5人开", "满6人开"},
            new List<string>() {"无", "5倍", "10倍", "20倍"},
            null,
            new List<string>() {"1倍", "2倍", "3倍", "4倍"},
            new List<string>() {"牛牛x4 牛九x3 牛八x2 牛七x2", "牛牛x3 牛九x2 牛八x2"},
            new List<string>() {"顺子牛(5倍)", "同花牛(5倍)", "葫芦牛(6倍)", "五花牛(5倍)", "炸弹牛(6倍)", "五小牛(8倍)"},
            new List<string>() {"中途禁入", "禁止搓牌", "王癞玩法", "闲家买码", "下注限制", "暗抢庄家", "下注加倍"})
        {
            Score = new List<string>() {"1/2", "2/4", "4/8", "5/10"},
            Dish = new List<int>() {10, 20},
            RoomRate = new List<int>() {3, 1},
            AutoGame = new List<int>() {0, 4, 5, 6},
            PlayerPush = new List<int>() {0, 5, 10, 20},
            DoubleRules = new List<List<string>>()
            {
                new List<string>() {"0/4", "9/3", "8/2", "7/2"},
                new List<string>() {"0/3", "9/2", "8/2"}
            }
        };

        #endregion

        #region 通比牛牛

        public static NiuNiuRule TongBiNiuNiu = new NiuNiuRule(new List<string>() {"1", "2", "4", "5"},
            new List<string>() {"10局", "20局"},
            new List<string>() {"房主支付(       3)  ", "AA支付(每人      1)  "},
            new List<string>() {"手动开桌", "满4人开", "满5人开", "满6人开"},
            null,
            null,
            null,
            new List<string>() {"牛牛x4 牛九x3 牛八x2 牛七x2", "牛牛x3 牛九x2 牛八x2"},
            new List<string>() {"顺子牛(5倍)", "同花牛(5倍)", "葫芦牛(6倍)", "五花牛(5倍)", "炸弹牛(6倍)", "五小牛(8倍)"},
            new List<string>() {"中途禁入", "禁止搓牌", "王癞玩法"})
        {
            Score = new List<string>() {"1/2", "2/4", "4/8", "5/10"},
            Dish = new List<int>() {10, 20},
            RoomRate = new List<int>() {3, 1},
            AutoGame = new List<int>() {0, 4, 5, 6},
            PlayerPush = null,
            DoubleRules = new List<List<string>>()
            {
                new List<string>() {"0/4", "9/3", "8/2", "7/2"},
                new List<string>() {"0/3", "9/2", "8/2"}
            }
        };

        #endregion
    }
}