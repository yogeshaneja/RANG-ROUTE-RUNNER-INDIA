namespace IndianRunner
{
    // Persisted IDs are stable; old Rajasthan/Kerala saves retain their meaning.
    public static class RegionProfile
    {
        public const int Rajasthan=0, Kerala=1, Punjab=2, Goa=3, Count=4;
        public static int Valid(int region) => region>=0 && region<Count ? region:Rajasthan;
        public static string Name(int region) => region==Kerala ? "KERALA":region==Punjab ? "PUNJAB":region==Goa ? "GOA":"RAJASTHAN";
        public static string Title(int region) => region==Kerala ? "KERALA - MONSOON TRAIL":region==Punjab ? "PUNJAB - HARVEST HUSTLE":region==Goa ? "GOA - COASTAL COLOUR RUN":"THE RAJASTHAN RUN";
        public static string Code(int region) => region==Kerala ? "KL":region==Punjab ? "PB":region==Goa ? "GA":"RJ";
        public static string BestKey(int region) => region==Kerala ? "runner.best.kerala":region==Punjab ? "runner.best.punjab":region==Goa ? "runner.best.goa":"runner.best";
        public static string TokenName(int region) => region==Kerala ? "LOTUS":region==Punjab ? "PHULKARI":region==Goa ? "SHELL":"MIRROR";
        public static string GuardianName(int region) => region==Kerala ? "THE MONSOON LEAF KEEPER":region==Punjab ? "THE BHANGRA CLOUD DANCER":region==Goa ? "THE SEA-BREEZE CAPTAIN":"THE THAR CLOUD GUARDIAN";
    }
}
