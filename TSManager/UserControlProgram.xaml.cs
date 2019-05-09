

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace TSManager
{
    public partial class UserControlProgram : UserControl
    {
        IEnumerable<string> program;

        public UserControlProgram()
        {
            InitializeComponent();
            program = Util.Data.Select(files => files.TvSeries).Distinct();
            Mode.ItemsSource = program;
            Listbox.ItemsSource = Util.Data.Where(files => files.TvSeries.Equals(program.ElementAt(0))).OrderBy(data => data.TvSeries).ThenBy(data => data.Epinum);
        }

        private void Mode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Listbox.ItemsSource = Util.Data.Where(files => files.TvSeries.Equals(program.ElementAt(Mode.SelectedIndex))).OrderBy(data => data.TvSeries).ThenBy(data => data.Epinum);
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
                Util.OpenFile(file.FilePath);
            }
        }
    }
}
