namespace MaliyetApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var backButton = this.FindByName<Button>("BackButton");
            if (backButton != null)
            {
                backButton.Style = (Style)Application.Current.Resources["BackButtonStyle"];
            }
        }

        private void Shell_Navigated(object sender, ShellNavigatedEventArgs e)
        {

        }

        private void Shell_Navigating(object sender, ShellNavigatingEventArgs e)
        {

        }
    }
}
