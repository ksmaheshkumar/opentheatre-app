﻿using Utilities;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using regexFileName;
using DatabaseFilesAPI;

namespace OpenTheatre
{

    public partial class ctrlMoviesPoster : UserControl
    {
        public ctrlMoviesPoster()
        {
            InitializeComponent();
        }

        public string infoGenres;
        public string infoSynopsis;
        public string infoRuntime;
        public string infoRated;
        public string infoDirector;
        public string infoCast;
        public string infoImdbId;
        public string infoImdbRating;
        public string infoImagePoster;
        public string infoImageFanart;
        public string infoTrailer;

        public string[] infoMovieLinks;

        private void ctrlPosterTitle_Load(object sender, EventArgs e)
        {
            BackColor = Color.Transparent;
        }

        private void InfoPoster_ClickButtonArea(object Sender, MouseEventArgs e)
        {
            try
            {
                frmOpenTheatre.form.tabBlank.Controls.Clear();

                ctrlDetails MovieDetails = new ctrlDetails();

                MovieDetails.infoTitle.Text = infoTitle.Text;
                MovieDetails.infoYear.Text = infoYear.Text;
                MovieDetails.infoGenre.Text = infoGenres;
                MovieDetails.infoSynopsis.Text = infoSynopsis;
                MovieDetails.infoRuntime.Text = infoRuntime;
                MovieDetails.infoRated.Text = infoRated;
                MovieDetails.infoDirector.Text = infoDirector;
                MovieDetails.infoCast.Text = infoCast;
                MovieDetails.infoRatingIMDb.Text = infoImdbRating;
                MovieDetails.infoImdbId = infoImdbId;

                try
                {
                    MovieDetails.imgPoster.Image = UtilityTools.ChangeOpacity(frmOpenTheatre.LoadPicture(infoImagePoster), 1);
                    MovieDetails.BackgroundImage = UtilityTools.ChangeOpacity(frmOpenTheatre.LoadPicture(infoImageFanart), 0.2F);
                }
                catch { }

                if (infoImagePoster == "") { MovieDetails.imgPoster.Image = UtilityTools.ChangeOpacity(Properties.Resources.poster_default, 1); }
                if (infoImageFanart == "") { MovieDetails.BackgroundImage = UtilityTools.ChangeOpacity(Properties.Resources.background_original, 0.5F); }

                foreach (string movieLink in infoMovieLinks)
                {
                    ctrlStreamInfo ctrlInfo = new ctrlStreamInfo();
                    ctrlInfo.infoFileURL = new Uri(movieLink).AbsoluteUri;
                    ctrlInfo.infoFileHost.Text = new Uri(movieLink).Host.Replace("www.", "");
                    ctrlInfo.infoFileName.Text = Path.GetFileNameWithoutExtension(new Uri(movieLink).LocalPath);
                    MovieDetails.panelStreams.Controls.Add(ctrlInfo);
                }

                MovieDetails.Dock = DockStyle.Fill;
                frmOpenTheatre.form.tabBlank.Controls.Clear();
                frmOpenTheatre.form.tabBlank.Controls.Add(MovieDetails);
                frmOpenTheatre.form.tab.SelectedTab = frmOpenTheatre.form.tabBlank;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void InfoPoster_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                if (!(infoPoster2.Image == null))
                {
                    infoPoster2.Image.Dispose();
                }

                infoPoster2.Image = new Bitmap(infoPoster.BackgroundImage);
                infoPoster.BorderShow = true;
                infoPoster.BackgroundImage.Dispose();
                infoPoster.BackgroundImage = UtilityTools.ChangeOpacity(infoPoster2.Image, 0.4F);
                Update();
            }
            catch
            {
                infoPoster.BackgroundImage = UtilityTools.ChangeOpacity(Properties.Resources.poster_default, 0.4F); 
            }
        }

        private void InfoPoster_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                infoPoster.BackgroundImage.Dispose();
                infoPoster.BorderShow = false;
                infoPoster.BackgroundImage = UtilityTools.ChangeOpacity(infoPoster2.Image, 1);
            }
            catch
            {
                infoPoster.BackgroundImage = UtilityTools.ChangeOpacity(Properties.Resources.poster_default, 1);
            }
        }

    }
}