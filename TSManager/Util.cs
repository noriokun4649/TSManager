
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using TSManager.Properties;

namespace TSManager
{
    class Util
    {
        public static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static ObservableCollection<Files> Data = new ObservableCollection<Files>();
        public static string[] genres = {
            "ニュース／速報","スポーツ","情報／ワイドショー","ドラマ","音楽","バラエティ","映画","アニメ／特撮","ドキュメンタリー／教養","劇場／公演","趣味／教育","福祉","その他","なし"
        };
        public static string file;
        public static ObservableCollection<PlayData> time = new ObservableCollection<PlayData>();
        public static void Setup()
        {
            try
            {
                file = Path.GetTempFileName().Replace(".tmp", ".exe");
                var bin = Properties.Resources.rplsinfo;
                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    fs.Write(bin, 0, bin.Length);
                }
            }
            catch
            {
                MessageBox.Show("一時ファイルの作成に失敗しました。");
                logger.Fatal("rplsinfo.exeの一時ファイルを作成するのに失敗しました。");
            }
        }

        public static BitmapSource GetThumbnailForWindows(string inputMoviePath, LoadCounter loadCounter)
        {
            BitmapSource bitmapSource;
            try
            {
                var shellFile = ShellFile.FromFilePath(inputMoviePath);
                shellFile.Thumbnail.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;
                bitmapSource = shellFile.Thumbnail.ExtraLargeBitmapSource;
            }
            catch (Exception ex)
            {
                logger.Warn($"[{inputMoviePath}]サムネイル画像の取得に失敗しました。");
                var shellFile = ShellFile.FromFilePath(inputMoviePath);
                shellFile.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
                bitmapSource = shellFile.Thumbnail.ExtraLargeBitmapSource;
                loadCounter.WarningCount++;
                Console.WriteLine(ex.StackTrace);
            }
            return bitmapSource;
        }


        public static string GetCurrentAppDir()
        {
            return Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
        }

        public static void OpenFile(string path, string filename)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = Settings.Default.TVTestPath,
                Arguments = Settings.Default.TVTPlayBondriver + " " + Settings.Default.Com + @" """ + path + @""""
            };
            try
            {
                Process.Start(psi);
                DateTime now_time = DateTime.Now;
                time.Add(new PlayData(filename, now_time));
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("実行" + ex.Message + "\n設定画面からTvTestのパスを設定してください。");
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(ex.Message + "\n設定画面から実行ファイルのパスを設定しなおしてください。");
            }
        }
    }

    //StringFormat=yyyy年MM月dd日
    public sealed class PlayData
    {
        public PlayData(string filename, DateTime lastplaytime)
        {
            LastPlayTime = lastplaytime.ToString("yyyy年MM月dd日(dddd)");
            FileName = filename;
        }
        public PlayData(string filename, string lastplaytime)
        {
            LastPlayTime = lastplaytime;
            FileName = filename;
        }

        public string FileName { get; set; }
        public string LastPlayTime { get; set; }
    }

    public sealed class Files
    {
        public Files(string filename, string filepath, string tvSeries, string company, List<string> sirizeinfo,
            List<int> genresIndex, List<string> genres, TimeSpan length, DateTime starttime,
            DateTime endtime, DateTime lastplay, BitmapSource bitmap, int epinum)
        {
            FileName = filename;
            FilePath = filepath;
            TvSeries = tvSeries;
            TvSeriesInfo = sirizeinfo;
            Company = company;
            Genres = genres;
            Length = length;
            StartTime = starttime;
            EndTime = endtime;
            LastPlay = lastplay;
            GenresIndex = genresIndex;
            Bitmap = bitmap;
            Epinum = epinum;
        }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string TvSeries { get; set; }
        public List<string> TvSeriesInfo { get; set; }
        public string Company { get; set; }
        public List<string> Genres { get; set; }
        public TimeSpan Length { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime LastPlay { get; set; }
        public List<int> GenresIndex { get; set; }
        public BitmapSource Bitmap { get; set; }
        public int Epinum { get; set; }
    }
    /*
    public class MovieInfo
    {
        public string Path { get; }
        public int FrameRate { get; set; }
        public double SecCount { get; set; }
        public int FrameCount { get; set; }
        public int FrameHeight { get; set; }
        public int FrameWidth { get; set; }
        public Mat Thumbnail { get; set; } = new Mat();
        public MovieInfo(string path)
        {
            Path = path;
        }
    }*/
}
