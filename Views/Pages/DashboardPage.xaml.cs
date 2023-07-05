using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Common.Interfaces;
using Newtonsoft.Json;
using System.Text;
using System.Drawing;
using System.Windows.Media;
using System.Security.Cryptography;
using System.Windows.Input;
using System.Windows.Forms;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Gameloop.Vdf.JsonConverter;
using Roundtable.Models;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Roundtable.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : INavigableView<ViewModels.DashboardViewModel>
    {
        private StringBuilder _console;
        private string _githubLatestUrl;

        private string _seamlessHash;

        private bool _seamlessCopied;
        private bool _roundtableDownloaded;

        public bool RoundTableDownloaded
        {
            get { return _roundtableDownloaded; }
            set
            {
                _roundtableDownloaded = value;
                if (value)
                {
                    DownloadButtonIcon.Glyph = '\uE74D';
                    DownloadHeaderIcon.Glyph = '\uE73E';
                    DownloadHeaderIcon.Foreground = new System.Windows.Media.SolidColorBrush(Colors.Green);
                    DownloadHeaderText.Text = "Roundtable Hold Arena already downloaded!";
                    DownloadText.Text = "Delete the already downloaded zip so you can redownload.";
                    DownloadButtonText.Text = "Delete";
                }
                else
                {
                    DownloadButtonIcon.Glyph = '\uE896';
                    DownloadHeaderIcon.Glyph = '\uE711';
                    DownloadHeaderIcon.Foreground = new System.Windows.Media.SolidColorBrush(Colors.Red);
                    DownloadHeaderText.Text = "Download Roundtable Hold Arena";
                    DownloadText.Text = "This will download the latest Roundtable Hold Arena version from GitHub.";
                    DownloadButtonText.Text = "Download";
                }

                checkAndUpdateIfReadyToSetup();
            }
        }
        public bool SeamlessCopied
        {
            get { return _seamlessCopied; }
            set
            {
                _seamlessCopied = value;
                if (value)
                {
                    SeamlessDropIcon.Glyph = '\uE8E1';
                    SeamlessCopiedIcon.Glyph = '\uE73E';
                    SeamlessCopiedIcon.Foreground = new System.Windows.Media.SolidColorBrush(Colors.Green);
                    SeamlessHeaderText.Text = "Seamless 1.5.1 found!";
                    DropMessage.Text = "Seamless found.";
                    BrowseButton.Visibility= Visibility.Hidden;
                    BrowseButton.Height = 0;
                    SeamlessPanel.IsEnabled = false;
                }
                else
                {
                    SeamlessDropIcon.Glyph = '\uE7B8';
                    SeamlessCopiedIcon.Glyph = '\uE711';
                    SeamlessCopiedIcon.Foreground = new System.Windows.Media.SolidColorBrush(Colors.Red);
                    SeamlessHeaderText.Text = "Seamless 1.5.1 not found!";
                    DropMessage.Text = "Drop Seamless 1.5.1 zip file here or";
                    BrowseButton.Visibility = Visibility.Visible;
                    BrowseButton.Height = double.NaN;
                    BrowseButton.IsEnabled = true;
                    SeamlessPanel.IsEnabled = true;
                }

                checkAndUpdateIfReadyToSetup();
            }
        }

        public bool ReadyToSetup
        {
            get { return _roundtableDownloaded && _seamlessCopied; }
        }

        public async Task<string> Get(string uri)
        {

            using var client = new HttpClient();

            client.BaseAddress = new Uri("https://api.github.com");
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
            client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var resp = await response.Content.ReadAsStringAsync();
            return resp;
        }

        public async Task<bool> DownoadZip(string url, string filename)
        {
            using (var httpClient = new HttpClient())
            {
                await httpClient.DownloadFile(url, filename);
                return true;
            }
        }

        public ViewModels.DashboardViewModel ViewModel
        {
            get;
        }

        public DashboardPage(ViewModels.DashboardViewModel viewModel)
        {
            _console = new StringBuilder();
            _seamlessHash = "C6576788918CB92592B7BCC12CD8D7F7";
            _githubLatestUrl = "https://api.github.com/repos/ClayAmore/Roundtable-Hold-Arena/releases/latest";

            ViewModel = viewModel;
            Loaded += DashBoardPage_Loaded;

            InitializeComponent();
        }

        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            var appRoot = System.AppDomain.CurrentDomain.BaseDirectory;

            if (RoundTableDownloaded)
            {
                var filePath = Path.Combine(appRoot, "downloads", "roundtable.zip");
                File.Delete(filePath);
                updateConsole("Mod package deleted!");
                RoundTableDownloaded = false;
            }
            else
            {
                updateConsole("getting release url...");
                var data = await Get(_githubLatestUrl);

                if (!string.IsNullOrEmpty(data))
                {
                    Response response = JsonConvert.DeserializeObject<Response>(data);
                    if (response.assets.Count > 0)
                    {
                        var downloadUrl = response.assets[0].browser_download_url;

                        if (!string.IsNullOrEmpty(downloadUrl))
                        {
                            var urlSplit = downloadUrl.Split("/");
                            
                            System.IO.Directory.CreateDirectory(Path.Combine(appRoot, "downloads"));
                            updateConsole("downloading mod package...");
                            var success = await DownoadZip(downloadUrl, Path.Combine(appRoot, "downloads", "roundtable.zip"));
                            if (success)
                            {
                                RoundTableDownloaded = true;
                                updateConsole("Mod package download complete!");
                            }
                        }
                    }
                }
            }
        }

        private async void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".zip"; // Default file extension
            dialog.Filter = "zip file (*.zip)|*.zip"; // Filter files by extension
            dialog.CheckFileExists = true;

            if (dialog.ShowDialog() == true)
            {
                bool valid = validateSeamlessFile(dialog.FileName);
                if (!valid) return;
                copySeamlessFile(dialog.FileName);
            }
        }

        private async void SetupBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog(this.GetIWin32Window());
            if(result == DialogResult.OK)
            {
                var appRoot = System.AppDomain.CurrentDomain.BaseDirectory;
                var roundtableZipPath = Path.Combine(appRoot, "downloads", "roundtable.zip");
                var seamlessZipPath = Path.Combine(appRoot, "downloads", "seamless.zip");
                var modInstallationPath = Path.Combine(dlg.SelectedPath, "Roundtable Hold Arena Mod");

                updateConsole("Creating directory...");
                System.IO.Directory.CreateDirectory(modInstallationPath);

                updateConsole("Exracting roundtable hold mod...");
                ZipFile.ExtractToDirectory(roundtableZipPath, modInstallationPath, true);
                updateConsole("Exracting seamless mod...");
                ZipFile.ExtractToDirectory(seamlessZipPath, Path.Combine(modInstallationPath, "_PLACE_SEAMLESS_COOP_151_HERE_"), true);
                copyToGameDirectoryIfNotInstalled();
                updateConsole("Done. Enjoy the mod!");
            }
        }

        private async void DashBoardPage_Loaded(object sender, RoutedEventArgs e)
        {
            var appRoot = System.AppDomain.CurrentDomain.BaseDirectory;
            RoundTableDownloaded = File.Exists(Path.Combine(appRoot, "downloads", "roundtable.zip"));

            var seamlessZipPath = Path.Combine(appRoot, "downloads", "seamless.zip");
            var seamlessFileExists = File.Exists(seamlessZipPath);
            if (!seamlessFileExists) {
                SeamlessCopied = false;
                return;
            }

            var seamlessHash = GetMD5HashFromFile(seamlessZipPath);
            SeamlessCopied = seamlessHash.Equals(_seamlessHash);
        }

        private void SeamlessPanel_Drop(object sender, System.Windows.DragEventArgs e)
        {

            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                bool valid = validateSeamlessFile(files[0]);

                if(!valid)
                {
                    DropMessage.Text = "Drop Seamless 1.5.1 zip file here or";
                    BrowseButton.Visibility = Visibility.Visible;
                    BrowseButton.IsEnabled = true;
                    return;
                }

                copySeamlessFile(files[0]);
            }
        }

        private void SeamlessPanel_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            DropMessage.Text = "Drop it here!";
            BrowseButton.Visibility = Visibility.Hidden;
            BrowseButton.IsEnabled = false;
        }

        private void SeamlessPanel_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            DropMessage.Text = "Drop Seamless 1.5.1 zip file here or";
            BrowseButton.Visibility = Visibility.Visible;
            BrowseButton.IsEnabled = true;
        }

        private bool validateSeamlessFile(string file)
        {
            updateConsole("Validating seamless files...");
            var droppedFileHash = GetMD5HashFromFile(file);
            bool valid = droppedFileHash.Equals(_seamlessHash);
            if(valid) updateConsole("Seamless files are valid!");
            else updateConsole("Seamless files are not valid! Check if you have the right version downloaded.");
            return valid;
        }

        private void copySeamlessFile(string file)
        {
            var appRoot = System.AppDomain.CurrentDomain.BaseDirectory;
            System.IO.Directory.CreateDirectory(Path.Combine(appRoot, "downloads"));
            var fileName = file.Split("\\").Last();
            var destination = Path.Combine(appRoot, "downloads", "seamless.zip");
            File.Copy(file, destination);
            SeamlessCopied = true;
        }

        private void copyToGameDirectoryIfNotInstalled()
        {
            updateConsole("Checking if you have seamless coop installed in game folder...");
            updateConsole($"Attempting to locate steam install directory..");
            string steamPath = Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Valve\\Steam", "SteamPath", "").ToString();
            if(!string.IsNullOrEmpty(steamPath))
            {
                updateConsole($"Located steam directory at: {steamPath}");
                updateConsole($"Attempting to locate appmanifest file...");
                var vdfPath = Path.Combine(steamPath, "SteamApps", "libraryfolders.vdf");
                VProperty applist = VdfConvert.Deserialize(File.ReadAllText(vdfPath));
                var json = applist.ToJson();
                JObject obj = JObject.Parse(json.Value.ToString());
                IList<string> paths = obj.Values().Where(v => v.Type == JTokenType.Object).Select(v => JObject.Parse(v.ToString())).Where(v => v.ContainsKey("path")).Select(v => v["path"].ToString()).ToList();

                var fileName = $"appmanifest_1245620.acf";
                foreach (var p in paths)
                {
                    var manifestPath = Path.Combine(p, "steamapps", fileName);
                    if (File.Exists(manifestPath))
                    {
                        updateConsole($"Located manifset file at: {manifestPath}");
                        updateConsole($"Attempting to locate Elden Ring directory...");
                        var manifestFileJson = VdfConvert.Deserialize(File.ReadAllText(manifestPath)).ToJson();
                        if(manifestFileJson != null)
                        {
                            var installDir = manifestFileJson.First()["installdir"].ToString();
                            var gamePath = Path.Combine(p, "steamapps", "common", installDir, "Game");
                            if(Directory.Exists(gamePath))
                            {
                                updateConsole($"Located Elden Ring directory at: {gamePath}");
                                updateConsole($"Attempting to locate SeamlessCoop directory...");
                                var seamlessDirectory = Path.Combine(gamePath, "SeamlessCoop");

                                if (Directory.Exists(seamlessDirectory))
                                {
                                    updateConsole("Found SeamlessCoop Folder in game directory.");
                                }
                                else
                                {
                                    updateConsole("Didn't find SeamlessCoop in game directory.");
                                    var appRoot = System.AppDomain.CurrentDomain.BaseDirectory;
                                    var seamlessZipPath = Path.Combine(appRoot, "downloads", "seamless.zip");
                                    updateConsole("Making a copy of Seamless files in game directory...");
                                    ZipFile.ExtractToDirectory(seamlessZipPath, gamePath);
                                }
                            }
                        }
                    }
                }
                updateConsole("Done copying Seamless!");
            }
        }

        private void updateConsole(string text)
        {
            _console.Append("> ").Append(text).Append("\n");
            Console.Text = _console.ToString();
        }

        private string GetMD5HashFromFile(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", string.Empty);
                }
            }
        }

        private void checkAndUpdateIfReadyToSetup()
        {
            if (ReadyToSetup)
            {
                SetupText.Text = "Pick where you want the mod unpacked and setup.";
                SetupBrowseButton.Visibility = Visibility.Visible;
                SetupBrowseButton.IsEnabled = true;
                SetupBrowseButton.Height = double.NaN;
            }
            else
            {
                SetupText.Text = "Missing either seamless or roundtable mod. Make sure to follow the steps above.";
                SetupBrowseButton.Visibility = Visibility.Hidden;
                SetupBrowseButton.IsEnabled = false;
                SetupBrowseButton.Height = 0;
            }
        }
    }
}