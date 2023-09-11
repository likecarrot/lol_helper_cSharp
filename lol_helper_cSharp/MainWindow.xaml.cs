using lol_helper_cSharp;
using lol_helper_cSharp.helpers;
using lol_helper_cSharp.riot_apis;
using Newtonsoft.Json;
using QuickType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Clipboard = System.Windows.Forms.Clipboard;

namespace lol_helper_cSharp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon;
        private RiotApiManager apiManager;
        private Resources_Helper resources_Helper;
        private Settings_Config _config;
        public MainWindow()
        {
            InitializeComponent();
            apiManager = RiotApiManager.GetInstance();
            resources_Helper = new Resources_Helper();
            _config = Settings_Config.GetInstance();

            // 初始化托盘图标
            SetTrayIconContextMenu();
            update_ui_by_config();
            Run();
        }
        private void update_ui_by_config()
        {
            auto_accept_switch.IsChecked = _config.settings.AcceptStatus.AutoAcceptSwitch;
            matchhist_switch.IsChecked = _config.settings.MatchingHelper;
            aram_helper_switch.IsChecked = _config.settings.AramHelper.AramHelperSwitch;
            match_helper_switch.IsChecked = _config.settings.ClassicHelper.ClassicHelperSwitch;
        }
        private async void Run()
        {
            while (true)
            {
                await apiManager.GetAndProcessCommandLine();
                if (apiManager.Status == true)
                {
                    await Task.Delay(1000); // 等待5秒
                    var res = await apiManager.GetClientStatus();
                    try
                    {
                        var gameversion = await apiManager.GetGameVersion1();
                        int index = gameversion.Version.IndexOf('.', gameversion.Version.IndexOf('.') + 1); // 查找第二个小数点的位置
                        string result = gameversion.Version.Substring(0, index); // 取出从头到第二个小数点的字符串
                        dynamic_log.Text = result;
                        bool ret = await dynamic_change_skin.check_isenable(result);
                        if (ret == true)
                        {
                            bool ret2 = await dynamic_change_skin.download_dll(result);
                            if (ret2 == true)
                            {
                                progressBar1.Value = 100;
                                dynamic_skin_checkbox.IsEnabled = true;
                            }
                        }
                        game_status.Content = Consture.GetGameStatus(res);
                        SummonerInfo info = await apiManager.GetSummonerInfo();
                        current_player_name.Content = info.DisplayName;
                        current_player_level.Content = info.SummonerLevel;

                        BitmapImage bitmapImage1 = new BitmapImage(new Uri(await resources_Helper.DownloadAsync(Consture.gamedata_resources_type.ICON, info.ProfileIconId)));
                        current_player_icon.Source = bitmapImage1;

                        var profile = await apiManager.GetProfileSkin();
                        var profile_skin_path = await resources_Helper.DownloadAsync(Consture.gamedata_resources_type.SKIN, profile.BackgroundSkinId);
                        BitmapImage bitmapImage2 = new BitmapImage();
                        bitmapImage2.BeginInit();
                        bitmapImage2.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage2.UriSource = new Uri(profile_skin_path);
                        bitmapImage2.EndInit();
                        ImageBrush imageBrush = new ImageBrush();
                        imageBrush.ImageSource = bitmapImage2;
                        imageBrush.Stretch = Stretch.UniformToFill;
                        ProfileSkinWind.Background = imageBrush;

                        var rankdatas = await apiManager.GetRankdatasForPuuid();
                        current_player_solo5x5.Content = Consture.GetRankTire(rankdatas.QueueMap.RankedSolo5X5.Tier) + rankdatas.QueueMap.RankedSolo5X5.Division;
                        current_player_fix5x5.Content = Consture.GetRankTire(rankdatas.QueueMap.RankedFlexSr.Tier) + rankdatas.QueueMap.RankedFlexSr.Division;
                        current_player_tft.Content = Consture.GetRankTire(rankdatas.QueueMap.RankedTft.Tier) + rankdatas.QueueMap.RankedTft.Division;
                        current_player_tftturbo.Content = Consture.GetRankTire(rankdatas.QueueMap.RankedTftTurbo.Tier) + rankdatas.QueueMap.RankedTftTurbo.Division;
                        current_player_tftdouble.Content = Consture.GetRankTire(rankdatas.QueueMap.RankedTftDoubleUp.Tier) + rankdatas.QueueMap.RankedTftDoubleUp.Division;

                        reject_ui reject_Ui = null;
                        room_history room_Ui = null;
                        Consture.client_status _last_status = Consture.client_status.Error;
                        do
                        {
                            switch (res)
                            {
                                case Consture.client_status.ReadyCheck:
                                    if (_config.settings.AcceptStatus.AutoAcceptSwitch == true)
                                    {
                                        if (_config.settings.AcceptStatus.AcceptTimeoutSwitch == true)
                                        {
                                            if (reject_Ui == null)
                                            {
                                                reject_Ui = reject_ui.GetInstance(_config.settings.AcceptStatus.AcceptTimeout);
                                                LeagueOfLegendsWindow lolWindow = new LeagueOfLegendsWindow();
                                                if (lolWindow.IsFound())
                                                {
                                                    LeagueOfLegendsWindow.RECT rect = lolWindow.GetWindowRect();
                                                    reject_Ui.Left = rect.Left - 100;
                                                    reject_Ui.Top = rect.Top;
                                                    reject_Ui.Show();
                                                    reject_Ui.start();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            await apiManager.AcceptGame();
                                        }
                                    }
                                    break;
                                case Consture.client_status.ChampSelect:
                                    if (_config.settings.MatchingHelper)    //战绩助手
                                    {
                                        if (room_Ui == null)
                                        {
                                            room_Ui = new room_history();
                                            LeagueOfLegendsWindow lolWindow = new LeagueOfLegendsWindow();
                                            if (lolWindow.IsFound())
                                            {
                                                LeagueOfLegendsWindow.RECT rect = lolWindow.GetWindowRect();
                                                room_Ui.Left = rect.Right;
                                                room_Ui.Top = rect.Top;
                                                room_Ui.Show();
                                            }
                                        }
                                    }
                                    var game_mode = await apiManager.GetCurrentGameMode();
                                    if (_config.settings.AramHelper.AramHelperSwitch)
                                    {
                                        if (game_mode.Map.GameMode.Equals("ARAM"))   //大乱斗助手
                                        {
                                            Aram_Helper helper = new Aram_Helper();
                                            await helper.Run();
                                        }
                                    }
                                    if (_config.settings.ClassicHelper.ClassicHelperSwitch)
                                    {
                                        if (game_mode.Map.GameMode.Equals("CLASSIC"))    //匹配助手
                                        {
                                            Match_Helper helper = new Match_Helper();
                                            await helper.Run();
                                        }
                                    }
                                    break;
                                default:
                                    if (reject_Ui != null)
                                    {
                                        if (reject_Ui.IsActive)
                                        {
                                            reject_Ui.Close();
                                        }
                                        reject_Ui = null;
                                    }
                                    if (room_Ui != null)
                                    {
                                        room_Ui.Close();
                                        room_Ui = null;
                                    }
                                    break;
                            }
                            game_status.Content = Consture.GetGameStatus(res);

                            res = await apiManager.GetClientStatus();
                            do
                            {
                                await Task.Delay(500);
                                res = await apiManager.GetClientStatus();
                            } while (_last_status == res);
                            _last_status = res;
                        } while (apiManager.Status == true);

                        game_status.Content = "等待游戏启动中。。。";
                        if (reject_Ui != null)
                        {
                            if (reject_Ui.IsActive)
                            {
                                reject_Ui.Close();
                            }
                            reject_Ui = null;
                        }
                        if (room_Ui != null)
                        {
                            room_Ui.Close();
                            room_Ui = null;
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                    
                }
                await Task.Delay(3000); // 等待5秒
            }
        }
        // 设置托盘图标上下文菜单
        private void SetTrayIconContextMenu()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Text = "LoL o.0助手";
            _notifyIcon.Icon = Properties.Resources.Icon1; // 使用资源文件中的图标
            _notifyIcon.Visible = true;
            var contextMenu = new System.Windows.Forms.ContextMenu();
            var menuItem1 = new System.Windows.Forms.MenuItem("q群:758843151");
            menuItem1.Click += MenuItem1_Click;
            var menuItem2 = new System.Windows.Forms.MenuItem("Exit");
            menuItem2.Click += MenuItem2_Click;

            contextMenu.MenuItems.Add(menuItem1);
            contextMenu.MenuItems.Add(menuItem2);

            _notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
            _notifyIcon.ContextMenu = contextMenu;
        }
        
        // 菜单项1的点击事件处理
        private void MenuItem1_Click(object sender, EventArgs e)
        {// 要复制到剪贴板的文本
            string textToCopy = "758843151";
            // 将文本复制到剪贴板
            Clipboard.SetText(textToCopy);
        }
        private void MenuItem2_Click(object sender, EventArgs e)
        {
            _config.settings.AcceptStatus.AutoAcceptSwitch = (bool)auto_accept_switch.IsChecked;
            _config.settings.MatchingHelper = (bool)matchhist_switch.IsChecked;
            _config.settings.AramHelper.AramHelperSwitch = (bool)aram_helper_switch.IsChecked;
            _config.settings.ClassicHelper.ClassicHelperSwitch = (bool)match_helper_switch.IsChecked;
            _config.save_config();
            Environment.Exit(0);
        }
        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // 进行Visibility的反操作
            if (Visibility == Visibility.Collapsed)
            {
                Visibility = Visibility.Visible;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {// 按下按钮时，设置状态为false，并重新获取命令行
            if (apiManager.Status==true)
            {
                SettingsWindow newWindow = new SettingsWindow();
                newWindow.ShowDialog();
            }
            else
            {
                System.Windows.MessageBox.Show("注意打开游戏后才能设置");
            }
            
        }

        private void accept_checked(object sender, RoutedEventArgs e)
        {
            _config.settings.AcceptStatus.AutoAcceptSwitch = true;
        }

        private void accept_unchecked(object sender, RoutedEventArgs e)
        {
            _config.settings.AcceptStatus.AutoAcceptSwitch = false;
        }

        private void matchhist_checked(object sender, RoutedEventArgs e)
        {
            _config.settings.MatchingHelper = true;
        }

        private void matchhist_unchecked(object sender, RoutedEventArgs e)
        {
            _config.settings.MatchingHelper = false;
        }

        private void aram_helper_checked(object sender, RoutedEventArgs e)
        {
            _config.settings.AramHelper.AramHelperSwitch = true;
        }

        private void aram_helper_unchecked(object sender, RoutedEventArgs e)
        {
            _config.settings.AramHelper.AramHelperSwitch = false;
        }

        private void match_helper_checked(object sender, RoutedEventArgs e)
        {
            _config.settings.ClassicHelper.ClassicHelperSwitch = true;
        }

        private void match_helper_unchecked(object sender, RoutedEventArgs e)
        {
            _config.settings.ClassicHelper.ClassicHelperSwitch = false;
        }


        private async void check_player_matchhistory(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var sum = await apiManager.GetSummonerInfoByName(check_playername_matchhistory.Text);
                MyTeam t = new MyTeam();
                t.Puuid = sum.Puuid.ToString();
                t.SummonerId = sum.SummonerId;
                history_match_pop pop_ui = new history_match_pop(t);
                pop_ui.Show();
            }
        }

        private async void save_player_config(object sender, RoutedEventArgs e)
        {
            var conf = await apiManager.GetPlayerConfig();
            File.WriteAllText(resources_Helper.GetBaseRunPath() + "/player_config.json", JsonConvert.SerializeObject(conf));
        }

        private async void load_player_config(object sender, RoutedEventArgs e)
        {
            PlayerConfig c;
            if (resources_Helper.FileIsExist(resources_Helper.GetBaseRunPath() + "/player_config.json"))
            {
                c=PlayerConfig.FromJson(File.ReadAllText(resources_Helper.GetBaseRunPath() + "/player_config.json"));
                apiManager.SetPlayerConfig(c);
            }
        }

        private void dynamic_changed_skin_select(object sender, RoutedEventArgs e)
        {
            dynamic_change_skin.open();
        }

        private void dynamic_changed_skin_Unselect(object sender, RoutedEventArgs e)
        {
            dynamic_change_skin.close();
        }
    }
}
