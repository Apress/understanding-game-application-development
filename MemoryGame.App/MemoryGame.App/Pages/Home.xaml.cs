using MemoryGame.App.Classes;
using MemoryGame.App.Helper;
using MemoryGame.App.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MemoryGame.App.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();
        }

        enum PlayType
        {
            Blink = 0,
            Sound = 1,
            Haptic = 2
        }

        private int _cycleStartInMS = 0;
        private int _cycleMaxInMS = 10000;
        private const int _cycleIntervalInMS = 2000;
        private const int _eventTypeCount = 3;

        public static int CurrentGameBlinkCount { get; private set; } = 0;
        public static int CurrentGameSoundCount { get; private set; } = 0;
        public static int CurrentGameHapticCount { get; private set; } = 0;
        public static int CurrentGameLevel { get; private set; } = 1;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasBackButton(this, false);

            PlayerManager.UpdateBest(CurrentGameLevel);

            if (Result._answered)
                LevelUp();
            else
                ResetLevel();

            lblBest.Text = $"Best: Level {PlayerManager.GetBestScore(CurrentGameLevel)}";
            lblLevel.Text = $"Level {CurrentGameLevel}";
        }

        static void IncrementPlayCount(PlayType play)
        {
            switch (play)
            {
                case PlayType.Blink:
                    {
                        CurrentGameBlinkCount++;
                        break;
                    }
                case PlayType.Sound:
                    {
                        CurrentGameSoundCount++;
                        break;
                    }
                case PlayType.Haptic:
                    {
                        CurrentGameHapticCount++;
                        break;
                    }
            }
        }

        public static void IncrementGameLevel()
        {
            CurrentGameLevel++;
        }

        void ResetLevel()
        {
            CurrentGameLevel = 1;
            _cycleStartInMS = _cycleIntervalInMS;
            lblTime.Text = string.Empty;
            btnStart.Text = "Start";
            btnStart.IsEnabled = true;
        }

        async void StartRandomPlay()
        {
            await Task.Run(() =>
            {

                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                int choice = rnd.Next(0, _eventTypeCount);

                switch (choice)
                {
                    case (int)PlayType.Blink:
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {

                                await imgLightOff.FadeTo(0, 200);
                                imgLightOff2.IsVisible = false;
                                imgLightOff.IsVisible = true;
                                imgLightOff.Source = ImageSource.FromFile("lighton.png");
                                await imgLightOff.FadeTo(1, 200);

                            });

                            IncrementPlayCount(PlayType.Blink);
                            break;
                        }
                    case (int)PlayType.Sound:
                        {
                            DependencyService.Get<ISound>().PlayMp3File("beep.mp3");
                            IncrementPlayCount(PlayType.Sound);
                            break;
                        }
                    case (int)PlayType.Haptic:
                        {
                            DependencyService.Get<IHaptic>().ActivateHaptic();
                            IncrementPlayCount(PlayType.Haptic);
                            break;
                        }
                }
            });
        }

        void ResetGameCount()
        {
            CurrentGameBlinkCount = 0;
            CurrentGameSoundCount = 0;
            CurrentGameHapticCount = 0;
        }

        void LevelUp()
        {
            _cycleStartInMS = _cycleStartInMS - 200; //minus 200 ms
        }

        void Play()
        {
            int timeLapsed = 0;
            int duration = 0;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                duration++;
                lblTime.Text = $"Timer: { TimeSpan.FromSeconds(duration).ToString("ss")}";

                if (duration < 10)
                    return true;
                else
                    return false;
            });

            Device.StartTimer(TimeSpan.FromMilliseconds(_cycleStartInMS), () => {
                timeLapsed = timeLapsed + _cycleStartInMS;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    imgLightOff2.IsVisible = true;
                    imgLightOff.IsVisible = false;
                    await Task.Delay(200);

                });

                if (timeLapsed <= _cycleMaxInMS)
                {
                    StartRandomPlay();
                    return true; //continue
                }



                App._navPage.PushAsync(App._resultPage);
                return false; //don't continue
            });
        }

        void OnButtonClicked(object sender, EventArgs args)
        {
            btnStart.Text = "Game Started...";
            btnStart.IsEnabled = false;

            ResetGameCount();

            Play();
        }

        async void OnbtnSyncClicked(object sender, EventArgs args)
        {

            if (Utils.IsConnectedToInternet())
            {
                btnSync.Text = "Syncing...";
                btnSync.IsEnabled = false;
                btnStart.IsEnabled = false;

                var response = await PlayerManager.Sync();
                if (!response)
                    await App.Current.MainPage
                        .DisplayAlert("Oops", 
                        "An error occured while connecting to the server. Please check your connection.", 
                        "OK");
                else
                    await App.Current.MainPage.DisplayAlert("Sync", "Data synced!", "OK");

                btnSync.Text = "Sync";
                btnSync.IsEnabled = true;
                btnStart.IsEnabled = true;
            }
            else
            {
                await App.Current.MainPage
                        .DisplayAlert("Error", 
                        "No internet connection.", 
                        "OK");
            }
        }

        async void OnbtnLogoutClicked(object sender, EventArgs args)
        {
            if (Utils.IsConnectedToInternet())
            {
                btnLogOut.IsEnabled = false;
                var response = await PlayerManager.Sync();
                if (response)
                {
                    Settings.ClearEverything();
                    await App._navPage.PopToRootAsync();
                }
                else
                    await App.Current.MainPage
                        .DisplayAlert("Oops", 
                        "An error occured while connecting to the server. Please check your connection.", 
                        "OK");
            }
            else
                await App.Current.MainPage
                    .DisplayAlert("Oops", 
                    "No internet connection. Please check your network.", 
                    "OK");

            btnLogOut.IsEnabled = true;

        }

    }
}


