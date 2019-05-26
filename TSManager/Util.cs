
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TSManager.Properties;

namespace TSManager
{
    class Util
    {
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
                WriteLog("一時ファイルの作成に失敗しました。", "rplsinfo.exe");
            }
        }

        public static BitmapSource Convert(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);
            //PixelFormats.Bgr32はOpenCVならPixelFormats.Bgr24つかう
            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();
            bitmapSource.Freeze();
            return bitmapSource;
        }
        /// <summary>動画ファイルからパイプを用いて画像を抽出する</summary>
        public static Bitmap ReadMovieInfoFfmpeg(string inputMoviePath)//680:383
        {
            //var arguments = $"-ss 10 -i \"{inputMoviePath}\" -filter_complex \"dejudder,fps=30000/1001,fieldmatch,yadif=0:-1:1,decimate,fps=24000/1001,scale=680:383\" -vframes 1  -f image2 pipe:1";
            var arguments = $"-ss 10 -i \"{inputMoviePath}\" -filter_complex \"pullup,dejudder,fps=24000/1001,scale=680:383\" -vframes 1  -f image2 pipe:1";
            //ここでFFmpeg利用。逆テレシネでFPSによるブレと、インターレースによる横線を取り除いてサムネイルを取得。680　383
            using (var process = new Process())
            {
                try
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = Settings.Default.FfmpegPath,
                        Arguments = arguments,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                    };
                    process.Start();
                    var id = process.Id;
                    var task = Task.Factory.StartNew(() => {
                        var pr = Process.GetProcessById(id);
                        Thread.Sleep(3000);
                        pr.Kill();
                        Util.WriteLog("指定した時間内に応答しなかったためプロセスを強制終了しました。",id.ToString());
                    });
                    var image = Image.FromStream(process.StandardOutput.BaseStream);
                    return new Bitmap(image);
                }
                catch (InvalidOperationException)
                {
                    WriteLog("FFMpegが見つからないため処理できません。", inputMoviePath);
                    return null;
                }
                catch (Exception ex)
                {
                    WriteLog($"スクランブル解除されていないか。TSファイルを正常に読み取れませんでした。 {ex.Message}", inputMoviePath);
                    return null;
                }
            }
        }
        public static string GetCurrentAppDir()
        {
            return Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
        }

        public static void WriteLog(string message,string filename)
        {
            try
            {
                var dateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                using (StreamWriter streamWriter = File.AppendText(GetCurrentAppDir() + @"\logs.txt"))
                {
                    streamWriter.WriteLine($@"{dateTime}, {filename} ,{message}");
                }
            }
            catch
            {
                MessageBox.Show("ログを書き込み出来ませんでした。書き込めなかった内容："+ message);
            }
        }
        public static void OpenFile(string path, string filename)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Settings.Default.TVTestPath;
            psi.Arguments = Settings.Default.TVTPlayBondriver + " " + Settings.Default.Com + @" """ + path + @"""";
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
