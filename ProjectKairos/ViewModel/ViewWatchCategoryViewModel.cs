namespace ProjectKairos.ViewModel
{
    public class ViewWatchCategoryViewModel
    {
        public X.PagedList.IPagedList<WatchInIndexPageModel> WatchList { get; set; }
        public int WatchCount { get; set; }
        public string CurrentCategory { get; set; }
    }
}