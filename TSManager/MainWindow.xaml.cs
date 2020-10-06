using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TSManager
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private CancellationTokenSource tokenSource = null;
        public MainWindow()
        {
            InitializeComponent();
        Util.Setup();
            Util.logger.Info("アプリ起動");
            TopBar.MouseLeftButtonDown += (o, e) => DragMove();
            if (!File.Exists(Util.GetCurrentAppDir() + @"\history.txt"))
            {
                using (File.Create(Util.GetCurrentAppDir() + @"\history.txt")) { } 
            }
            LoadTxtFile();
            tokenSource = new CancellationTokenSource();
            LoadTs(tokenSource.Token);
        }


        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc;
            GridMain.Children.Clear();
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "ItemHome":
                    usc = new UserControlHome();
                    GridMain.Children.Add(usc);
                    TopText.Text = "Home";
                    break;
                case "ItemGenre":
                    usc = new UserControlGenre();
                    GridMain.Children.Add(usc);
                    TopText.Text = "番組のジャンル別";
                    break;
                case "ItemCompany":
                    usc = new UserControlCompany();
                    GridMain.Children.Add(usc);
                    TopText.Text = "番組の放送局別";
                    break;
                case "ItemPackage":
                    usc = new UserControlProgram();
                    GridMain.Children.Add(usc);
                    TopText.Text = "番組のシリーズ別";
                    break;
                case "ItemHistory":
                    usc = new UserControlGenre();
                    GridMain.Children.Add(usc);
                    TopText.Text = "番組の再生履歴";
                    break;
                case "ItemDay":
                    usc = new UserControlDay();
                    GridMain.Children.Add(usc);
                    TopText.Text = "日付別";
                    break;
                default:
                    break;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var setting = new Setting
            {
                Owner = this
            };
            setting.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            LoadTs(tokenSource.Token);
        }

        private List<string> LoadTxtFile()
        {
            List<string> list = new List<string>();
            try
            {
                using (var file = new StreamReader(Util.GetCurrentAppDir() + @"\history.txt"))
                {
                    while (!file.EndOfStream)
                    {
                        list.Add(file.ReadLine());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("エラーが発生しました。このエラーについての詳細は以下を参照してください。\n\n:" + ex.Message);
            }

            return list;
        }

        private void LoadTs(CancellationToken token)
        {
            BindingOperations.EnableCollectionSynchronization(Util.Data, new object());
            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    Util.Data.Clear();
                    GC.Collect();
                    var folder = Properties.Settings.Default.SaveFolder;
                    var blacklist = Properties.Settings.Default.BlackList.Split(',');
                    IEnumerable<string> files = Directory.EnumerateFiles(@folder,"*", SearchOption.AllDirectories).Where(name =>
                    name.EndsWith(".ts",StringComparison.CurrentCultureIgnoreCase) ||
                    name.EndsWith(".m2t", StringComparison.CurrentCultureIgnoreCase) ||
                    name.EndsWith(".mts", StringComparison.CurrentCultureIgnoreCase) ||
                    name.EndsWith(".m2ts", StringComparison.CurrentCultureIgnoreCase)).Where(names => {
                        foreach (var black in blacklist)
                        {
                            if (Regex.IsMatch(names, @".+\\" + black +@"\\.+")) return false;
                        }
                        return true;
                    });
                    LoadCounter loadCounter = new LoadCounter(progressDiag,progress,now,loadingText,files.Count());
                    foreach (string str in files)
                    {
                        try
                        {
                            token.ThrowIfCancellationRequested();
                            var program = new ReadTxtFile(str + ".program.txt");
                            var image = Util.GetThumbnailForWindows(str, loadCounter);//Windows標準のサムネイル取得
                            image.Freeze();
                            Util.Data.Add(new Files(program.Title, str, program.Series, program.Company, program.SeriesInfo,
                                program.GenreIndex, program.Genre, program.Length, program.Starttime, program.Endtime, DateTime.Now, image, program.Epinum));
                            loadCounter.NowCount++;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            loadCounter.ErrorCount++;
                            Util.logger.Error($"[{str}.program.txt]EDCB録画結果ファイルのパースに失敗しました。");
                        }
                        catch (FormatException)
                        {
                            loadCounter.ErrorCount++;
                            Util.logger.Error($"[{str}.program.txt]EDCB録画結果ファイルのパースに失敗しました。");
                        }
                        catch (FileNotFoundException)
                        {
                            loadCounter.ErrorCount++;
                            Util.logger.Error($"[{str}.program.txt]EDCB録画結果ファイルが見つかりませんでした。EDCBにて録画時に番組情報を保存するようにしてあるか確認してください。");
                        }
                        catch (IOException ex)
                        {
                            loadCounter.ErrorCount++;
                            Util.logger.Error($"[{str}]ファイルを開く際にIOエラーが発生しました。 IOエラー詳細：{ex.Message}");
                        }
                        catch (NullReferenceException)
                        {
                            loadCounter.ErrorCount++;
                            Util.logger.Error($"[{str}]やべぇーヌルッてしまった。。。ということで解析不能なTSファイルがありました。");
                        }
                        catch (InvalidOperationException)
                        {
                            loadCounter.ErrorCount++;
                            Util.logger.Warn($"[{str}]サムネイルが取得出来なかったためTSアイコンに差し替えました。");
                        }
                        catch (AggregateException)
                        {
                            loadCounter.ErrorCount++;
                            Util.logger.Error($"[{str}]TSファイル内から番組情報を取得しようとしましたが見つかりませんでした。");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Dispatcher.Invoke(() =>
                    {
                        loadingText.Content = "TSファイル読み込み状況\nキャンセル";
                    });
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("設定画面にて録画保存フォルダの設定がされていないようです。設定しなおしてください。");
                    Util.logger.Warn("設定画面にて録画保存フォルダの設定がされていないようです。設定しなおしてください。");
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show("設定で指定されている録画保存フォルダのパスが正しくないようです。設定しなおしてください。\n");
                    Util.logger.Warn("設定で指定されている録画保存フォルダのパスが正しくないようです。設定しなおしてください。");
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("ファイルにアクセスする権利がありません。このアプリを管理者で実行するかアクセス制御を見直してください。");
                    Util.logger.Warn("ファイルにアクセスする権利がありません。このアプリを管理者で実行するかアクセス制御を見直してください。");
                }
                catch (SecurityException)
                {
                    MessageBox.Show("ファイルにアクセスする権利がありません。このアプリを管理者で実行するかアクセス制御を見直してください。");
                    Util.logger.Warn("ファイルにアクセスする権利がありません。このアプリを管理者で実行するかアクセス制御を見直してください。");
                }
                catch (IOException ex)
                {
                    MessageBox.Show("ファイルを開く際にIOエラーが発生しました。\n\nIOエラー詳細：" + ex.Message);
                    Util.logger.Error($"ファイルを開く際にIOエラーが発生しました。 IOエラー詳細：{ex.Message}");
                }
                Dispatcher.Invoke(() =>
                {
                    CancelButton.IsEnabled = true;
                    Diag.IsOpen = false;
                });
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            File.Delete(Util.file);
            Util.logger.Info("アプリ終了");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                tokenSource.Dispose();
            }
            // free native resources
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

            var setting = new OpenSource
            {
                Owner = this
            };
            setting.ShowDialog();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var setting = new Version
            {
                Owner = this
            };
            setting.ShowDialog();

        }
    }
}
