using System;

namespace IndianRunner
{
    // Pure, bounded progress model; persistence is owned by RunManager.
    public sealed class FestivalProgress
    {
        public int Set { get; private set; }
        public int Tokens { get; private set; }
        public int Slides { get; private set; }
        public int Distance { get; private set; }
        public int Outfit { get; private set; }
        public int TokenGoal => 10+Math.Min(Set,20)*5;
        public int SlideGoal => 1+Math.Min(Set,10);
        public int DistanceGoal => 150+Math.Min(Set,20)*50;
        public int Unlocked => Math.Min(4,Set+1);
        public int Badges => Math.Max(0,Set-3);
        public int Completed => (Tokens>=TokenGoal ? 1:0)+(Slides>=SlideGoal ? 1:0)+(Distance>=DistanceGoal ? 1:0);
        public bool Ready => Completed==3;
        public FestivalProgress(int set=0,int tokens=0,int slides=0,int distance=0,int outfit=0)
        {
            Set=Math.Max(0,Math.Min(9999,set));
            Tokens=Math.Max(0,Math.Min(TokenGoal,tokens)); Slides=Math.Max(0,Math.Min(SlideGoal,slides));
            Distance=Math.Max(0,Math.Min(DistanceGoal,distance)); Outfit=Math.Max(0,Math.Min(Unlocked-1,outfit));
        }
        public bool Add(int tokens,int slides,float distance)
        {
            int before=Completed;
            Tokens=Math.Min(TokenGoal,Tokens+Math.Max(0,Math.Min(100,tokens)));
            Slides=Math.Min(SlideGoal,Slides+Math.Max(0,Math.Min(10,slides)));
            Distance=Math.Min(DistanceGoal,Math.Max(Distance,(int)Math.Max(0,Math.Min(100000,distance))));
            return Completed>before;
        }
        public bool Claim()
        {
            if(!Ready || Set>=9999) return false;
            Set++; Tokens=Slides=Distance=0; Outfit=Unlocked-1; return true;
        }
        public void NextOutfit() { Outfit=(Outfit+1)%Unlocked; }
        public static string OutfitName(int outfit) => outfit==1 ? "MARIGOLD" : outfit==2 ? "FESTIVAL PINK" : outfit==3 ? "ROYAL BLUE" : "TURQUOISE";
    }
}
