using Xamarin.Forms;
using Android.Media;
using MemoryGame.App.Droid.Services;
using MemoryGame.App.Services;

[assembly: Dependency(typeof(SoundService))]

namespace MemoryGame.App.Droid.Services
{
    public class SoundService : ISound
    {
        public SoundService() { }

        private MediaPlayer _mediaPlayer;

        public bool PlayMp3File(string fileName)
        {
            _mediaPlayer = MediaPlayer.Create(Android.App.Application.Context, Resource.Raw.beep);
            _mediaPlayer.Start();

            return true;
        }

        public bool PlayWavFile(string fileName)
        {
            //TO DO: Own implementation here
            return true;
        }
    }
}
