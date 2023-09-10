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
using System.Windows.Shapes;
using lol_helper_cSharp.riot_apis;
using QuickType;


namespace lol_helper_cSharp.helpers
{
    /// <summary>
    /// history_match_pop.xaml 的交互逻辑
    /// </summary>
    public partial class history_match_pop : Window
    {
        private MyTeam _team;
        private RiotApiManager apiManager;
        private HistoryMatchDatas history_list = new HistoryMatchDatas();
        public history_match_pop(MyTeam team)
        {
            InitializeComponent();
            apiManager = RiotApiManager.GetInstance();
            _team = team;
            init();
        }
        private async Task init()
        {
            var summoner = await apiManager.GetSummonerInfo(_team.SummonerId.ToString());
            this.Title = summoner.DisplayName;

            history_list = await apiManager.GetHistoryMatchingdatas(_team.Puuid, 0, 20);
            foreach (var item in history_list.Games.GamesGames)
            {
                match_list_item it = new match_list_item(item);
                match_list_container.Children.Add(it);
            }
        }
    }
}
