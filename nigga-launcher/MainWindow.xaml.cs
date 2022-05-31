using Octokit;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace nigga_launcher
{
    enum LauncherStatus
    {
        loading,
        play,
        downloadingGame,
        downloadingUpdate,
        downloading,
        failed
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string gamesPath;
        private GitHubClient client;
        private List<Repository> gamesRep;


        private LauncherStatus _status;
        internal LauncherStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                switch (_status)
                {
                    case LauncherStatus.loading:
                        PlayButton.Content = "Loading...";
                        // --------------------------
                        // add loading dots animation
                        // --------------------------
                        break;
                    case LauncherStatus.play:
                        PlayButton.Content = "Play";
                        break;
                    case LauncherStatus.downloadingGame:
                        PlayButton.Content = "Download";
                        break;
                    case LauncherStatus.downloadingUpdate:
                        PlayButton.Content = "Download Update";
                        break;
                    case LauncherStatus.downloading:
                        PlayButton.Content = "Downloading";
                        // add dots animation? maybe
                        break;
                    case LauncherStatus.failed:
                        PlayButton.Content = "Download Failed - Retry";
                        break;
                    default:
                        break;
                }
            }
        }

        public async void CheckGames()
        {
            IReadOnlyList<Repository> reps = await client.Repository.GetAllForUser("RoadRoller01");
            gamesRep = new List<Repository>();

            for (int i = 0;i < reps.Count; i++)
            {
                try
                {
                    Repository current = reps[i];
                    if (reps[i].Topics[0] == "game")
                    {
                        gamesRep.Add(current);
                        GamesList.Items.Add(current.Name);
                    }

                }catch(ArgumentOutOfRangeException)
                {
                    continue;
                }
            }

        }
        public MainWindow()
        {
            InitializeComponent();

            /* --- check games dir exists if it didn't, create dir --- */
            gamesPath = AppDomain.CurrentDomain.BaseDirectory + "/games/";

            if (!Directory.Exists(gamesPath))
            {
                Directory.CreateDirectory(gamesPath);
            }
            /* --- end --- */

            client = new GitHubClient(new ProductHeaderValue("amogusBalls123hentai"));
            CheckGames();

        }

        

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (GamesList.SelectedIndex != -1)
            {
                // get selected game url
                Repository gameRep = gamesRep[GamesList.SelectedIndex];
                Release latestRelease = await client.Repository.Release.GetLatest(gameRep.Id);
                Uri gameUri = new Uri(latestRelease.Assets[0].Url);

                // Download game file
                WebClient webClient = new WebClient();

                webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
                webClient.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");
                
                webClient.DownloadFileAsync(gameUri, gamesPath + "game.zip");

                // debug stuff
                //Debug.WriteLine(latestRelease.Assets[0].Name);
                //Debug.WriteLine(latestRelease.Assets[0].BrowserDownloadUrl);
                //Debug.WriteLine(latestRelease.Assets[0].Url);
            }
            else
            {
                MessageBox.Show("are you nigga select game");
            }

            // Download with WebClient
            

           // MessageBox.Show("nia");

        }
        

    }
}

















































































































// amogusballs123hentai is the best
// ofc