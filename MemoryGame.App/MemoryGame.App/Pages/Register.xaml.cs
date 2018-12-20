using MemoryGame.App.Classes;
using MemoryGame.App.Helper;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace MemoryGame.App.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Register : ContentPage
	{
		public Register ()
		{
			InitializeComponent ();
		}

        enum EntryOption
        {
            Register = 0,
            Returning = 1,
            Cancel = 2
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasBackButton(this, false);

            if (!string.IsNullOrEmpty(Settings.PlayerFirstName))
                App._navPage.PushAsync(App._homePage);
        }

        async Task CheckExistingProfileAndSave(string email)
        {
                try
                {
                    PlayerData player = await PlayerManager.CheckExistingPlayer(email);
                    if (string.IsNullOrEmpty(player.FirstName) && string.IsNullOrEmpty(player.LastName))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Email does not exist.", "OK");
                    }
                    else
                    {
                        Settings.PlayerFirstName = player.FirstName.Trim();
                        Settings.PlayerLastName = player.LastName.Trim();
                        Settings.PlayerEmail = email.Trim();
                        Settings.TopScore = player.Best;
                        Settings.DateAchieved = player.DateAchieved;

                    await App._navPage.PushAsync(App._homePage);
                }
                }
                catch
                {
                    await App.Current.MainPage.DisplayAlert("Oops", "An error occured while connecting to the server. Please check your connection.", "OK");
                }
        }

        async Task Save()
        {
            Settings.PlayerFirstName = entryFirstName.Text.Trim();
            Settings.PlayerLastName = entryLastName.Text.Trim();
            Settings.PlayerEmail = entryEmail.Text.ToLower().Trim();

            await App._navPage.PushAsync(App._homePage);
        }

        void ToggleEntryView(EntryOption option)
        {
            switch (option)
            {
                case EntryOption.Register:
                    {
                        lblWelcome.IsVisible = false;
                        layoutChoose.IsVisible = false;
                        layoutLogin.IsVisible = false;
                        layoutRegister.IsVisible = true;
                        break;
                    }
                case EntryOption.Returning:
                    {
                        lblWelcome.IsVisible = false;
                        layoutChoose.IsVisible = false;
                        layoutRegister.IsVisible = false;
                        layoutLogin.IsVisible = true;
                        break;
                    }
                case EntryOption.Cancel:
                    {
                        lblWelcome.IsVisible = true;
                        layoutChoose.IsVisible = true;
                        layoutRegister.IsVisible = false;
                        layoutLogin.IsVisible = false;
                        break;
                    }
            }
        }

        void OnbtnNewClicked(object sender, EventArgs args)
        {
            ToggleEntryView(EntryOption.Register);
        }

        void OnbtnReturnClicked(object sender, EventArgs args)
        {
            ToggleEntryView(EntryOption.Returning);
        }

        void OnbtnCancelLoginClicked(object sender, EventArgs args)
        {
            ToggleEntryView(EntryOption.Cancel);
        }

        void OnbtnCancelRegisterClicked(object sender, EventArgs args)
        {
            ToggleEntryView(EntryOption.Cancel);
        }

        async void OnbtnRegisterClicked(object sender, EventArgs args)
        {
            btnRegister.IsEnabled = false;

            if (string.IsNullOrEmpty(entryFirstName.Text) 
                || string.IsNullOrEmpty(entryLastName.Text) 
                || string.IsNullOrEmpty(entryEmail.Text))
                await App.Current.MainPage.DisplayAlert("Error", "Please supply the required fields.", "Got it");
            else
                await Save();

            btnRegister.IsEnabled = true;
        }

        async void OnbtnLoginClicked(object sender, EventArgs args)
        {


            if (string.IsNullOrEmpty(entryExistingEmail.Text.ToLower().Trim()))
                await App.Current.MainPage.DisplayAlert("Error", "Please supply your email.", "Got it");
            else
            {
                if (Utils.IsConnectedToInternet())
                {
                    btnLogin.IsEnabled = false;
                    await CheckExistingProfileAndSave(entryExistingEmail.Text.ToLower().Trim());
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No internet connection.", "OK");
                }
            }

            btnLogin.IsEnabled = true;
        }

    }
}



