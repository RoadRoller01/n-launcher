using Octokit;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Text.Json;

namespace n_launcher
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
        private string SaveFile = "config.json";
        private string username = "RoadRoller01";
        private string token = "";
        private MyData data;
        private string gamesPath;
        private GitHubClient client;
        private List<Repository> gamesRep;
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };



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
            IReadOnlyList<Repository> reps = await client.Repository.GetAllForUser(username);
            

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

            ReadConfigFile();
            client = new GitHubClient(new ProductHeaderValue("amogusBalls123hentai"));
            if (token.Length != 0)
            {
                var tokenAuth = new Credentials(token);
                client.Credentials = tokenAuth;
            }
            
            CheckGames();
        }

        void ReadConfigFile()
        {



            string jsonString = File.ReadAllText(SaveFile);
            data = JsonSerializer.Deserialize<MyData>(jsonString)!;


            username = data.username;
            token = data.token;

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

        }
        public async void  DownloadGame() {

            if (GamesList.SelectedIndex != -1)
            {
                // get selected game url
                Repository gameRep = gamesRep[GamesList.SelectedIndex];
                Release latestRelease = await client.Repository.Release.GetLatest(gameRep.Id);
                Uri gameUri = new Uri(latestRelease.Assets[0].BrowserDownloadUrl);


                // Download game file
                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
                webClient.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");


                webClient.DownloadProgressChanged += DownloadProgress;
                webClient.DownloadFileCompleted += DownloadProgressCompleted;

                MyDownloadProgressBarPanel.Visibility = Visibility.Visible;

                webClient.DownloadFileAsync(gameUri, gamesPath + "game.zip");



            }
            else
            {
                MessageBox.Show("are you nigga select game");
            }
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            
            
            CheckGameVer();

        }


        private async void CheckGameVer()
        {
            Repository SelectedgameRep = gamesRep[GamesList.SelectedIndex];
            IReadOnlyList<RepositoryTag> repsTags = await client.Repository.GetAllTags(SelectedgameRep.Id);



            if (data.Games.ContainsValue(repsTags[0].Name) && data.Games.ContainsKey(SelectedgameRep.Name))
            {

                MessageBox.Show("game up to date");

                return;
            }
            else if (!data.Games.ContainsValue(repsTags[0].Name) && !data.Games.ContainsKey(SelectedgameRep.Name))
            {

                data.Games.Add(SelectedgameRep.Name, repsTags[0].Name);

            }
            else
            {

                data.Games.TryAdd(default,repsTags[0].Name);
                DownloadGame();

            }




            string jsonString = JsonSerializer.Serialize(data);
            File.WriteAllText(SaveFile, jsonString);

           
        }



        // ---------------- Download Progress stuff ---------------- //
        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {

            MyDownloadProgressBar.Value = e.ProgressPercentage;

            // MessageBox.Show("nia");
            string received = string.Format("{0,0}", e.BytesReceived);
            string total = string.Format("{0,0}", e.TotalBytesToReceive);

            MyDownloadProgressBarText.Text = $"{received} / {total}";
        }
        
        private void DownloadProgressCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MyDownloadProgressBarPanel.Visibility = Visibility.Hidden;
            MyDownloadProgressBar.Value = 0;
            MyDownloadProgressBarText.Text = "";
        }



    }
}

















































































































// amogusballs123hentai is the best
// ofc
// i love amogus