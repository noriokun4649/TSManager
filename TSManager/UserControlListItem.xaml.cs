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

namespace TSManager
{
    /// <summary>
    /// ListItem.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControlListItem : UserControl
    {
        public UserControlListItem()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Files file = (Files)DataContext;
            Util.OpenFile(file.FilePath, file.FileName);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Files file = (Files)DataContext;
            var wasu = file.Epinum == 0 ? "なし" : "第" + file.Epinum.ToString() + "話";
            var subtitle = file.TvSeriesInfo.LongCount() == 0 ? "なし" : file.TvSeriesInfo[0];
            MessageBox.Show(
            "ファイル名:" + file.FileName + "\n" +
            "ファイルパス:" + file.FilePath + "\n" +
            "サブタイトル1:" + subtitle + "\n" +
            "シリーズ名:" + file.TvSeries + "\n" +
            "話数:" + wasu + "\n" +
            "ジャンル1:" + file.Genres[0] + "\n" +
            "放送局:" + file.Company + "\n" +
            "放送開始時間:" + file.StartTime.ToString("yyyy年MM月dd日(dddd) tthh時mm分") + "\n" +
            "放送終了時間:" + file.EndTime.ToString("yyyy年MM月dd日(dddd) tthh時mm分") + "\n" +
            "放送時間:" + file.Length.ToString(@"hh'時間'mm'分'ss'秒'") + "\n"
            );
        }
    }
}
