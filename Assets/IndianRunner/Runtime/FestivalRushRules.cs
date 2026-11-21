namespace IndianRunner
{
    // A rare bonus version of an authored passage, measured in global journey rows.
    public static class FestivalRushRules
    {
        public const int PeriodRows=108;
        public const int FirstRow=4,LastRow=7;
        public static bool ActiveRow(int row) => row>=0 && row%PeriodRows>=FirstRow && row%PeriodRows<=LastRow;
        public static bool Approaching(int row) => row>=0 && row%PeriodRows==FirstRow-1;
        public static float Remaining(float distance) => ActiveRow(AdventureRules.Row(distance)) ?
            ((AdventureRules.Row(distance)/PeriodRows)*PeriodRows+LastRow+1)*RunRules.RowSpacing-distance:0;
        public static string Name(int region) => ExpandedRegionRules.IsExpanded(region) ? ExpandedRegionRules.Festival(region):region==RegionProfile.Kerala ? "FLOWER FESTIVAL RUSH":
            region==RegionProfile.Punjab ? "HARVEST MELA RUSH":region==RegionProfile.Goa ? "COASTAL CARNIVAL RUSH":"DESERT KITE FESTIVAL RUSH";
    }
}
