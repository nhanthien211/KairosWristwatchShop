namespace ProjectKairos.ViewModel
{
    public class WatchReviewViewModel
    {
        public int OrderId { get; set; }
        public int WatchId { get; set; }
        public string Thumbnail { get; set; }
        public string WatchCode { get; set; }
        public int TotalReview { get; set; }
        public double? YourRating { get; set; }

    }
}