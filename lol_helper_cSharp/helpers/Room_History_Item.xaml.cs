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
using lol_helper_cSharp.riot_apis;
using lol_helper_cSharp.helpers;
using QuickType;


namespace lol_helper_cSharp.helpers
{
    /// <summary>
    /// Room_History_Item.xaml 的交互逻辑
    /// </summary>
    public partial class Room_History_Item : UserControl
    {
        private RiotApiManager apiManager;
        private Resources_Helper resourcesManager;
        private MyTeam info_;
        private history_match_pop pop_ui = null;
        public  Room_History_Item(MyTeam info)
        {
            InitializeComponent();
            info_ = info;
            resourcesManager = new Resources_Helper();
            apiManager = riot_apis.RiotApiManager.GetInstance();
            init();
        }
        private async   Task init()
        {
            var summoner = await apiManager.GetSummonerInfo(info_.SummonerId.ToString());
            player_name.Text = summoner.DisplayName;
            player_level.Text = "Level: " + summoner.SummonerLevel;
            var rank = await apiManager.GetRankdatasForPuuid(info_.Puuid);
            now_rank5x5.Text = Consture.GetRankTire(rank.QueueMap.RankedSolo5X5.Tier) + rank.QueueMap.RankedSolo5X5.Division;
            now_rankfix5x5.Text = Consture.GetRankTire(rank.QueueMap.RankedFlexSr.Tier) + rank.QueueMap.RankedFlexSr.Division;
            last_rank5x5.Text= Consture.GetRankTire(rank.QueueMap.RankedSolo5X5.PreviousSeasonEndTier) + rank.QueueMap.RankedSolo5X5.PreviousSeasonEndDivision;
            last_rankfix5x5.Text= Consture.GetRankTire(rank.QueueMap.RankedFlexSr.PreviousSeasonEndTier) + rank.QueueMap.RankedFlexSr.PreviousSeasonEndDivision;

            var top=await apiManager.GetTopChamps(info_.SummonerId.ToString(), 6);
            foreach (var item in top.Masteries)
            {
                Image img = new Image();
                img.Width = 40;
                img.Height = 40;
                img.Margin = new Thickness(5, 0, 0, 0);
                img.Source = new BitmapImage(new Uri(await resourcesManager.DownloadAsync(Consture.gamedata_resources_type.CHAMP_ICON, item.ChampionId)));
                img_container.Children.Add(img);
            }
        }

        

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (pop_ui==null)
            {
                pop_ui = new history_match_pop(info_);
                pop_ui.Show();
            }
            else
            {
                pop_ui.Activate();
            }
        }
    }
}
