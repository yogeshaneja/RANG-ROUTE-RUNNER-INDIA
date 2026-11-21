namespace IndianRunner
{
    // Data mapping only; scheduling, safe lanes, jump and slide physics are shared.
    public static class ExpandedRegionRules
    {
        public static bool IsExpanded(int region) => region>=RegionProfile.Shillong && region<RegionProfile.Count;
        public static int Traffic(int region) => region==RegionProfile.Shillong ? RegionRules.HillBus:region==RegionProfile.Kolkata ? RegionRules.YellowTaxi:RegionRules.SafariJeep;
        public static int Jump(int region) => region==RegionProfile.Shillong ? RegionRules.PineappleBasket:region==RegionProfile.Kolkata ? RegionRules.SweetCrates:region==RegionProfile.MadhyaPradesh ? RegionRules.ForestBaskets:RegionRules.AppleCrates;
        public static int Slide(int region) => region==RegionProfile.Shillong ? RegionRules.ForestBranch:region==RegionProfile.Kolkata ? RegionRules.FlowerAwning:region==RegionProfile.MadhyaPradesh ? RegionRules.ForestBranch:RegionRules.WoolAwning;
        public static bool UsesKind(int region,int kind)
        {
            if(kind==14 || kind==Traffic(region) || kind==Jump(region) || kind==Slide(region))return true;
            if(region==RegionProfile.Shillong)return kind==RegionRules.Goat || kind==RegionRules.Langur || kind==RegionRules.Log;
            if(region==RegionProfile.Kolkata)return kind==RegionRules.Dog;
            if(region==RegionProfile.MadhyaPradesh)return kind==RegionRules.Deer || kind==12 || kind==RegionRules.Log;
            return kind==RegionRules.Sheep || kind==RegionRules.Goat;
        }
        public static int Dynamic(int original,int row,int region)
        {
            if(original==0)return 0;
            if(original==6)return Traffic(region);
            if(region==RegionProfile.Shillong)return row%4==0 ? RegionRules.Goat:RegionRules.Langur;
            if(region==RegionProfile.Kolkata)return RegionRules.Dog;
            if(region==RegionProfile.MadhyaPradesh)return row%4==0 ? RegionRules.Deer:12;
            return row%4==0 ? RegionRules.Sheep:RegionRules.Goat;
        }
        public static int Static(int kind,int region) => kind==1 ? Jump(region):kind==2 ?
            (region==RegionProfile.Shillong || region==RegionProfile.MadhyaPradesh ? RegionRules.Log:Jump(region)):kind==3 ? Traffic(region):Slide(region);
        public static string Passage(int region) => region==RegionProfile.Shillong ? "PINE LAKE BRIDGE":region==RegionProfile.Kolkata ? "RIVERFRONT FLOWER ARCADE":region==RegionProfile.MadhyaPradesh ? "SAL FOREST CANOPY":"APPLE ORCHARD WALK";
        public static string Ride(int region) => region==RegionProfile.Shillong ? "HILL GARDEN TROLLEY":region==RegionProfile.Kolkata ? "RIVERFRONT FLOWER CART":region==RegionProfile.MadhyaPradesh ? "FOREST DISCOVERY BUGGY":"APPLE HARVEST TROLLEY";
        public static string Festival(int region) => region==RegionProfile.Shillong ? "PINE HILL BLOSSOM RUSH":region==RegionProfile.Kolkata ? "RIVER FLOWER FAIR RUSH":region==RegionProfile.MadhyaPradesh ? "FOREST COLOUR FESTIVAL":"APPLE BLOSSOM FESTIVAL";
    }
}
