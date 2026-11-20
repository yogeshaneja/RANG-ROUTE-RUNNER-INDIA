using System;

namespace IndianRunner
{
    // Pure scheduling/order rules. No scene, frame or random-generator dependency.
    public static class JourneyRules
    {
        public const float MinimumLegDistance=600;
        public static bool InTransferWindow(float distance,float nextDistance)
        {
            if(distance<nextDistance)return false;
            int row=(int)Math.Max(0,distance/RunRules.RowSpacing);
            return row>=9 && row%18==9 && distance-row*RunRules.RowSpacing<20;
        }
        public static int NextRegion(int current,int visited,int randomValue,out int updatedVisited)
        {
            current=RegionProfile.Valid(current);visited|=1<<current;
            int all=(1<<RegionProfile.Count)-1;
            if((visited&all)==all)visited=0;
            int count=0;
            for(int i=0;i<RegionProfile.Count;i++)if(i!=current && (visited&(1<<i))==0)count++;
            int pick=(randomValue&int.MaxValue)%count;
            for(int i=0;i<RegionProfile.Count;i++)if(i!=current && (visited&(1<<i))==0 && pick--==0)
            { updatedVisited=visited|(1<<i);return i; }
            throw new InvalidOperationException("No next journey region is available.");
        }
    }
    // In-memory handoff only. Process termination does not resume a live run.
    public sealed class JourneyCheckpoint
    {
        public int Region,StartingRegion,Coins,Lane,Seed,Visited,NextMilestone,Weather;
        public float Distance,Clock,X,MagnetUntil,CompanionReadyAt;
    }
}
