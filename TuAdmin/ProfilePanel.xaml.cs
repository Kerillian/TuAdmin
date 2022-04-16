using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TuAdmin
{
	public partial class ProfilePanel : UserControl
	{
		public static readonly DependencyProperty AvatarProperty = DependencyProperty.Register("Avatar", typeof(string), typeof(ProfilePanel), new PropertyMetadata(null));
		public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(ProfilePanel), new PropertyMetadata(null));
		public static readonly DependencyProperty SteamIdProperty = DependencyProperty.Register("SteamId", typeof(string), typeof(ProfilePanel), new PropertyMetadata(null));

		public ProfilePanel()
		{
			InitializeComponent();
		}

		public string Avatar
		{
			get => (string)GetValue(AvatarProperty);
			set => SetValue(AvatarProperty, value);
		}

		public string Username
		{
			get => (string)GetValue(UsernameProperty);
			set => SetValue(UsernameProperty, value);
		}

		public string SteamId
		{
			get => (string)GetValue(SteamIdProperty);
			set => SetValue(SteamIdProperty, value);
		}

		private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (Parent is StackPanel panel)
			{
				panel.Children.Remove(this);
			}
		}
	}
}