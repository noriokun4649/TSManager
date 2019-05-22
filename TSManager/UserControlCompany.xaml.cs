using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace TSManager
{
    public partial class UserControlCompany : UserControl
    {
        IEnumerable<string> company;
        public UserControlCompany()
        {
            InitializeComponent();
            company = Util.Data.Select(files => files.Company).Distinct();
            Mode.ItemsSource = company;
            Listbox.ItemsSource = Util.Data.Where(files => files.Company.Equals(company.ElementAt(0))).OrderBy(data => data.TvSeries).ThenBy(data => data.Epinum);
        }

        private void Mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Listbox.ItemsSource = Util.Data.Where(files => files.Company.Equals(company.ElementAt(Mode.SelectedIndex))).OrderBy(data => data.TvSeries).ThenBy(data => data.Epinum);
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
