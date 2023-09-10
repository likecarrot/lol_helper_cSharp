using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//匹配助手
namespace lol_helper_cSharp.helpers
{
    public enum SELECT_CHAMPS_STATUS
    {
        CAN,WAIT,CANOT
    }
    public class Match_Helper : Base_Helper
    {
        private long lock_champ = Settings_Config.GetInstance().settings.ClassicHelper.ClassicConfig.ChampId;
        private long    wait_max= Settings_Config.GetInstance().settings.ClassicHelper.ClassicConfig.LockTimeout;
        private bool auto_lock = Settings_Config.GetInstance().settings.ClassicHelper.ClassicConfig.AutoLock;

        public Match_Helper()
        {

        }
        public override async Task<bool> Run()
        {
            var has_champs = await ApiManager.GetOwnerChamps();
            if (lock_champ==0)
            {
                return false;
            }
            if (await CheckHasChamp(lock_champ) ==true&&await CheckIsSelectChamp(lock_champ)==true)
            {
                if (auto_lock==true)
                {
                    if (wait_max!=0)
                    {
                        await    ApiManager.SetLockChamp((int)lock_champ, true);
                    }
                    else
                    {
                        await ApiManager.SetLockChamp((int)lock_champ, false);
                        await Task.Delay((int)(wait_max * 1000));
                        await ApiManager.SetLockChamp((int)await GetCurrentSelectChamp(), true);
                    }
                }else
                    await ApiManager.SetLockChamp((int)lock_champ, false);
            }
            return true;
        }
        private async   Task<bool>  CheckHasChamp(long champid)
        {
            var champs = await ApiManager.GetOwnerChamps();
            foreach (var item in champs)
            {
                if (item.Id==champid)
                {
                    return true;
                }
            }
            return false;
        }
        private async Task<bool> CheckIsSelectChamp(long champid)
        {
            var c = await ApiManager.GetTeamSessions();
            foreach (var item in c.MyTeam)
            {
                if (item.ChampionId==champid)
                {
                    return false;
                }
            }
            return true;
        }
        private async   Task<long> GetCurrentSelectChamp()
        {
            var s = await ApiManager.GetTeamSessions();
            var s2 = await ApiManager.GetSummonerInfo();
            foreach (var item in s.MyTeam)
            {
                if (s2.SummonerId==item.SummonerId)
                {
                    return item.ChampionId;
                }
            }
            return 0;
        }

    }
}
