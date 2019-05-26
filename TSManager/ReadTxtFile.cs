using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace TSManager
{
    class ReadTxtFile
    {
        string flag = @"\[新\]|\[終\]|\[再\]|\[字\]|\[デ\]|\[解\]|\[無\]|\[二\]|\[S\]|\[SS\]|\[初\]|\[生\]|\[Ｎ\]|\[映\]|\[多\]|\[双\]";
        string subtitleflag = @"「.+」|【.+】";
        string tvIndex = @"[#＃♯](?<index>[0-9０-９]{1,3})|[第](?<index>[0-9０-９]{1,3})[話回]";

        public ReadTxtFile(string filepath)
        {
            if (!Properties.Settings.Default.Mode)
            {
                try
                {
                    GetFromTxt(filepath);
                }
                catch
                {
                    GetFromEit(filepath);
                    Util.WriteLog("EDCBの録画結果から番組情報取得できなかったためEitから取得します。", filepath);
                }
            }
            else
            {
                try
                {
                    GetFromEit(filepath);
                }
                catch
                {
                    GetFromTxt(filepath);
                    Util.WriteLog("Eitから番組情報取得できなかったためEDCBの録画結果から取得します。", filepath);
                }
            }
        }

        private void GetFromEit(string filepath)
        {
            using (var process = new Process())
            {
                try
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = Util.file,
                        Arguments = $"\"{filepath.Replace(".program.txt", "")}\" -cbidtpg -l 10",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                    };
                    process.Start();
                    var infos = process.StandardOutput.ReadToEnd().Split(',');
                    Company = infos[0].Normalize(NormalizationForm.FormKC);
                    Title = infos[1];
                    Subtitle = infos[2];
                    var day = infos[3];
                    var starttime = infos[4];
                    var genres = infos[6].Split('　');
                    Length = TimeSpan.Parse(infos[5]);
                    Starttime = DateTime.Parse(day + " " + starttime);
                    Endtime = Starttime.Add(Length);
                    Series = Regex.Replace(Title, flag + "|" + subtitleflag + "|" + tvIndex, "");
                    if (Series.Split('　').Length > 1)
                    {
                        Series = Series.Split('　')[0];
                    }
                    var matche = Regex.Matches(Title, subtitleflag);
                    foreach (Match ma in matche)
                    {
                        SeriesInfo.Add(ma.Value);
                    }
                    foreach (var genre in genres)
                    {
                        GenreIndex.Add(GenreIndexRet(genre));
                        Genre.Add(genre.Split(' ')[0]);
                    }
                    if (Regex.IsMatch(Title, tvIndex))
                    {
                        var tvidex_mat = Regex.Match(Title, tvIndex);
                        Epinum = int.Parse(tvidex_mat.Groups["index"].Value.Normalize(NormalizationForm.FormKC));
                    }
                    else
                    {
                        Epinum = 0;
                    }
                }
                catch
                {
                    throw new AggregateException();
                }
            }
        }

        private void GetFromTxt(string filepath)
        {
            var file = File.ReadAllLines(filepath);
            var time = file[0];
            Company = file[1].Normalize(NormalizationForm.FormKC);
            Title = file[2];

            var timesprit = time.Split(' ');
            var day = Regex.Replace(timesprit[0], @"\([月|火|水|木|金|土|日]\)", "");
            var time_sprit = timesprit[1].Split('～');

            Starttime = DateTime.Parse(day + " " + time_sprit[0]);
            Endtime = DateTime.Parse(day + " " + time_sprit[1]);

            if (Endtime.CompareTo(Starttime) == -1)
            {
                Endtime = Endtime.AddDays(1);
            }

            Length = Endtime - Starttime;

            Series = Regex.Replace(Title, flag + "|" + subtitleflag + "|" + tvIndex, "");
            if (Series.Split('　').Length > 1)
            {
                Series = Series.Split('　')[0];
            }
            var matche = Regex.Matches(Title, subtitleflag);
            foreach (Match ma in matche)
            {
                SeriesInfo.Add(ma.Value);
            }
            var i = 4;
            while (file[i] != "")
            {
                Subtitle += "\n" + file[i];
                i++;
            }
            var genreIndexCount = Array.IndexOf(file, "ジャンル : ") + 1;
            while (file[genreIndexCount] != "")
            {
                GenreIndex.Add(GenreIndexRet(file[genreIndexCount]));
                Genre.Add(file[genreIndexCount].Split(' ')[0]);
                genreIndexCount++;
            }
            if (Regex.IsMatch(Title, tvIndex))
            {
                var tvidex_mat = Regex.Match(Title, tvIndex);
                Epinum = int.Parse(tvidex_mat.Groups["index"].Value.Normalize(NormalizationForm.FormKC));
            }
            else
            {
                Epinum = 0;
            }
        }

        private int GetInt(string txt)
        {
            return int.Parse(txt);
        }

        private int GenreIndexRet(string r)
        {
            r = r.Split(' ')[0];
            return Array.IndexOf(Util.genres, r);
        }

        public string Company { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public List<string> Genre { get; set; } = new List<string>();
        public List<int> GenreIndex { get; set; } = new List<int>();
        public string Series { get; set; }
        public List<string> SeriesInfo { get; set; } = new List<string>();
        public int Epinum { get; set; }
        public DateTime Starttime { get; set; }
        public DateTime Endtime { get; set; }
        public TimeSpan Length { get; set; }
    }
}
