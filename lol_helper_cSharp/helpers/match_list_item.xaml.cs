using lol_helper_cSharp.riot_apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lol_helper_cSharp.helpers
{
    /// <summary>
    /// match_list_item.xaml 的交互逻辑
    /// </summary>
    public partial class match_list_item : UserControl
    {
        private Resources_Helper resourcesManager;

        public match_list_item(QuickType.Game item)
        {
            InitializeComponent();
            resourcesManager = new Resources_Helper();
            init(item);
        }

        private async Task init(QuickType.Game item)
        {
            if (!string.IsNullOrEmpty(Consture.GetGameClass((int)item.QueueId)))
            {
                game_mode.Text = Consture.GetGameClass((int)item.QueueId);
            }
            else
            {
                game_mode.Text = "未知模式";
            }
            create_date.Text = DateTimeOffset.FromUnixTimeMilliseconds(item.GameCreation).DateTime.ToString();
            game_time.Text = "对局时长:" + item.GameDuration / 60 + "分钟";
            if (item.Participants[0].Stats.Win == true)
            {
                game_win.Text = "胜利";
                game_win.Foreground = Brushes.Blue;
            }
            else
            {
                game_win.Text = "失败";
                game_win.Foreground = Brushes.Red;
                this.Background = Brushes.LightPink;
            }
            use_champ.Source = new BitmapImage(new Uri(await resourcesManager.DownloadAsync(Consture.gamedata_resources_type.CHAMP_ICON, item.Participants[0].ChampionId)));
            player_kda.Text = item.Participants[0].Stats.Kills + "/" + item.Participants[0].Stats.Deaths + "/" + item.Participants[0].Stats.Assists;
            player_lv.Text = "Lv:" + item.Participants[0].Stats.ChampLevel;
            player_gold.Text = "经济:" + item.Participants[0].Stats.GoldEarned;
            player_hurm.Text = "对英雄伤害:" + item.Participants[0].Stats.TotalDamageDealtToChampions;
            player_visionscore.Text = "视野:" + item.Participants[0].Stats.VisionScore;
        }
    }
}
