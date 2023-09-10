using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_helper_cSharp.riot_apis
{
    public class Consture
    {
        public enum client_status
        {
            Error, None, Lobby, Matchmaking, ReadyCheck, ChampSelect, InProgress, PreEndOfGame, WaitingForStats, EndOfGame, Reconnect
        }
        static Dictionary<string, string> gameStatusDict1 = new Dictionary<string, string>()
{
    { "Error", "获取错误" },
    { "None", "游戏大厅" },
    { "Lobby", "房间内" },
    { "Matchmaking", "匹配中" },
    { "ReadyCheck", "找到对局" },
    { "ChampSelect", "选英雄中" },
    { "InProgress", "游戏中" },
    { "PreEndOfGame", "游戏即将结束" },
    { "WaitingForStats", "等待结算页面" },
    { "EndOfGame", "游戏结束" },
    { "Reconnect", "等待重新连接" }
};
        static Dictionary<string, client_status> gameStatusDict2 = new Dictionary<string, client_status>()
{
    { "\"Error\"", client_status.Error },
    { "\"None\"", client_status.None },
    { "\"Lobby\"", client_status.Lobby },
    { "\"Matchmaking\"", client_status.Matchmaking },
    { "\"ReadyCheck\"", client_status.ReadyCheck },
    { "\"ChampSelect\"", client_status.ChampSelect },
    { "\"InProgress\"", client_status.InProgress },
    { "\"PreEndOfGame\"", client_status.PreEndOfGame },
    { "\"WaitingForStats\"", client_status.WaitingForStats },
    { "\"EndOfGame\"", client_status.EndOfGame },
    { "\"Reconnect\"", client_status.Reconnect }
};
        public static string GetGameStatus(client_status status)
        {
            string statusString = status.ToString();
            if (gameStatusDict1.ContainsKey(statusString))
            {
                return gameStatusDict1[statusString];
            }
            return statusString;
        }
        public static client_status GetGameStatus(string statusString)
        {
            if (gameStatusDict2.ContainsKey(statusString))
            {
                return gameStatusDict2[statusString];
            }
            return client_status.Error;
        }


        public enum gamedata_resources_type
        {
            ICON, CHAMP_ICON, SKIN
        }

        static Dictionary<string, string> rank_tiers_dict = new Dictionary<string, string>()
    {{"CHALLENGER","最强王者"},
    {"GRANDMASTER","傲世宗师"},
    {"MASTER","超凡大师"},
    {"IRON","坚韧黑铁"},
    {"DIAMOND","璀璨钻石"},
    {"EMERALD","流光翡翠"},
    {"PLATINUM","华贵铂金"},
    {"GOLD","荣耀黄金"},
    {"SILVER","不屈白银"},
    {"BRONZE","英勇黄铜"},
    {"UNRANKED","没有段位"},

    {"Grey","灰"},
    {"Green","绿"},
    {"Blue","蓝"},
    {"Purple","紫"},
    {"Hyper","金"}
};
        public static string GetRankTire(string tire)
        {
            if (rank_tiers_dict.ContainsKey(tire))
            {
                return rank_tiers_dict[tire];
            }
            return string.Empty;
        }

        static Dictionary<string, string> game_type = new Dictionary<string, string>(){
    {"MATCHED_GAME","匹配模式"},
    {"CUSTOM_GAME","自定义"},
    {"TUTORIAL_GAME","游戏教程"}
};
        public static string GetGameType(string type)
        {
            if (game_type.ContainsKey(type))
            {
                return game_type[type];
            }
            return string.Empty;
        }
        static Dictionary<int, string> GAME_CLASS = new Dictionary<int, string>(){
    {0, "自定义"},
    {72, "冰雪节1v1"},
    {73, "冰雪节2v2"},
    {75, "双倍人数"},
    {76, "极速模式"},
    {78, "镜像模式"},
    {83, "合作对战极速模式"},
    {98, "双倍人数"},
    {100, "狂乱ARAM"},
    {310, "复仇者"},
    {313, "黑市交易"},
    {317, "不是支配"},
    {325, "全随机"},
    {400, "选秀"},
    {420, "单双"},
    {430,"匹配"},
    {440, "灵活"},
    {450, "大乱斗"},
    {600, "猎杀刺客"},
    {610, "宇宙星系"},
    {700, "对决"},
    {720, "ARAM对决"},
    {820, "初级人机"},
    {830, "入门人机"},
    {840, "初级人机"},
    {850, "一般人机"},
    {900, "乱斗模式"},
    {910, "晋级"},
    {920, "波比王传奇"},
    {940, "攻城战"},
    {950, "末日投票"},
    {960, "末日"},
    {980, "星光守护者"},
    {990, "星光守护者"},
    {1000, "PROJECT: Hunters"},
    {1010, "雪地乱斗"},
    {1020, "全员英雄"},
    {1030, "奥德赛入门"},
    {1040, "奥德赛新兵"},
    {1050, "奥德赛船员"},
    {1060, "奥德赛船长"},
    {1070, "奥德赛强袭"},
    {1090, "云顶匹配"},
    {1100, "云顶排位"},
    {1110, "云顶教学"},
    {1111, "云顶测试"},
    {1130,"云顶狂暴"},
    {1160,"云顶双人作战"},
    {1300, "奇迹模式"},
    {1400, "终极技能书"},
    {1700, "斗魂竞技场"},
    {1900, "URF"},
    {2000, "教学 1"},
    {2010, "教学 2"},
    {2020, "教学 3"},
};//不完全统计,并且只是暂时作为判断游戏类型的选项
        public static string GetGameClass(int id)
        {
            if (GAME_CLASS.ContainsKey(id))
            {
                return GAME_CLASS[id];
            }
            return string.Empty;
        }



    }
}
