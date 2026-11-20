namespace IndianRunner
{
    // Region mapping preserves the proven safe lane, empty approach/recovery rows,
    // speed curve and rest buffers. Authored encounters reserve their own rows.
    public static class RegionRules
    {
        public const int Ducks=16, Boar=17, Langur=18, Coconuts=19, Coir=20, FishCart=21, Bamboo=22, Log=23;
        public const int Buffalo=24, Goat=25, Tractor=26, Hay=27, Grain=28, PhulkariGate=29, Crab=30, Dog=31, FishCrates=32, BoatCart=33, NetGate=34, CanalBuoy=35;
        public static bool UsesKind(int region,int kind)
        {
            if(region==RegionProfile.Rajasthan) return kind>=1 && kind<=15;
            if(kind==14) return true; // shared rain visitor
            if(region==RegionProfile.Kerala) return kind==6 || kind==15 || kind==CanalBuoy || (kind>=Ducks && kind<=Log);
            if(region==RegionProfile.Punjab) return kind>=Buffalo && kind<=PhulkariGate;
            return kind==6 || kind==Coconuts || (kind>=Crab && kind<=NetGate);
        }
        public static bool IsSlide(int kind) => kind==4 || kind==13 || kind==Bamboo || kind==PhulkariGate || kind==NetGate;
        public static bool ForestRow(int row) => row>=0 && (row/2)%7>=4;
        public static bool BridgeTile(int index) => index%7==1 || index%7==3;
        public static int DynamicKind(int row,bool kerala) => DynamicKind(row,kerala ? RegionProfile.Kerala:RegionProfile.Rajasthan);
        public static int DynamicKind(int row,int region)
        {
            if(AdventureRules.OverridesRow(row))return 0;
            // Keep oncoming traffic outside the new authored encounters.
            if(row>=36 && row%36==0)return region==RegionProfile.Punjab ? Tractor:6;
            int original=RunRules.DynamicKind(row);
            if(region==RegionProfile.Rajasthan || original==0) return original;
            if(region==RegionProfile.Punjab) return original==6 ? Tractor:row%4==0 ? Buffalo:Goat;
            if(region==RegionProfile.Goa) return original==6 ? 6:row%4==0 ? Crab:Dog;
            if(original==6) return 6;
            if(ForestRow(row)) return original==15 ? 15:row%4==0 ? Boar:Langur;
            return original%2==0 ? Ducks:Log;
        }
        public static bool NearDynamic(int row,int region) => DynamicKind(row-1,region)!=0 || DynamicKind(row,region)!=0 || DynamicKind(row+1,region)!=0;
        public static int ObstacleFor(int row,int lane,int seed,bool kerala) => ObstacleFor(row,lane,seed,kerala ? RegionProfile.Kerala:RegionProfile.Rajasthan);
        public static int ObstacleFor(int row,int lane,int seed,int region)
        {
            if(AdventureRules.OverridesRow(row))return AdventureRules.Obstacle(row,lane,seed,region);
            if(row<2 || RunRules.RestBuffer(row) || lane==RunRules.SafeLane(row,seed)) return 0;
            int dynamic=DynamicKind(row,region);
            if(dynamic!=0) return lane==RunRules.HazardLane(row,seed) ? dynamic:0;
            if(NearDynamic(row,region))return 0;
            if(AdventureRules.RoadworksRow(row,region))return lane==(RunRules.SafeLane(row,seed)+1)%3 ? 11:13;
            if(region==RegionProfile.Rajasthan)return RunRules.ObstacleFor(row,lane,seed);
            int kind=RunRules.Kind(row,lane,seed);
            if(region==RegionProfile.Punjab) return kind==1 ? Hay:kind==2 ? Grain:kind==3 ? Hay:PhulkariGate;
            if(region==RegionProfile.Goa) return kind==1 ? Coconuts:kind==2 ? FishCrates:kind==3 ? BoatCart:NetGate;
            return kind==1 ? Coconuts:kind==2 ? Coir:kind==3 ? FishCart:Bamboo;
        }
    }
}
