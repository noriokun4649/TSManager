﻿
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TSManager
{
    /// <summary>
    /// Interação lógica para UserControlHome.xam
    /// </summary>
    public partial class UserControlHome : UserControl
    {
        public UserControlHome()
        {
            InitializeComponent();
            ListBox.ItemsSource = Util.Data.OrderBy(data => data.TvSeries).ThenBy(data => data.Epinum);
            GC.Collect();
        }

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ListBox.SelectedItem;
            if (item is Files)
            {
                var file = (Files)item;
                Util.OpenFile(file.FilePath);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ListBox.SelectedItem;
            if (item is Files)
            {
                var file = (Files)item;
                var wasu = file.Epinum == 0 ? "なし" : "第"+file.Epinum.ToString()+"話";
                var subtitle = file.TvSeriesInfo.LongCount() == 0 ? "なし" : file.TvSeriesInfo[0];
                MessageBox.Show(
                "ファイル名:" + file.FileName + "\n" +
                "ファイルパス:" + file.FilePath + "\n" +
                "サブタイトル1:" + subtitle + "\n" +
                "シリーズ名:" + file.TvSeries + "\n" +
                "話数:" + wasu + "\n" +
                "ジャンル1:" + file.Genres[1] + "\n" +
                "放送局:" + file.Company + "\n" +
                "放送開始時間:" + file.StartTime.ToString("yyyy年MM月dd日(dddd) tthh時mm分") + "\n" +
                "放送終了時間:" + file.EndTime.ToString("yyyy年MM月dd日(dddd) tthh時mm分") + "\n" +
                "放送時間:" + file.Length.ToString(@"hh'時間'mm'分'ss'秒'") + "\n"
                );
            }

        }
    }
}


