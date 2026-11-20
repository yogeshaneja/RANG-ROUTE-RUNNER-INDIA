namespace IndianRunner
{
    // Region mapping preserves the proven safe lane, empty approach/recovery rows,
    // speed curve and rest buffers. It does not change Rajasthan generation.
    public static class RegionRules
    {
        public const int Ducks=16, Boar=17, Langur=18, Coconuts=19, Coir=20, FishCart=21, Bamboo=22, Log=23;
        public static bool ForestRow(int row) => row>=0 && (row/2)%7>=4;
        public static bool BridgeTile(int index) => index%7==1 || index%7==3;
        public static int DynamicKind(int row,bool kerala)
        {
            int original=RunRules.DynamicKind(row);
            if(!kerala || original==0) return original;
            if(original==6) return 6;
            if(ForestRow(row)) return original==15 ? 15:row%4==0 ? Boar:Langur;
            return original%2==0 ? Ducks:Log;
        }
        public static int ObstacleFor(int row,int lane,int seed,bool kerala)
        {
            if(!kerala) return RunRules.ObstacleFor(row,lane,seed);
            if(row<2 || RunRules.RestBuffer(row) || lane==RunRules.SafeLane(row,seed)) return 0;
            int dynamic=DynamicKind(row,true);
            if(dynamic!=0) return lane==RunRules.HazardLane(row,seed) ? dynamic:0;
            if(RunRules.NearDynamic(row)) return 0;
            int kind=RunRules.Kind(row,lane,seed);
            return kind==1 ? Coconuts:kind==2 ? Coir:kind==3 ? FishCart:Bamboo;
        }
    }
}
