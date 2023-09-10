using QuickType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace lol_helper_cSharp.helpers
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private bool accept_timeout_switch_bool; //延时 启用 开关
        private long accept_timeout_long = 0;      //延时 时间 秒
        private bool locked_autolock_timeout_switch_bool; //自动锁定延时 弃用 开关 
        private long locked_autolock_timeout_long = 0;  //自动锁定 延时 秒
        private long reroller_long = 1;
        private long get_lovachamp_timeout = 0;
        private List<LoveChamp> lova_list;
        private ChampList[] champLists = new ChampList[] { };

        private Settings_Config _settings;
        public SettingsWindow()
        {
            InitializeComponent();
            _settings = Settings_Config.GetInstance();
            InitializeAsync();
        }
        private void update_ui_by_config()
        {
            accept_timeout_switch_bool = _settings.settings.AcceptStatus.AcceptTimeoutSwitch;
            accept_timeout_long = _settings.settings.AcceptStatus.AcceptTimeout;

            locked_autolock_timeout_switch_bool = _settings.settings.ClassicHelper.ClassicConfig.AutoLock;
            locked_autolock_timeout_long = _settings.settings.ClassicHelper.ClassicConfig.LockTimeout;

            reroller_long = _settings.settings.AramHelper.AramConfig.UseReroller;
            get_lovachamp_timeout = _settings.settings.AramHelper.AramConfig.WaitMaxSec;

            accept_timeout_switch.IsChecked = accept_timeout_switch_bool;
            accept_timeout.Text = accept_timeout_long.ToString();
            locked_autolock_timeout_switch.IsChecked = locked_autolock_timeout_switch_bool;
            locked_autolock_timeout.Text = locked_autolock_timeout_long.ToString();

            lova_list = _settings.settings.AramHelper.AramConfig.LoveChamps;

            switch (reroller_long)
            {
                case 2:
                    reroller2.IsChecked = true;
                    break;
                case 3:
                    reroller3.IsChecked = true;
                    break;
                default:
                    reroller1.IsChecked = true;
                    break;
            }
            if (lova_list != null && lova_list.Count != 0)
            {
                foreach (var item in lova_list)
                {
                    if (item.LoveScore!=0)
                    {
                        ComboBoxItem item3 = new ComboBoxItem();
                        item3.Content = item.ChampName;
                        item3.Content += " " + item.LoveScore;
                        lova_champs_list.Items.Add(item3);
                    }
                    
                }
            }
            else
            {
                lova_list = new List<LoveChamp>();
            }
            if (_settings.settings.ClassicHelper.ClassicConfig.ChampId != 0)
            {
                lock_champ.Text = GetChampNameByChampId(_settings.settings.ClassicHelper.ClassicConfig.ChampId);
            }


        }
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (true)
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("未保存，需要保存吗?", "提示", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        save_all();
                    }
                    Close();
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void save_all()
        {
            _settings.settings.AcceptStatus.AcceptTimeoutSwitch = accept_timeout_switch_bool;

            accept_timeout_long = int.Parse(accept_timeout.Text);
            _settings.settings.AcceptStatus.AcceptTimeout = accept_timeout_long;

            locked_autolock_timeout_long = int.Parse(locked_autolock_timeout.Text);
            _settings.settings.ClassicHelper.ClassicConfig.AutoLock = locked_autolock_timeout_switch_bool;
            _settings.settings.ClassicHelper.ClassicConfig.LockTimeout = locked_autolock_timeout_long;

            if (reroller1.IsChecked == true)
            {
                reroller_long = 1;
            }
            else if (reroller2.IsChecked == true)
            {
                reroller_long = 2;
            }
            else if (reroller3.IsChecked == true)
            {
                reroller_long = 3;
            }
            _settings.settings.AramHelper.AramConfig.UseReroller = reroller_long;

            get_lovachamp_timeout = int.Parse(try_get_lovachamp_timeout.Text);
            _settings.settings.AramHelper.AramConfig.WaitMaxSec = get_lovachamp_timeout;
            if (_settings.settings.AramHelper.AramConfig.LoveChamps == null)
            {
                _settings.settings.AramHelper.AramConfig.LoveChamps = new List<LoveChamp>();
            }
            foreach (var item in lova_list)
            {
                _settings.settings.AramHelper.AramConfig.LoveChamps.Add(item);
            }
            _settings.settings.ClassicHelper.ClassicConfig.ChampId = GetChampIdByChampName(lock_champ.Text);

            _settings.settings.AramHelper.AramConfig.LoveChamps = _settings.settings.AramHelper.AramConfig.LoveChamps
    .GroupBy(c => c.ChampId)
    .ToDictionary(g => g.Key, g => g.Last())
    .Values
    .ToList();
            var loveChamps = _settings.settings.AramHelper.AramConfig.LoveChamps;
            for (int i = loveChamps.Count - 1; i >= 0; i--)
            {
                var item = loveChamps[i];
                if (item.LoveScore == 0)
                {
                    loveChamps.RemoveAt(i);
                }
            }
            _settings.save_config();
        }
        private async void InitializeAsync()
        {
            champLists = await riot_apis.RiotApiManager.GetInstance().GetAllChamps();
            update_ui_by_config();

            List<string> list = new List<string>();
            foreach (var item in champLists)
            {
                list.Add(item.Name);
            }
            lock_champ.ItemsSource = list;
        }

        private string GetChampNameByChampId(long champ_id)
        {
            if (champLists != null && champLists.Count() > 0)
            {
                foreach (var item in champLists)
                {
                    if (item.Id == champ_id)
                    {
                        return item.Name;
                    }
                }
            }
            return default(string);
        }
        private long GetChampIdByChampName(string name)
        {
            if (champLists != null && champLists.Count() > 0)
            {
                foreach (var item in champLists)
                {
                    if (item.Name == name)
                    {
                        return item.Id;
                    }
                }
            }
            return default(long);
        }
        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // 阻止键盘输入
            e.Handled = true;
        }


        private void locked_autolock_timeout_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 阻止输入框的文本变化
            e.Handled = true;

            // 获取当前文本框中的数字值
            int currentValue = 0;
            int.TryParse(locked_autolock_timeout.Text, out currentValue);

            // 根据滚轮滚动方向增加或减少数字的值
            if (e.Delta > 0)
            {
                // 向上滚动，增加数字
                currentValue++;
            }
            else
            {
                // 向下滚动，减少数字
                currentValue--;
            }
            if (currentValue >= 10)
            {
                currentValue = 10;
            }
            else if (currentValue < 0)
            {
                currentValue = 0;
            }

            // 更新文本框中的值
            locked_autolock_timeout.Text = currentValue.ToString();
        }

        private void try_get_lovachamp_timeout_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 阻止输入框的文本变化
            e.Handled = true;

            // 获取当前文本框中的数字值
            int currentValue = 0;
            int.TryParse(try_get_lovachamp_timeout.Text, out currentValue);

            // 根据滚轮滚动方向增加或减少数字的值
            if (e.Delta > 0)
            {
                // 向上滚动，增加数字
                currentValue++;
            }
            else
            {
                // 向下滚动，减少数字
                currentValue--;
            }
            if (currentValue >= 30)
            {
                currentValue = 30;
            }
            else if (currentValue < 0)
            {
                currentValue = 0;
            }

            // 更新文本框中的值
            try_get_lovachamp_timeout.Text = currentValue.ToString();
        }

        private void accept_timeout_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 阻止输入框的文本变化
            e.Handled = true;

            // 获取当前文本框中的数字值
            int currentValue = 0;
            int.TryParse(accept_timeout.Text, out currentValue);

            // 根据滚轮滚动方向增加或减少数字的值
            if (e.Delta > 0)
            {
                // 向上滚动，增加数字
                currentValue++;
            }
            else
            {
                // 向下滚动，减少数字
                currentValue--;
            }
            if (currentValue >= 10)
            {
                currentValue = 10;
            }
            else if (currentValue < 0)
            {
                currentValue = 0;
            }
            // 更新文本框中的值
            accept_timeout.Text = currentValue.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            save_all();
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }





        private void Checked_accept_timeout_switch(object sender, RoutedEventArgs e)
        {
            accept_timeout_switch_bool = true;
        }
        private void Unchecked_accept_timeout_switch(object sender, RoutedEventArgs e)
        {
            accept_timeout_switch_bool = false;
        }

        private void Cheched_locked_autolock_timeout_switch(object sender, RoutedEventArgs e)
        {
            locked_autolock_timeout_switch_bool = true;
        }

        private void Cncheched_locked_autolock_timeout_switch(object sender, RoutedEventArgs e)
        {
            locked_autolock_timeout_switch_bool = false;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置对话框的标题
            openFileDialog.Title = "选择文件";

            // 设置对话框的初始目录
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;

            // 设置对话框可选择的文件类型
            openFileDialog.Filter = "配置文件 (*.json)|*.json";

            // 设置是否允许选择多个文件
            openFileDialog.Multiselect = false;

            // 弹出文件对话框并获取用户选择的文件
            DialogResult result = openFileDialog.ShowDialog();

            // 处理用户选择的文件
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                _settings.load_config(selectedFilePath);
                Console.WriteLine("用户选择的文件路径： " + selectedFilePath);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // 设置对话框的标题
            saveFileDialog.Title = "保存文件";

            // 设置对话框的初始目录
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory;

            // 设置对话框的默认文件名称
            saveFileDialog.FileName = "config.json";

            // 设置对话框可选择的文件类型
            saveFileDialog.Filter = "配置文件 (*.json)|*.json";

            // 弹出保存文件对话框并获取用户选择的文件路径
            DialogResult result = saveFileDialog.ShowDialog();

            // 处理用户选择的文件
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFilePath = saveFileDialog.FileName;
                _settings.save_config(selectedFilePath);
                Console.WriteLine("用户选择的文件路径： " + selectedFilePath);
            }
        }

        private void select_champs_Click(object sender, RoutedEventArgs e)
        {
            select_lova lova_ui = new select_lova(champLists);
            bool? ret = lova_ui.ShowDialog();
            if (ret == false)
            {
                foreach (var item in lova_ui.lovaLists_)
                {
                    lova_list.Add(item);
                }
                lova_list = lova_list.GroupBy(x => x.ChampId)
                     .Select(g => g.OrderByDescending(x => x.LoveScore).Last())
                     .ToList();
                lova_champs_list.Items.Clear();
                foreach (var item in lova_list)
                {
                    ComboBoxItem item3 = new ComboBoxItem();
                    item3.Content = item.ChampName;
                    item3.Content += " " + item.LoveScore;
                    lova_champs_list.Items.Add(item3);
                }
            }
        }
    }
}
