using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TSManager
{
    public partial class UserControlDay : UserControl
    {
        IEnumerable<string> company;
        public UserControlDay()
        {
            InitializeComponent();
            company = Util.Data.Select(files => files.StartTime.ToString("yyyy年MM月dd日(dddd)")).OrderByDescending(data => data).Distinct();
            Mode.ItemsSource = company;
            Listbox.ItemsSource = Util.Data.Where(files => files.StartTime.ToLongDateString().Equals(company.ElementAt(0))).OrderBy(data => data.TvSeries).ThenBy(data => data.Epinum);
        }

        private void Mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Mode.SelectedIndex >= 0)
                {
                    Listbox.ItemsSource = Util.Data.Where(files => files.StartTime.ToString("yyyy年MM月dd日(dddd)").Equals(company.ElementAt(Mode.SelectedIndex))).OrderBy(data => data.TvSeries).ThenBy(data => data.Epinum);
                }
                else
                {
                    Listbox.ItemsSource = Util.Data;
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("この日付のTSファイルを読み込み中のため正しく操作できません。読み込み完了後お試しください。");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = Listbox.SelectedItem;
            switch (item)
            {
                case Files itemfile:
                    Util.OpenFile(itemfile.FilePath, itemfile.FileName);
                    break;
            }
        }
    }
}
