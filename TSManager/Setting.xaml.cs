using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
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
    /// Setting.xaml の相互作用ロジック
    /// </summary>
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();
            TVTestPath.Text = Properties.Settings.Default.TVTestPath;
            TVTPlayBondriver.Text=Properties.Settings.Default.TVTPlayBondriver ;
            Com.Text = Properties.Settings.Default.Com;
            SaveFolder.Text = Properties.Settings.Default.SaveFolder;
            FfmpegPath.Text = Properties.Settings.Default.FfmpegPath;
        }

        private void Button_Click_Ok(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.TVTestPath = TVTestPath.Text;
            Properties.Settings.Default.TVTPlayBondriver = TVTPlayBondriver.Text;
            Properties.Settings.Default.Com = Com.Text;
            Properties.Settings.Default.SaveFolder = SaveFolder.Text;
            Properties.Settings.Default.FfmpegPath = FfmpegPath.Text;
            Properties.Settings.Default.Save();
            Close();
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_File_Open(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "実行ファイルパス(TvTest、MPCやVLCなどの動画プレイヤーのパス)を取得する";
            dialog.Filters.Add(new CommonFileDialogFilter("実行ファイル", "*.exe")); 
            dialog.Filters.Add(new CommonFileDialogFilter("すべてのファイル", "*.*"));
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.TVTestPath.Text = dialog.FileName;
            }
        }
        private void Button_Click_Ffmpeg_Open(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("実行ファイル", "*.exe"));
            dialog.Filters.Add(new CommonFileDialogFilter("すべてのファイル", "*.*"));
            dialog.Title = "FFmpegのパスを取得する";
            dialog.DefaultFileName = "ffmpeg.exe";
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.FfmpegPath.Text = dialog.FileName;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Folder_Open(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = "録画保存フォルダを取得する";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.SaveFolder.Text = dialog.FileName;
            }
        }
    }
}
