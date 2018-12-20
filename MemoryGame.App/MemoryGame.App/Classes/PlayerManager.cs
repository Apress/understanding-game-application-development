using MemoryGame.App.Helper;
using System;
using System.Threading.Tasks;

namespace MemoryGame.App.Classes
{
    public static class PlayerManager
    {
        public static void Save(PlayerProfile player)
        {
            Settings.PlayerFirstName = player.FirstName;
            Settings.PlayerLastName = player.LastName;
            Settings.PlayerEmail = player.Email;
        }

        public static PlayerProfile GetPlayerProfileFromLocal()
        {
            return new PlayerProfile
            {
                FirstName = Settings.PlayerFirstName,
                LastName = Settings.PlayerLastName,
                Email = Settings.PlayerEmail
            };
        }

        public static PlayerScore GetPlayerScoreFromLocal()
        {
            return new PlayerScore
            {
                ChallengerID = Settings.PlayerID,
                Best = Convert.ToByte(Settings.TopScore),
                DateAchieved = Settings.DateAchieved
            };
        }

        public static void UpdateBest(int score)
        {
            if (Settings.TopScore < score)
            {
                Settings.TopScore = score;
                Settings.DateAchieved = DateTime.UtcNow;
            }
        }

        public static int GetBestScore(int currentLevel)
        {
            if (Settings.TopScore > currentLevel)
                return Settings.TopScore;
            else
                return currentLevel;
        }

        public async static Task<bool> Sync()
        {
            REST.GameAPI api = new REST.GameAPI();
            bool result = false;

            try
            {
                if (!Settings.IsProfileSync)
                    result = await api.SavePlayerProfile(PlayerManager.GetPlayerProfileFromLocal(), true);

                if (Settings.PlayerID == 0)
                    Settings.PlayerID = await api.GetPlayerID(Settings.PlayerEmail);

                result = await api.SavePlayerScore(PlayerManager.GetPlayerScoreFromLocal());

            }
            catch
            {
                return result;
            }

            return result;
        }

        public async static Task<bool> CheckScoreAndSync(int score)
        {
            if (Settings.TopScore < score)
            {
                UpdateBest(score);
                var response = await Sync();
                return response == true ? true : false;
            }
            else
                return false;
        }

        public async static Task<PlayerData> CheckExistingPlayer(string email)
        {
            REST.GameAPI api = new REST.GameAPI();
            PlayerData player = new PlayerData();

            if (Utils.IsConnectedToInternet())
            {
                player = await api.GetPlayerData(email);
            }

            return player;
        }
    }
}


