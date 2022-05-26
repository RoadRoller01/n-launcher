using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace nigga_launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string gamesPath;
        private GitHubClient client;

        public async void CheckGames()
        {
            IReadOnlyList<Repository> reps = await client.Repository.GetAllForUser("RoadRoller01");
            

            for (int i = 0;i < reps.Count; i++)
            {
                try
                {
                    Repository current = reps[i];
                    if (reps[i].Topics[0] == "game")
                    {
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
            gamesPath = Directory.GetCurrentDirectory() + "/games";

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
            
            var releases = await client.Repository.Release.GetAll("RoadRoller01", "amogus");
            var latestAsset = releases[0].Assets[0];
            

            // Download with WebClient
            using var webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");

            var latestAssetUri = new Uri(latestAsset.Url);
            // Download the file
            webClient.DownloadFileAsync(latestAssetUri, gamesPath + "/build.zip");

           // MessageBox.Show("nia");

        }
        

    }
}

















































































































// amogusballs123hentai is the best
// ofc