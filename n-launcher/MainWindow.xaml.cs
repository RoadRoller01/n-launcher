namespace n_launcher;

using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;


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

        for (int i = 0; i < reps.Count; i++)
        {
            try
            {
                Repository current = reps[i];
                if (reps[i].Topics[0] == "game")
                {
                    gamesRep.Add(current);
                    GamesList.Items.Add(current.Name.Replace("-"," ").Replace("_"," "));
                }

            }
            catch (ArgumentOutOfRangeException)
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

        ReadSaveFile();
        client = new GitHubClient(new ProductHeaderValue("amogusBalls123hentai"));
        if (token.Length != 0)
        {
            var tokenAuth = new Credentials(token);
            client.Credentials = tokenAuth;
        }

        CheckGames();
    }

    void ReadSaveFile()
    {



        string jsonString = File.ReadAllText(SaveFile);
        data = JsonSerializer.Deserialize<MyData>(jsonString)!;


        username = data.username;
        token = data.token;

    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {

    }


    /// <summary>
    /// Download and extract given uri zip file
    /// </summary>
    /// <param name="gameUri"></param>
    /// <returns></returns>
    private async Task DownloadGame(Uri gameUri)
    {
        String gameFile = gamesPath + "game.zip";
        WebClient webClient = new WebClient();
    try { Directory.Delete(gamesPath + GamesList.SelectedValue , true); } catch { }


        webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
        webClient.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");

        webClient.DownloadProgressChanged += DownloadProgress;
        webClient.DownloadFileCompleted += DownloadProgressCompleted;


        MyDownloadProgressBarPanel.Visibility = Visibility.Visible;
        await webClient.DownloadFileTaskAsync(gameUri, gameFile);

        System.IO.Compression.ZipFile.ExtractToDirectory(gameFile, gamesPath);
        File.Delete(gameFile);
    }


    private async void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        PlayButton.IsEnabled = false;
        if (GamesList.SelectedIndex != -1)
        {
            Repository selectedGameRep = gamesRep[GamesList.SelectedIndex];
            Release latestRelease = await client.Repository.Release.GetLatest(selectedGameRep.Id);
            String savedGameVer = data.Games.GetValueOrDefault(selectedGameRep.Name);

            if (savedGameVer == latestRelease.TagName)
            {

                MessageBox.Show("game up to date");
            }
            else
            {
                
                await DownloadGame(new Uri(latestRelease.Assets[0].BrowserDownloadUrl));
                if (savedGameVer == default)
                {
                    data.Games.Add(selectedGameRep.Name, latestRelease.TagName);
                }
                else
                {
                    data.Games.Remove(selectedGameRep.Name);
                    data.Games.Add(selectedGameRep.Name, latestRelease.TagName);
                }

                string jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(SaveFile, jsonString);
            }
        }
        else
        {
            MessageBox.Show("are you nigga select game");
        }



        PlayButton.IsEnabled = true;
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


















































































































// amogusballs123hentai is the best
// ofc
// i love amogus