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

namespace lol_helper_cSharp
{
    /// <summary>
    /// reject_ui.xaml 的交互逻辑
    /// </summary>
    public partial class reject_ui : Window
    {
        private long time;
        private static reject_ui instance; // 唯一实例
        private static object lockObject = new object(); // 锁对象，用于线程安全
        private int _flag = 0;
        private static object _flag_lock = new object();
        private reject_ui(long _time)
        {
            InitializeComponent();
            time = _time * 1000;

            Console.WriteLine("call function:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " time:" + time);
        }

        public static reject_ui GetInstance(long _time)
        {
            lock (lockObject)
            {
                instance = new reject_ui(_time);
            }
            return instance;
        }

        public void start()
        {
            _rejectAsync();
        }

        private async Task _rejectAsync()
        {
            long now = 0;
            while (time > now)
            {
                now += 100;
                await Task.Delay(100);
            }
            if (_flag==0)
            {
                lock (_flag_lock)
                {
                    _flag = 1;
                }
            }
            if (_flag==1)
            {
                var apis = riot_apis.RiotApiManager.GetInstance();
                await apis.AcceptGame();
            }
            Close();
        }

        private async void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_flag == 0)
            {
                lock (_flag_lock)
                {
                    _flag = 2;
                }
            }
            if (_flag == 2)
            {
                var apis = riot_apis.RiotApiManager.GetInstance();
                await apis.DeclineGame();
            }
            Close();
        }
    }
}
