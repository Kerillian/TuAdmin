using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TuAdmin
{
	public partial class MainWindow : Window
	{
		private readonly Regex xmlAvatarPattern = new Regex(@"<avatarMedium><\!\[CDATA\[(.+)\]\]><\/avatarMedium>", RegexOptions.Compiled);
		private readonly Regex xmlSteamId64Pattern = new Regex(@"<steamID64>([0-9]+)<\/steamID64>", RegexOptions.Compiled);
		private readonly Regex xmlUsernamePattern = new Regex(@"<steamID><\!\[CDATA\[(.+)\]\]><\/steamID>", RegexOptions.Compiled);

		public MainWindow()
		{
			InitializeComponent();
		}

		private string TowerConfigDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tower\\Saved\\Config\\WindowsNoEditor");
		private string TowerServerIni => Path.Combine(TowerConfigDirectory, "TowerServer.ini");

		private async Task<ProfilePanel?> GetPanel(string id)
		{
			string link = id.StartsWith("http") ? $"{id}?xml=1" : $"https://steamcommunity.com/profiles/{id}/?xml=1";

			try
			{
				using HttpClient client = new HttpClient();
				string data = await client.GetStringAsync(link);
				string id64 = xmlSteamId64Pattern.Match(data).Groups[1].Value;
				string username = xmlUsernamePattern.Match(data).Groups[1].Value;
				string avatar = xmlAvatarPattern.Match(data).Groups[1].Value;

				if (Profiles.Children.OfType<ProfilePanel>().Any(x => x.SteamId == id64))
				{
					return null;
				}

				return new ProfilePanel()
				{
					SteamId = id64,
					Username = username,
					Avatar = avatar
				};
			}
			catch
			{
				return null;
			}
		}

		private async void WindowLoaded(object sender, RoutedEventArgs e)
		{
			if (File.Exists(TowerServerIni))
			{
				string[] lines = await File.ReadAllLinesAsync(TowerServerIni);

				foreach (string line in lines)
				{
					if (line.StartsWith("AdminSteamID"))
					{
						if (await GetPanel(line[13..]) is { } panel)
						{
							Profiles.Children.Add(panel);
						}
					}
				}
			}
		}

		private async void SaveButtonClicked(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(TowerConfigDirectory))
			{
				Directory.CreateDirectory(TowerConfigDirectory);
			}

			string[] ids = Profiles.Children.OfType<ProfilePanel>().Select(p => "AdminSteamID=" + p.SteamId).ToArray();
			SaveButton.IsEnabled = false;

			await File.WriteAllTextAsync(TowerServerIni, "[Administration]\n" + string.Join('\n', ids));

			SaveButton.Content = "Admins saved to config!";

			await Task.Delay(2000);

			SaveButton.Content = "Save";
			SaveButton.IsEnabled = true;
		}

		private async void SteamIdInputBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				if (await GetPanel(SteamIdInputBox.Text) is { } panel)
				{
					Profiles.Children.Add(panel);
				}
			}
		}

		private void SteamIdInputBoxGotFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (SteamIdInputBox.Text == "SteamId or link...")
			{
				SteamIdInputBox.Text = "";
			}
		}

		private void SteamIdInputBoxLostFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (SteamIdInputBox.Text == string.Empty)
			{
				SteamIdInputBox.Text = "SteamId or link...";
			}
		}
	}
}