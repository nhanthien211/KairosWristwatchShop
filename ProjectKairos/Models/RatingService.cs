using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectKairos.Models
{
    public class RatingService
    {
        private KAIROS_SHOPEntities db;

        public RatingService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public bool RateAWatch(int watchId, string username, int star)
        {
            Review review = db.Reviews.Find(watchId, username);

            if (review == null)
            {
                db.Reviews.Add(new Review
                {
                    WatchId = watchId,
                    Username = username,
                    Rating = star,
                    ReviewDate = DateTime.Now
                });
            }
            else
            {
                db.Reviews.Attach(review);
                review.Rating = star;
                review.ReviewDate = DateTime.Now;
            }
            

            int result = db.SaveChanges();
            if (result > 0)
            {
                return true;
            }
            return false;
        }
    }
}