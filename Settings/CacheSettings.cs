namespace SalesIndicators.API.Settings
{
    public class CacheSettings
    {
        public int TotalRevenueCacheMinutes { get; set; }
        public int SalesByRegionCacheMinutes { get; set; }
        public bool UseSlidingExpiration { get; set; }
    }
}
