using System;
using System.Collections.Generic;
using System.Linq;
using MemoryGame.API.Models.DB;

namespace MemoryGame.API.Models.DataManager
{
    #region DTO
    public class ChallengerViewModel
    {
        public int ChallengerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte Best { get; set; }
        public DateTime DateAchieved { get; set; }
    }
    #endregion

    #region HTTP Response Object
    public class HTTPApiResponse
    {
        public enum StatusResponse
        {
            Success = 1,
            Fail = 2
        }

        public StatusResponse Status { get; set; }
        public string StatusDescription { get; set; }
        public int StatusCode { get; set; }
    }
    #endregion

    #region Data Access
    public class GameManager
    {
        public IEnumerable<ChallengerViewModel> GetAll { get { return GetAllChallengerRank(); } }

        public List<ChallengerViewModel> GetAllChallengerRank()
        {

            using (MemoryGameEntities db = new MemoryGameEntities())
            {
                var result = (from c in db.Challengers
                              join r in db.Ranks on c.ChallengerID equals r.ChallengerID
                              select new ChallengerViewModel
                              {
                                  ChallengerID = c.ChallengerID,
                                  FirstName = c.FirstName,
                                  LastName = c.LastName,
                                  Best = r.Best,
                                  DateAchieved = r.DateAchieved
                              }).OrderByDescending(o => o.Best).ThenBy(o => o.DateAchieved);

                return result.ToList();
            }
        }

        public HTTPApiResponse UpdateCurrentBest(DB.Rank user)
        {
            using (MemoryGameEntities db = new MemoryGameEntities())
            {
                var data = db.Ranks.Where(o => o.ChallengerID == user.ChallengerID);
                if (data.Any())
                {
                    Rank rank = data.FirstOrDefault();
                    rank.Best = user.Best;
                    rank.DateAchieved = user.DateAchieved;
                    db.SaveChanges();
                }
                else
                {
                    db.Ranks.Add(user);
                    db.SaveChanges();
                }
           
            }

            return new HTTPApiResponse
            {
                Status = HTTPApiResponse.StatusResponse.Success,
                StatusCode = 200,
                StatusDescription = "Operation successful."
            };
        }

        public int GetChallengerID(string email)
        {
            using (MemoryGameEntities db = new MemoryGameEntities())
            {
                var data = db.Challengers.Where(o => o.Email.ToLower().Equals(email.ToLower()));
                if (data.Any())
                {
                    return data.FirstOrDefault().ChallengerID;
                }

                return 0;
            }
        }

        public HTTPApiResponse AddChallenger(DB.Challenger c)
        {
            HTTPApiResponse response = null;
            using (MemoryGameEntities db = new MemoryGameEntities())
            {
                var data = db.Challengers.Where(o => o.Email.ToLower().Equals(c.Email.ToLower()));
                if (data.Any())
                {
                    response = new HTTPApiResponse
                    {
                        Status = HTTPApiResponse.StatusResponse.Fail,
                        StatusCode = 400,
                        StatusDescription = "User with associated email already exist."
                    };
                }
                else
                {
                    db.Challengers.Add(c);
                    db.SaveChanges();

                    response = new HTTPApiResponse
                    {
                        Status = HTTPApiResponse.StatusResponse.Success,
                        StatusCode = 200,
                        StatusDescription = "Operation successful."
                    };
                }

                return response;
            }
        }

        public ChallengerViewModel GetChallengerByEmail(string email)
        {
            using (MemoryGameEntities db = new MemoryGameEntities())
            {
                var result = (from c in db.Challengers
                              join r in db.Ranks on c.ChallengerID equals r.ChallengerID
                              where c.Email.ToLower().Equals(email.ToLower())
                              select new ChallengerViewModel
                              {
                                  ChallengerID = c.ChallengerID,
                                  FirstName = c.FirstName,
                                  LastName = c.LastName,
                                  Best = r.Best,
                                  DateAchieved = r.DateAchieved
                              });
                if (result.Any())
                    return result.SingleOrDefault();
            }
            return new ChallengerViewModel();
        }

        public HTTPApiResponse DeleteChallenger(int id)
        {
            HTTPApiResponse response = null;
            using (MemoryGameEntities db = new MemoryGameEntities())
            {
                var data = db.Challengers.Where(o => o.ChallengerID == id);
                if (data.Any())
                {
                    try
                    {
                        var rankData = db.Ranks.Where(o => o.ChallengerID == id);
                        if (rankData.Any())
                        {
                            db.Ranks.Remove(rankData.FirstOrDefault());
                            db.SaveChanges();
                        }

                        db.Challengers.Remove(data.FirstOrDefault());
                        db.SaveChanges();

                        response = new HTTPApiResponse
                        {
                            Status = HTTPApiResponse.StatusResponse.Success,
                            StatusCode = 200,
                            StatusDescription = "Operation successful."
                        };
                    }
                    catch (System.Data.Entity.Validation.DbUnexpectedValidationException)
                    {
                        //handle error and log

                        response = new HTTPApiResponse
                        {
                            Status = HTTPApiResponse.StatusResponse.Fail,
                            StatusCode = 500,
                            StatusDescription = "An unexpected error occured."
                        };
                    }
                }
                else
                {
                    response = new HTTPApiResponse
                    {
                        Status = HTTPApiResponse.StatusResponse.Fail,
                        StatusCode = 400,
                        StatusDescription = "Associated ID not found."
                    };
                }

                return response;
            }
        }
    }
    #endregion
}

