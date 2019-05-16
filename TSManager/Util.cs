using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TSManager
{
    class Util
    {
        public static ObservableCollection<Files> Data = new ObservableCollection<Files>();
        public static string[] genres = {
            "ニュース／速報","スポーツ","情報／ワイドショー","ドラマ","音楽","バラエティ","映画","アニメ／特撮","ドキュメンタリー／教養","劇場／公演","趣味／教育","福祉","その他","なし"
        };
        public static ObservableCollection<PlayData> time = new ObservableCollection<PlayData>();
        
        public static Bitmap ReadMovieInfo(string moviePath)
        {
            var movie = new MovieInfo(moviePath);
            using (var vc = new VideoCapture(movie.Path))
            {
                if (vc.IsOpened())
                {
                    movie.FrameRate = (int)vc.Get(CaptureProperty.Fps);
                    movie.FrameCount = (int)vc.Get(CaptureProperty.FrameCount);
                    movie.FrameHeight = (int)vc.Get(CaptureProperty.FrameHeight);
                    movie.FrameWidth = (int)vc.Get(CaptureProperty.FrameWidth);
                    movie.SecCount = movie.FrameCount / movie.FrameRate;
                    // 再生時間1秒時点のキャプチャーを取得
                    var frameNo = (movie.FrameRate <= movie.FrameCount ? movie.FrameRate : 0);
                    vc.Set(CaptureProperty.PosFrames, 10 * frameNo);
                    vc.Read(movie.Thumbnail);
                    var dispSize = new OpenCvSharp.Size(680, 383);
                    var dispImg = movie.Thumbnail.Resize(dispSize);
                    vc.Dispose();
                    return BitmapConverter.ToBitmap(dispImg);
                }
            }
            return null;
        }
        public static BitmapSource Convert(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();
            return bitmapSource;
        }
        public static string GetCurrentAppDir()
        {
            return System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        public static void OpenFile(string path,string filename)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = Properties.Settings.Default.TVTestPath;
            psi.Arguments = Properties.Settings.Default.TVTPlayBondriver + " " + Properties.Settings.Default.Com + @" """ + path + @"""";
            try
            {
                System.Diagnostics.Process.Start(psi);
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
            DateTime endtime, DateTime lastplay, BitmapSource bitmap,int epinum)
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
    }
}
