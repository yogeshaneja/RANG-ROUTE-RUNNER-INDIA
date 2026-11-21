namespace IndianRunner
{
    // Repeating authored encounters coexist with the garden/region-transfer cadence.
    public static class AdventureRules
    {
        public const float RideHalfWidth=.80f;
        public const float CompanionCooldown=8;
        public static int Phase(int row) => row<0 ? -1:row%36;
        public static bool SectionRow(int row) => Phase(row)>=4 && Phase(row)<=7;
        public static bool RideRow(int row) => Phase(row)>=18 && Phase(row)<=23;
        public static bool OverridesRow(int row) => (Phase(row)>=3 && Phase(row)<=8) || (Phase(row)>=17 && Phase(row)<=26);
        public static bool RoadworksRow(int row,int region) => region==RegionProfile.Rajasthan && row>=34 && row%36==34 && (row/36)%2==0;
        public static bool RainVisitorRow(int row,int region) => row>=34 && row%36==34 && !RegionRules.NearDynamic(row,region);
        public static int Row(float distance) => (int)System.Math.Max(0,distance/RunRules.RowSpacing);
        public static bool BoardingWindow(float distance) => Phase(Row(distance))==18;
        public static int PickupValue(int row) => FestivalRushRules.ActiveRow(row) ? 3:SectionRow(row) || RideRow(row) ? 2:1;
        public static int RideObstacle(int region) => ExpandedRegionRules.IsExpanded(region) ? ExpandedRegionRules.Traffic(region):region==RegionProfile.Kerala ? RegionRules.CanalBuoy:region==RegionProfile.Punjab ? RegionRules.Tractor:region==RegionProfile.Goa ? RegionRules.BoatCart:3;
        public static int Obstacle(int row,int lane,int seed,int region)
        {
            if(FestivalRushRules.ActiveRow(row))return 0;
            if(lane==RunRules.SafeLane(row,seed) || lane!=(RunRules.SafeLane(row,seed)+1)%3)return 0;
            // Boarding, alternating recovery and dismount rows stay clear.
            if(RideRow(row))return Phase(row)==19 || Phase(row)==21 ? RideObstacle(region):0;
            if(!SectionRow(row))return 0;
            bool slide=row%2==1;
            if(ExpandedRegionRules.IsExpanded(region))return slide ? ExpandedRegionRules.Slide(region):ExpandedRegionRules.Jump(region);
            return region==RegionProfile.Kerala ? (slide ? RegionRules.Bamboo:RegionRules.Coir):
                region==RegionProfile.Punjab ? (slide ? RegionRules.PhulkariGate:RegionRules.Grain):
                region==RegionProfile.Goa ? (slide ? RegionRules.NetGate:RegionRules.FishCrates):(slide ? 4:1);
        }
        public static string SectionName(int region) => ExpandedRegionRules.IsExpanded(region) ? ExpandedRegionRules.Passage(region):region==RegionProfile.Kerala ? "CANOPY BRIDGE DASH":region==RegionProfile.Punjab ? "HARVEST FESTIVAL ALLEY":region==RegionProfile.Goa ? "COASTAL BOARDWALK":"FORT LANTERN PASSAGE";
        public static string RideName(int region) => ExpandedRegionRules.IsExpanded(region) ? ExpandedRegionRules.Ride(region):region==RegionProfile.Kerala ? "BACKWATER CANOE":region==RegionProfile.Punjab ? "HARVEST TROLLEY":region==RegionProfile.Goa ? "COLOURFUL COASTAL SCOOTER":"DESERT WIND CART";
    }
}
