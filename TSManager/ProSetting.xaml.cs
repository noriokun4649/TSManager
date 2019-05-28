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

namespace TSManager
{
    /// <summary>
    /// ProSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class ProSetting : Window
    {
        public ProSetting()
        {
            InitializeComponent();
            FFmpegPara.Text = Properties.Settings.Default.FfmpegPara;
            FFmpegSec.Text = Properties.Settings.Default.FfmpegSec;
            FFmpegWaitSec.Text = Properties.Settings.Default.FFmpegWaitSec.ToString();
        }
        private void Button_Click_Ok(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.FfmpegPara = FFmpegPara.Text;
            Properties.Settings.Default.FfmpegSec = FFmpegSec.Text;
            Properties.Settings.Default.FFmpegWaitSec = int.Parse(FFmpegWaitSec.Text);
            Properties.Settings.Default.Save();
            Close();
        }
        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
