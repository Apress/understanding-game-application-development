using Xamarin.Forms;
using MemoryGame.App.iOS.Services;
using System.IO;
using Foundation;
using AVFoundation;
using MemoryGame.App.Services;

[assembly: Dependency(typeof(SoundService))]

namespace MemoryGame.App.iOS.Services
{
    public class SoundService : NSObject, ISound, IAVAudioPlayerDelegate
    {
        //#region IDisposable implementation

        //public void Dispose()
        //{

        //}

        //#endregion

        public SoundService()
        {
        }

        public bool PlayWavFile(string fileName)
        {
            return true;
        }

        public bool PlayMp3File(string fileName)
        {

            var played = false;

            NSError error = null;
            AVAudioSession.SharedInstance().SetCategory(AVAudioSession.CategoryPlayback, out error);

            string sFilePath = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(fileName), "mp3");
            var url = NSUrl.FromString(sFilePath);
            var _player = AVAudioPlayer.FromUrl(url);
            _player.Delegate = this;
            _player.Volume = 100f;
            played = _player.PrepareToPlay();
            _player.FinishedPlaying += (object sender, AVStatusEventArgs e) => {
                _player = null;
            };
            played = _player.Play();

            return played;
        }
    }

}