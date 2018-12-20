using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MemoryGame.App.Classes;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MemoryGame.App.REST
{
    public class GameAPI
    {
        //replace the value of APIUri with the published URI to where your API is hosted. 
        //E.g http://yourdomain.com/yourappname/api/game 
        private const string APIUri = "http://192.168.0.14:5000/api/game/players"; 
        HttpClient client;
        public GameAPI()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Clear();
            //Define request data format  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> SavePlayerProfile(PlayerProfile data, bool isNew = false)
        {
            var uri = new Uri(APIUri);

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            if (isNew)
                response = await ProcessPostAsync(uri, content);


            if (response.IsSuccessStatusCode)
            {
                Settings.IsProfileSync = true;
                return true;
            }

            return false;
        }

        public async Task<bool> SavePlayerScore(PlayerScore data)
        {
            var uri = new Uri($"{APIUri}/score");

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await ProcessPostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }

        public async Task<int> GetPlayerID(string email)
        {
            var uri = new Uri($"{APIUri}/{email}/");
            int id = 0;

            var response = await ProcessGetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                id = JsonConvert.DeserializeObject<int>(content);
            }

            return id;
        }

        public async Task<PlayerData> GetPlayerData(string email)
        {
            var uri = new Uri($"{APIUri}/profile/{email}/");
            PlayerData player = null;

            var response = await ProcessGetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                player = new PlayerData();
                var content = await response.Content.ReadAsStringAsync();
                player = JsonConvert.DeserializeObject<PlayerData>(content);
            }

            return player;
        }

        private async Task<HttpResponseMessage> ProcessPostAsync(Uri uri, StringContent content)
        {
            return await client.PostAsync(uri, content); ;
        }

        private async Task<HttpResponseMessage> ProcessGetAsync(Uri uri)
        {
            return await client.GetAsync(uri);
        }
    }
}


