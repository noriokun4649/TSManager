﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TSManager
{
    public partial class UserControlGenre : UserControl
    {
        public UserControlGenre()
        {
            InitializeComponent();
            Listbox.ItemsSource = Util.Data.Where(files => files.GenresIndex.Contains(0)).OrderBy(data => data.TvSeries).ThenBy(data => data.Epinum);
        }

        private void Mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Mode.SelectedIndex >= 0) {
                    Listbox.ItemsSource = Util.Data.Where(files => files.GenresIndex.Contains(Mode.SelectedIndex)).OrderBy(data => data.TvSeries).ThenBy(data => data.Epinum);

                }
                else
                {
                    Listbox.ItemsSource = Util.Data;
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("このジャンルのTSファイルを読み込み中のため正しく操作できません。読み込み完了後お試しください。");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = Listbox.SelectedItem;
            if (item is Files file)
            {
                Util.OpenFile(file.FilePath, file.FileName);
            }
        }
    }
}
