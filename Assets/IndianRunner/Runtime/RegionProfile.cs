namespace IndianRunner
{
    // Append-only save IDs. Existing four-state saves keep their meaning.
    public static class RegionProfile
    {
        public const int Rajasthan=0,Kerala=1,Punjab=2,Goa=3,Shillong=4,Kolkata=5,MadhyaPradesh=6,Himachal=7,Count=8;
        private static readonly string[] names={"RAJASTHAN","KERALA","PUNJAB","GOA","SHILLONG","KOLKATA","MADHYA PRADESH","HIMACHAL"};
        private static readonly string[] titles={"THE RAJASTHAN RUN","KERALA - MONSOON TRAIL","PUNJAB - HARVEST HUSTLE","GOA - COASTAL COLOUR RUN","MEGHALAYA - SHILLONG MIST RUN","WEST BENGAL - KOLKATA RIVER RUN","MADHYA PRADESH - FOREST TRAIL","HIMACHAL - APPLE VALLEY DASH"};
        private static readonly string[] codes={"RJ","KL","PB","GA","ML","WB","MP","HP"};
        private static readonly string[] keys={"runner.best","runner.best.kerala","runner.best.punjab","runner.best.goa","runner.best.shillong","runner.best.kolkata","runner.best.mp","runner.best.himachal"};
        private static readonly string[] tokens={"MIRROR","LOTUS","PHULKARI","SHELL","PINE CONE","CLAY CUP","FOREST LEAF","APPLE"};
        private static readonly string[] guardians={"THE THAR CLOUD GUARDIAN","THE MONSOON LEAF KEEPER","THE BHANGRA CLOUD DANCER","THE SEA-BREEZE CAPTAIN","THE PINE MIST EXPLORER","THE RIVER CLOUD STORYTELLER","THE SAL FOREST KEEPER","THE APPLE VALLEY GUIDE"};
        public static int Valid(int region) => region>=0 && region<Count ? region:Rajasthan;
        public static string Name(int region) => names[Valid(region)];
        public static string Title(int region) => titles[Valid(region)];
        public static string Code(int region) => codes[Valid(region)];
        public static string BestKey(int region) => keys[Valid(region)];
        public static string TokenName(int region) => tokens[Valid(region)];
        public static string GuardianName(int region) => guardians[Valid(region)];
    }
}
