using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TSManager
{
    class ReadTxtFile
    {
        private DateTime starttime;
        private DateTime endtime;
        private TimeSpan length;
        private string company;
        private string title;
        private string subtitle;
        private string series;
        private int epinum;
        private List<string> seriesInfo = new List<string>();
        private List<int> genreIndex = new List<int>();
        private List<string> genre = new List<string>();


        public ReadTxtFile(string filepath)
        {
            var flag = @"\[新\]|\[終\]|\[再\]|\[字\]|\[デ\]|\[解\]|\[無\]|\[二\]|\[S\]|\[SS\]|\[初\]|\[生\]|\[Ｎ\]|\[映\]|\[多\]|\[双\]";
            var subtitleflag = @"「.+」|【.+】";
            var tvIndex = @"[#＃♯](?<index>[0-9０-９]{1,3})|[第](?<index>[0-9０-９]{1,3})[話回]";
            var file = File.ReadAllLines(filepath);
            var time = file[0];
            Company = file[1];
            Title = file[2];

            var timesprit = time.Split(' ');
            var day = Regex.Replace(timesprit[0], @"\([月|火|水|木|金|土|日]\)", "");
            var time_sprit = timesprit[1].Split('～');
            
            

            Starttime = DateTime.Parse(day+" "+time_sprit[0]);
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
            var genreIndexCount = Array.IndexOf(file, "ジャンル : ");
            while (file[genreIndexCount] != "")
            {
                genreIndex.Add(GenreIndexRet(file[genreIndexCount]));
                genre.Add(file[genreIndexCount].Split(' ')[0]);
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

        public string Company { get => company; set => company = value; }
        public string Title { get => title; set => title = value; }
        public string Subtitle { get => subtitle; set => subtitle = value; }
        public List<string> Genre { get => genre; set => genre = value; }
        public List<int> GenreIndex { get => genreIndex; set => genreIndex = value; }
        public string Series { get => series; set => series = value; }
        public List<string> SeriesInfo { get => seriesInfo; set => seriesInfo = value; }
        public int Epinum { get => epinum; set => epinum = value; }
        public DateTime Starttime { get => starttime; set => starttime = value; }
        public DateTime Endtime { get => endtime; set => endtime = value; }
        public TimeSpan Length { get => length; set => length = value; }
    }
}
