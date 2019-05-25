using System.Linq;
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
                Listbox.ItemsSource = Util.Data.Where(files => files.GenresIndex.Contains(Mode.SelectedIndex)).OrderBy(data => data.TvSeries).ThenBy(data => data.Epinum);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
