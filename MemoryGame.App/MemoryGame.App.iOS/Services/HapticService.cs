using Xamarin.Forms;
using AudioToolbox;
using MemoryGame.App.iOS.Services;
using MemoryGame.App.Services;

[assembly: Dependency(typeof(HapticService))]
namespace MemoryGame.App.iOS.Services
{
    public class HapticService : IHaptic
    {
        public HapticService() { }
        public void ActivateHaptic()
        {
            SystemSound.Vibrate.PlaySystemSound();
        }
    }
}