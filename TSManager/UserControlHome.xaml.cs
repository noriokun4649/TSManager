
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            if (item is Files file)
            {
                Util.OpenFile(file.FilePath, file.FileName);
            }
        }

    }
}


