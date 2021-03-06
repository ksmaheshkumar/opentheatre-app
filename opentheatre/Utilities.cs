﻿using DatabaseFilesAPI;
using OMDbAPI;
using OpenTheatre;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utilities
{
    public class UtilityTools
    {
        // open new broken source issue template
        public static void openBrokenFileIssue(string webFile)
        {
            Process.Start("https://github.com/invu/OpenTheatre-app/issues/new?title=" + "Found Broken File" +
                "&body=" +
                "Host: " + new Uri(webFile).Host.Replace("www.", "") + "%0A" +
                "File Name: " + new Uri(webFile).LocalPath);
        }

        // return list that contains file
        public static string getContainingListOfURL(string fileUrl)
        {
            foreach (string file in frmOpenTheatre.dataFilesMovies)
            {
                var data = DatabaseFilesEntity.FromJson(file);
                if (data.URL == fileUrl) { return "Movies"; }
            }
            foreach (string file in frmOpenTheatre.dataFilesSeries)
            {
                var data = DatabaseFilesEntity.FromJson(file);
                if (data.URL == fileUrl) { return "Series"; }
            }
            foreach (string file in frmOpenTheatre.dataFilesAnime)
            {
                var data = DatabaseFilesEntity.FromJson(file);
                if (data.URL == fileUrl) { return "Anime"; }
            }
            foreach (string file in frmOpenTheatre.dataFilesSubtitles)
            {
                var data = DatabaseFilesEntity.FromJson(file);
                if (data.URL == fileUrl) { return "Subtitles"; }
            }
            foreach (string file in frmOpenTheatre.dataFilesTorrents)
            {
                var data = DatabaseFilesEntity.FromJson(file);
                if (data.URL == fileUrl) { return "Torrents"; }
            }
            foreach (string file in frmOpenTheatre.dataFilesArchives)
            {
                var data = DatabaseFilesEntity.FromJson(file);
                if (data.URL == fileUrl) { return "Archives"; }
            }
            foreach (string file in frmOpenTheatre.dataMovies)
            {
                var data = OMDbEntity.FromJson(file);
                if (data.Sources.Contains(fileUrl)) { return "Movies"; }
            }

            return "null";
        }

        // return file size with suffix
        public static string ToFileSize(double value)
        {
            string[] suffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            for (int i = 0; i < suffixes.Length; i++)
            {
                if (value <= (Math.Pow(1024, i + 1)))
                {
                    return ThreeNonZeroDigits(value / Math.Pow(1024, i)) + " " + suffixes[i];
                }
            }

            return ThreeNonZeroDigits(value / Math.Pow(1024, suffixes.Length - 1)) +
                " " + suffixes[suffixes.Length - 1];
        }

        // Return the value formatted to include at most three
        // non-zero digits and at most two digits after the
        // decimal point. Examples:
        //         1
        //       123
        //        12.3
        //         1.23
        //         0.12
        public static string ThreeNonZeroDigits(double value)
        {
            if (value >= 100)
            {
                // No digits after the decimal.
                return value.ToString("0,0");
            }
            else if (value >= 10)
            {
                // One digit after the decimal.
                return value.ToString("0.0");
            }
            else
            {
                // Two digits after the decimal.
                return value.ToString("0.00");
            }
        }

        // open file
        public static void openFile(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            // combine the arguments together
            // it doesn't matter if there is a space after ','
            string argument = "/select, \"" + path + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        // if text contains words = true
        public static bool ContainsAll(string source, params string[] values)
        {
            return values.All(x => source.ToLower().Contains(x));
        }

        // if file above hours
        public static bool IsAboveThreshold(string filename, int hours)
        {
            return new FileInfo(filename).LastAccessTime < DateTime.Now.AddHours(-hours);
        }

        public static string Random(ICollection<string> Items)
        {
            Random Rndm = new Random();
            List<string> StringList = new List<string>(Items);
            return StringList[Rndm.Next(0, Items.Count)];
        }

        // get words from text
        public static string[] GetWords(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"\b[\w']*\b");

            var words = from m in matches.Cast<Match>()
                        where !string.IsNullOrEmpty(m.Value)
                        select TrimSuffix(m.Value);

            return words.ToArray();
        }

        public static string TrimSuffix(string word)
        {
            int apostropheLocation = word.IndexOf('\'');
            if (apostropheLocation != -1)
            {
                word = word.Substring(0, apostropheLocation);
            }

            return word;
        }

        // change image opacity
        public static Bitmap ChangeOpacity(Image img, float opacityvalue)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = opacityvalue;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();
            return bmp;
        }

        // get time ago
        public static string getTimeAgo(DateTime date)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - date.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "1 second" : ts.Seconds + " seconds";

            if (delta < 2 * MINUTE)
                return "1 minute";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes";

            if (delta < 90 * MINUTE)
                return "1 hour";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours";

            if (delta < 48 * HOUR)
                return "1 day";

            if (delta < 30 * DAY)
                return ts.Days + " days";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "1 month" : months + " months";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "1 year" : years + " years";
            }
        }

        // check app for updates, installs installer if available
        public static void checkForUpdate()
        {
            Version newVersion = null;
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(frmOpenTheatre.linkLatestVersion);
            StreamReader reader = new StreamReader(stream);
            newVersion = new Version(reader.ReadToEnd());
            Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            if (curVersion.CompareTo(newVersion) < 0)
            {
                MessageBox.Show("There is a new update available ready to be installed.", "OpenTheatre - Update Available");

                try
                {
                    client.DownloadFile(frmOpenTheatre.getLatestInstaller(newVersion), frmOpenTheatre.pathDownloadInstaller);
                    Process.Start(frmOpenTheatre.pathDownloadInstaller);
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to run installer." + Environment.NewLine + Environment.NewLine + ex.Message, "OpenTheatre - Update Error");
                }
            }
        }
    }
}
