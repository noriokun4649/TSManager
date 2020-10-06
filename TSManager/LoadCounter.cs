using System.Windows;
using System.Windows.Controls;

namespace TSManager
{
    public class LoadCounter
    {
        private readonly int totalCount;
        private int nowCount;
        private int errorCount;
        private int warningCount;
        private ProgressBar bar1;
        private ProgressBar bar2;
        private TextBlock text;
        private Label label;


        public LoadCounter(ProgressBar bar1,ProgressBar bar2,TextBlock text,Label label,int max)
        {
            totalCount = max;
            this.bar1 = bar1;
            this.bar2 = bar2;
            this.text = text;
            this.label = label;
            UpdateCount();
        }

        public int NowCount { 
            get => nowCount;
            set
            {
                nowCount = value;
                UpdateCount();
            } 
        }
        public int ErrorCount { 
            get => errorCount;
            set
            { 
                errorCount = value;
                UpdateCount();
            }
        }
        public int WarningCount {
            get => warningCount; 
            set
            {
                warningCount = value;
                UpdateCount();
            }
        }

        private void UpdateCount()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                bar1.Maximum = totalCount;
                bar2.Maximum = totalCount;
                var progressCount = nowCount + errorCount;
                bar1.Value = progressCount;
                bar2.Value = progressCount;
                text.Text = totalCount + "件中" + (progressCount) + "件完了(エラー" + errorCount + "件/注意" + warningCount + "件)";
                label.Content = "TSファイル読み込み状況\n" + totalCount + "件中" + (progressCount) + "件完了\n(エラー" + errorCount + "件/注意" + warningCount + "件)";
            });
        }
    }
}
