using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_helper_cSharp.riot_apis
{
    public class ApiUrls
    {
        public static string current_client_player_summoner_info_api = "/lol-summoner/v1/current-summoner";        //获取现在客户端上登录的玩家的信息
        public static string puuid_get_player_rank_data_api = "/lol-ranked/v1/ranked-stats/";          //通过puuid获取段位信息
        public static string client_get_player_status_api = "/lol-gameflow/v1/gameflow-phase";         //获取客户端状态,比如 游戏中,大厅中
        public static string client_accept_matching_api = "/lol-matchmaking/v1/ready-check/accept";        //接收对局
        public static string client_decline_matching_api = "/lol-matchmaking/v1/ready-check/decline";  //拒绝对局
        public static string matching_get_myteam_summonerinfo_api = "/lol-champ-select/v1/session";        //获取己方队伍的信息,比如 summoner accoutid
        public static string get_current_game_mode_api = "/lol-gameflow/v1/session";                   //430 匹配 420 单双排位 440灵活 450 大乱斗 云顶匹配 1090  云顶排位 1100 云顶狂暴 1130 云顶双人作战 1160
        public static string use_reroller_api = "/lol-champ-select/v1/session/my-selection/reroll";    //使用骰子
        public static string get_owner_champions_api = "/lol-champions/v1/owned-champions-minimal";    //获取已经拥有的英雄
        public static string get_all_champions_api = "/lol-game-data/assets/v1/champion-summary.json"; //获取所有英雄
        public static string summonerid_get_summonerinfo_api = "/lol-summoner/v1/summoners/";  //根据summonerid获取信息 + ${summonerId}
        public static string gameid_get_detailed_data_api = "/lol-match-history/v1/games/";        //根据gameid获取详细数据,+gameid
        public static string summonerid_get_top_champion_api = "/lol-collections/v1/inventories/"; //+ ${summonerId} + /champion-mastery/top?limit=6
        public static string get_select_champion_chatroomid_api = "/lol-chat/v1/conversations";    //获取选人界面时,游戏房间id
        public static string gamename_get_summonerinfo_api = "/lol-summoner/v1/summoners?name=";   //玩家游戏名 "/lol-summoner/v1/summoners?name=" + name
        public static string aram_swap_champ_api = "/lol-champ-select/v1/session/bench/swap/"; //+英雄id  大乱斗从备战席拿英雄
        public static string lock_champ_completed_api = "/lol-champ-select/v1/session/actions/";   //+localPlayerCellId
        public static string lock_now_select_champ_api = "/lol-champ-select/v1/session/actions/";  //+localPlayerCellId +/complete

        public static string auto_next_game_api = "/lol-lobby/v2/play-again";               //下一局
        public static string search_game_api = "/lol-lobby/v2/lobby/matchmaking/search";    //开始匹配

        public static string get_game_version_http = "https://game.gtimg.cn/images/lol/act/img/js/heroList/hero_list.js";    //从lol网站获取版本信息,并且加入到换肤中
        public static string get_gamedata_icons = "/lol-game-data/assets/v1/profile-icons/";    //下载头像
        public static string get_gamedata_champicons = "/lol-game-data/assets/v1/champion-icons/";//下载英雄头像
        public static string get_matchingdatas = "/lol-match-history/v1/products/lol/{0}/matches?begIndex={1}&endIndex={2}";    //{0} --puuid {1}--起始条 {2}--结束条
        public static string get_top_champs = "/lol-collections/v1/inventories/{0}/champion-mastery/top?limit={1}"; //0-account_id 1-limit
        public static string get_game_version = "/system/v1/builds";    //获取游戏版本

        public static string get_profile_skin = "/lol-summoner/v1/current-summoner/summoner-profile";//获取主页面上的皮肤id
        public static string get_champ_info = "/lol-game-data/assets/v1/champions/{0}.json";    //0--英雄id 
        //下载皮肤计算方式 获取skinid /1000 == 英雄id  
        //获取英雄详情
        //其中 loadScreenPath 就是下载地址

        public static string get_current_player_settings = "/lol-game-settings/v1/game-settings";   //获取现在玩家的游戏配置
        public static string set_current_player_settings = "/lol-game-settings/v1/game-settings";   //设置现在玩家的游戏配置 patch方法


    }
}
