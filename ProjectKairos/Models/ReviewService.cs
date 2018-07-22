using System;
using System.Collections.Generic;
using System.Linq;
using ProjectKairos.ViewModel;
using System.Data.Entity;

namespace ProjectKairos.Models
{
    public class ReviewService
    {
        private KAIROS_SHOPEntities db;

        public ReviewService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public WatchReviewViewModel ViewWatchReview(int watchId, string username, int orderId)
        {
            var viewModel = db.Reviews
                .Where(r => r.WatchId == watchId && r.Username == username)
                .Select(r => new WatchReviewViewModel
                {
                    YourRating = r.Rating
                }).FirstOrDefault();
            if (viewModel == null)
            {
                viewModel = new WatchReviewViewModel();
            }

            viewModel.OrderId = orderId;
            viewModel.WatchId = watchId;
            viewModel.Thumbnail = db.Reviews.Include(r => r.Watch).Where(r => r.WatchId == watchId).Select(r => r.Watch.Thumbnail).FirstOrDefault();
            viewModel.WatchCode = db.Reviews.Include(r => r.Watch).Where(r => r.WatchId == watchId).Select(r => r.Watch.WatchCode).FirstOrDefault();
            viewModel.TotalReview = db.Reviews.Count(r => r.WatchId == watchId);


            return viewModel;
        }

        public bool RateStarWatch(int watchId, string username, int star)
        {
            Review r = db.Reviews.Find(watchId, username);
            if (r != null)
            {
                db.Reviews.Attach(r);
                r.Rating = star;
                r.ReviewDate = DateTime.Now;

            }
            else
            {
                r = new Review
                {
                    Rating = star,
                    ReviewDate = DateTime.Now,
                    Username = username,
                    WatchId = watchId
                };
                db.Reviews.Add(r);
            }
            int result = db.SaveChanges();
            return (db.Entry(r).State == EntityState.Unchanged || result > 0);
        }

        public List<ReviewInProductViewModel> LoadWatchAllReview(string watchCode)
        {
            return db.Reviews
                .Include(r => r.Account)
                .Include(r => r.Watch)
                .Where(r => r.Watch.WatchCode.Equals(watchCode, StringComparison.OrdinalIgnoreCase))
                .Select(r => new ReviewInProductViewModel
                {
                    FullName = r.Account.LastName + " " + r.Account.FirstName,
                    ReviewDate = r.ReviewDate,
                    Rating = r.Rating
                }).ToList();
        }
    }
}