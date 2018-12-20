using Plugin.Connectivity;

namespace MemoryGame.App.Helper
{
    public static class StringExtensions
    {
        public static int ToInteger(this string numberString)
        {
            int result = 0;
            if (int.TryParse(numberString, out result))
                return result;
            return 0;
        }
    }


    public static class Utils
    {
        public static bool IsConnectedToInternet()
        {
            return CrossConnectivity.Current.IsConnected;
        }
    }
}
