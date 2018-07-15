namespace ProjectKairos.ViewModel
{
    public class ViewWatchCategoryViewModel
    {
        public X.PagedList.IPagedList<WatchInIndexPageModel> WatchList { get; set; }
        public int WatchCount { get; set; }
        public string SearchValue { get; set; }
        public string PriceRange { get; set; }
        public string SortFilter { get; set; }
    }
}