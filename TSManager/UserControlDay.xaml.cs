

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                Listbox.ItemsSource = Util.Data.Where(files => files.StartTime.ToString("yyyy年MM月dd日(dddd)").Equals(company.ElementAt(Mode.SelectedIndex))).OrderBy(data => data.TvSeries).ThenBy(data => data.Epinum);
            }
            catch
            {

            }

        }
        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = Listbox.SelectedItem;
            if (item is Files)
            {
                var file = (Files)item;
                Util.OpenFile(file.FilePath,file.FileName);
            }
        }
    }
}
