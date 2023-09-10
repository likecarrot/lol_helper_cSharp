using Newtonsoft.Json.Linq;
using QuickType;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_helper_cSharp.helpers
{
    public class Aram_Helper : Base_Helper
    {
        private long wait_max_sec = Settings_Config.GetInstance().settings.AramHelper.AramConfig.WaitMaxSec;
        private List<LoveChamp> lova_champs = Settings_Config.GetInstance().settings.AramHelper.AramConfig.LoveChamps;
        private long use_Reroller = Settings_Config.GetInstance().settings.AramHelper.AramConfig.UseReroller;
        private long _my_summoner_id = 0;
        public Aram_Helper()
        {

        }

        public override async Task<bool> Run()
        {
            long _last_chmap = 0;
            int clock_sec = 0;
            int rolls = 0;
            var status = await ApiManager.GetClientStatus();
            if (status == riot_apis.Consture.client_status.ChampSelect)
            {
                if (use_Reroller == 1 && rolls == 0)
                {
                    await ApiManager.GetReroller();
                    await ApiManager.GetReroller();
                }
                long select = await GetNowSelectChampAsync();
                if (GetScore(select) <= 0)
                {
                    if (use_Reroller == 2 && rolls == 0)
                    {
                        await ApiManager.GetReroller();
                        await ApiManager.GetReroller();
                    }
                }
                _last_chmap = await GetNowSelectChampAsync();
                do
                {
                    List<LoveChamp> now_all_champs = new List<LoveChamp>();
                    long now_select = _last_chmap;
                    long now_select_score = GetScore(now_select);
                    now_all_champs.Add(new LoveChamp { ChampId = now_select, LoveScore = now_select_score });
                    var champions = await GetAllBechChampsAsync();
                    foreach (var item in champions)
                    {
                        now_all_champs.Add(new LoveChamp { ChampId = item, LoveScore = GetScore(item) });
                    }

                    long maxKey = 0;
                    long maxValue = 0;
                    foreach (var item in now_all_champs)
                    {
                        if (item.LoveScore>maxValue)
                        {
                            maxValue = item.LoveScore;
                            maxKey = item.ChampId;
                        }
                    }
                    if (maxValue!=0&&maxValue>now_select_score)
                    {
                        _ = ApiManager.BenchChamp((int)maxKey);
                        _last_chmap = maxKey;
                    }
                    await Task.Delay(1000);
                    if (_last_chmap!= await GetNowSelectChampAsync())
                    {
                        Debug.WriteLine("玩家自己更改了英雄 所以提前退出了循环.");
                        break;
                    }
                    if (wait_max_sec!=0)
                    {
                        clock_sec++;
                        if (clock_sec>wait_max_sec)
                        {
                            Debug.WriteLine("已经超时 所以退出了循环.");
                            break;  
                        }
                    }
                } while (await ApiManager.GetClientStatus()==riot_apis.Consture.client_status.ChampSelect);
            }
            return true;
        }
        private async Task set_my_summoner_idAsync()
        {
            var sum = await ApiManager.GetSummonerInfo();
            _my_summoner_id = sum.SummonerId;
        }
        private long GetScore(long champid)
        {
            foreach (var item in lova_champs)
            {
                if (item.ChampId == champid)
                {
                    return item.LoveScore;
                }
            }
            return 0;
        }
        private async Task<List<int>> GetAllBechChampsAsync()
        {
            List<int> ret = new List<int>();
            var c = await ApiManager.GetTeamSessions();
            if (c.BenchEnabled == true && c.BenchChampions.Length != 0)
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(c.BenchChampions);
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject obj in jsonArray)
                {
                    if (obj.ContainsKey("championId"))
                    {
                        ret.Add((int)obj.GetValue("championId"));
                        Console.WriteLine((int)obj.GetValue("championId"));
                    }
                }
            }
            return ret;
        }

        private async Task<long> GetNowSelectChampAsync()
        {
            long ret = 0;
            var c = await ApiManager.GetTeamSessions();
            foreach (var item in c.MyTeam)
            {
                if (_my_summoner_id == item.SummonerId)
                {
                    ret = item.ChampionId;
                }
            }
            if (ret == 0)
            {
                set_my_summoner_idAsync();
                foreach (var item in c.MyTeam)
                {
                    if (_my_summoner_id == item.SummonerId)
                    {
                        ret = item.ChampionId;
                    }
                }
            }
            return ret;
        }
    }

}
