using MemoryGame.API.Models.DataManager;
using MemoryGame.API.Models.DB;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MemoryGame.API.API
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/game/players")]
    public class GameController : ApiController
    {
        GameManager _gm;
        public GameController()
        {
            _gm = new GameManager();
        }

        [HttpGet, Route("")]
        public IEnumerable<ChallengerViewModel> Get()
        {
            return _gm.GetAll;
        }

        [HttpGet, Route("{email}")]
        public int GetPlayerID(string email)
        {
            return _gm.GetChallengerID(email);
        }

        [HttpGet, Route("profile/{email}")]
        public ChallengerViewModel GetPlayerProfile(string email)
        {
            return _gm.GetChallengerByEmail(email);
        }

        [HttpPost, Route("")]
        public HTTPApiResponse AddPlayer(Challenger user)
        {
            return _gm.AddChallenger(user);
        }

 
        [HttpPost, Route("score")]
        public void UpdateScore(Rank user)
        {
            _gm.UpdateCurrentBest(user);

            HttpClient client = new HttpClient();
            var uri = new Uri($"http://localhost:57865/api/ranking");
            client.PostAsync(uri, null).Wait();

        }

        [HttpDelete, Route("{id}")]
        public HTTPApiResponse DeletePlayer(int id)
        {
            return _gm.DeleteChallenger(id);
        }

        
    }
}

