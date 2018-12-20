using System.Web.Http;

namespace MemoryGame.Web.Controllers
{
    [RoutePrefix("api/ranking")]
    public class LeaderBoardAppController : ApiController
    {
        [HttpPost,Route("")]
        public void Broadcast()
        {
            LeaderboardHub.Broadcast();
        }   
    }
}


