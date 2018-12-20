using System;

namespace MemoryGame.App.Classes
{
    public class PlayerProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class PlayerScore
    {
        public int ChallengerID { get; set; }
        public byte Best { get; set; }
        public DateTime DateAchieved { get; set; }
    }

    public class PlayerData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte Best { get; set; }
        public DateTime DateAchieved { get; set; }
    }

}
