using lol_helper_cSharp.riot_apis;
using QuickType;
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

namespace lol_helper_cSharp.helpers
{
    /// <summary>
    /// room_history.xaml 的交互逻辑
    /// </summary>
    public partial class room_history : Window
    {
        private ChampSelectSession session;
        private RiotApiManager apiManager;
        private List<Room_History_Item> list_ = new List<Room_History_Item>();
        public room_history()
        {
            InitializeComponent();
            apiManager = RiotApiManager.GetInstance();
            getRoomPlayer();
        }
        private async   Task getRoomPlayer()
        {
            session=await apiManager.GetTeamSessions();
            foreach (var item in session.MyTeam)
            {
                Room_History_Item item_ui = new Room_History_Item(item);
                list_.Add(item_ui);
                container.Children.Add(item_ui);
            }
        }
        private void ParentWindow_Closed(object sender, EventArgs e)
        {
            foreach (var item in list_)
            {
                item.Dispose();
            }
        }
    }
}
