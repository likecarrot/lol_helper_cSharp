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
    /// select_lova.xaml 的交互逻辑
    /// </summary>
    public partial class select_lova : Window
    {

        private List<ChampList> sortLists_ = new List<ChampList>();
        private ChampList[] champLists_;
        public List<LoveChamp> lovaLists_ { get; set; } = new List<LoveChamp>();

        private string _last_input;
        private int _last_index = 0;
        public select_lova(ChampList[] champLists)
        {
            InitializeComponent();
            champLists_ = new ChampList[champLists.Length - 1];
            Array.Copy(champLists, 1, champLists_, 0, champLists.Length - 1);
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
                Close();
            }
        }

        private void TextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 阻止输入框的文本变化
            e.Handled = true;

            // 获取当前文本框中的数字值
            int currentValue = 0;
            int.TryParse(lova_score.Text, out currentValue);

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
            lova_score.Text = currentValue.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_last_input == check_champ_input.Text && sortLists_.Count != 0)
            {
                _last_index++;
                display_champname.Content = sortLists_[_last_index % sortLists_.Count].Name;
            }
            else
            {
                _last_index = 0;
                sortLists_.Clear();
                foreach (var item in champLists_)
                {
                    if (item.Name.Contains(check_champ_input.Text))
                    {
                        sortLists_.Add(item);
                    }
                }
                if (sortLists_.Count != 0)
                {
                    display_champname.Content = sortLists_.FirstOrDefault<ChampList>().Name;
                }
                _last_input = check_champ_input.Text;
            }
        }

        private void check_champ_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sortLists_.Count != 0)
            {
                display_champname.Content = (sortLists_.FirstOrDefault<ChampList>()).Name;
            }
        }

        private long GetChampIdByChampName(string champ_name)
        {
            foreach (var item in champLists_)
            {
                if (item.Name == champ_name)
                {
                    return item.Id;
                }
            }
            return 0;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoveChamp lova = new LoveChamp
            {
                ChampId = GetChampIdByChampName((string)display_champname.Content),
                LoveScore = long.Parse(lova_score.Text),
                ChampName = (string)display_champname.Content
            };
            lovaLists_.Add(lova);
            add_list_log.Children.Add(new TextBlock
            {
                Text = "英雄名: " + lova.ChampName + "   好感度: " + lova_score.Text
            });
        }
    }
}
