﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TSManager
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TopBar.MouseLeftButtonDown += (o, e) => DragMove();
            if (!File.Exists(Util.GetCurrentAppDir() + @"\history.txt"))
            {
                using (File.Create(Util.GetCurrentAppDir() + @"\history.txt")) { } 
            }
            LoadTxtFile();
            LoadTs();
        }


        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            GridMain.Children.Clear();
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "ItemHome":
                    usc = new UserControlHome();
                    GridMain.Children.Add(usc);
                    Top.Text = "Home";
                    break;
                case "ItemGenre":
                    usc = new UserControlGenre();
                    GridMain.Children.Add(usc);
                    Top.Text = "番組のジャンル別";
                    break;
                case "ItemCompany":
                    usc = new UserControlCompany();
                    GridMain.Children.Add(usc);
                    Top.Text = "番組の放送局別";
                    break;
                case "ItemPackage":
                    usc = new UserControlProgram();
                    GridMain.Children.Add(usc);
                    Top.Text = "番組のシリーズ別";
                    break;
                case "ItemHistory":
                    usc = new UserControlGenre();
                    GridMain.Children.Add(usc);
                    Top.Text = "番組の再生履歴";
                    break;
                case "ItemDay":
                    usc = new UserControlDay();
                    GridMain.Children.Add(usc);
                    Top.Text = "日付別";
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
            var setting = new Setting();
            setting.Owner = this;
            setting.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            LoadTs();
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

        private void LoadTs()
        {
            BindingOperations.EnableCollectionSynchronization(Util.Data, new object());
            var task = Task.Factory.StartNew(() =>
            {
                try
                {

                    Util.Data.Clear();
                    var folder = Properties.Settings.Default.SaveFolder;
                    IEnumerable<string> files = Directory.EnumerateFiles(@folder, "*.ts");
                    foreach (string str in files)
                    {
                        try
                        {
                            var bitmap = Util.ReadMovieInfoFfmpeg(str);//FFmpeg使うように。インターレース解除も
                            if (bitmap == null) throw new NullReferenceException();
                            var image = Util.Convert(bitmap);
                            image.Freeze();
                            var program = new ReadTxtFile(str + ".program.txt");
                            Util.Data.Add(new Files(program.Title, str, program.Series, program.Company, program.SeriesInfo,
                                program.GenreIndex, program.Genre, program.Length, program.Starttime, program.Endtime, DateTime.Now, image, program.Epinum));
                        }
                        catch (IndexOutOfRangeException)
                        {
                            MessageBox.Show(str + ".program.txtファイルのパースに失敗しました。");
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show(str + ".program.txtファイルのパースに失敗しました。");
                        }
                        catch (FileNotFoundException)
                        {
                            MessageBox.Show(str + ".program.txtファイルが見つかりませんでした。EDCBにて録画時に番組情報を保存するようにしてあるか確認してください。");
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show("ファイルを開く際にIOエラーが発生しました。\n\nIOエラー詳細：" + ex.Message);
                        }
                        catch (NullReferenceException)
                        {
                            MessageBox.Show("やべぇーヌルッてしまった。。。ということで解析不能なTSファイルがありました。\n\n" + str);
                        }

                    }
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("設定画面にて録画保存フォルダの設定がされていないようです。設定しなおしてください。");
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show("設定で指定されている録画保存フォルダのパスが正しくないようです。設定しなおしてください。");
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("ファイルにアクセスする権利がありません。このアプリを管理者で実行するかアクセス制御を見直してください。");
                }
                catch (SecurityException)
                {
                    MessageBox.Show("ファイルにアクセスする権利がありません。このアプリを管理者で実行するかアクセス制御を見直してください。");
                }
                catch (IOException ex)
                {
                    MessageBox.Show("ファイルを開く際にIOエラーが発生しました。\n\nIOエラー詳細：" + ex.Message);
                }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}