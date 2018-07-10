using ProjectKairos.Utilities;

namespace ProjectKairos.ViewModel
{
    public class WatchInIndexPageModel
    {
        public int WatchID { get; set; }
        public string WatchCode { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public bool Status { get; set; }

        private string thumbnail;

        public string Thumbnail
        {
            get { return thumbnail; }
            set { thumbnail = MyCustomUtility.RelativeFromAbsolutePath(value); }
        }

    }
}