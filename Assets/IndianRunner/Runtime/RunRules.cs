using System;

namespace IndianRunner
{
    // Pure rules: deliberately independent of Unity, usable in Edit Mode checks.
    public static class RunRules
    {
        public const float LaneWidth = 2.65f;
        public const float LaneSpeed = 14f;
        public const float StartSpeed = 11f;
        public const float MaxSpeed = 18f;
        public const float RowSpacing = 28f;
        public const float JumpVelocity = 9f;
        public const float Gravity = 24f;
        public const float PlayerHalfWidth = .34f;
        public const float ObstacleHalfDepth = 1.1f;
        public const float CamelHalfWidth = 1.4f;
        public const float CamelHalfDepth = .55f;
        public const float CamelWarningDistance = 45f;
        public const int ObstacleKinds = 4;
        // Cubic easing has peak derivative 1.5; this duration preserves LaneSpeed.
        public static float LaneTravelTime(float distance) => Math.Max(.09f, 1.5f * Math.Abs(distance) / LaneSpeed);
        public static float LaneProgress(float progress)
        {
            float t = Math.Max(0, Math.Min(1, progress));
            return t * t * (3 - 2 * t);
        }
        public static float Speed(float distance) => Math.Min(MaxSpeed, StartSpeed + Math.Max(0,distance) / 180f);
        public static int Score(float distance, int coins) => (int)Math.Max(0, distance) + coins * 10;
        public static float LaneX(int lane) => (Math.Max(0, Math.Min(2, lane)) - 1) * LaneWidth;
        public static int SafeLane(int row, int seed) => (int)(Mix((uint)row ^ (uint)seed) % 3);
        public static int Kind(int row, int lane, int seed) => 1 + (int)(Mix((uint)(row * 7 + lane + 71) ^ (uint)seed) % ObstacleKinds);
        public static bool HasCamel(int row, int seed) => row >= 2 && !NearDynamic(row) && !RestBuffer(row) && !RoadworksRow(row) && row % 4 == 2 && SafeLane(row, seed) != 1;
        // Empty neighbouring rows leave time to enter/leave the warned lane.
        public static bool RestRow(int row) => row >= 10 && row%18 >= 10 && row%18 <= 12;
        public static bool RestBuffer(int row) => row >= 9 && row%18 >= 9 && row%18 <= 13;
        public static bool GuardianWindow(float distance)
        {
            int row=(int)Math.Max(0,distance/RowSpacing);
            return row>=10 && row%36==10 && distance-row*RowSpacing>=8;
        }
        public static bool RestDistance(float distance) => RestRow((int)Math.Max(0,distance/RowSpacing));
        public static bool DanceRestDistance(float distance)
        {
            int row=(int)Math.Max(0,distance/RowSpacing);
            return RestRow(row) && ((row-10)/18)%3==0;
        }
        public static bool PouchRow(int row) => row >= 11 && row%18 == 11;
        // Rare fantasy surprise: first at 910 m, then every 2,016 m. Skip rest buffers.
        // Rain visitors replace one eligible row, preserving its existing escape lane.
        public static bool RainVisitorRow(int row) => row>=6 && row%18==6 && !RestBuffer(row) && !NearDynamic(row) && !RoadworksRow(row);
        public static bool CanAdmitRainVisitor(float intensity,float ahead) => intensity>.12f && ahead>=180 && ahead<=210;
        public static bool RoadworksRow(int row) => row>=18 && row%18==0;
        public static int DynamicKind(int row)
        {
            if(RestBuffer(row)) return 0;
            if(row>=32 && row%72==32) return 7;
            if(row>=2 && row%36==2) return 8;
            if(row>=8 && row%36==8) return 10;
            if(row>=14 && row%36==14) return 12;
            if(row>=20 && row%36==20) return 9;
            if(row>=16 && row%36==16) return 15; // oncoming elephant; keep this rules file independent of Unity types
            return row>=4 && row%12==4 ? 6 : 0;
        }
        public static bool NearDynamic(int row) => DynamicKind(row-1) != 0 || DynamicKind(row) != 0 || DynamicKind(row+1) != 0;
        public static int HazardLane(int row, int seed) => DynamicKind(row)==12 ? (SafeLane(row,seed)==0 ? 2:0) : (SafeLane(row, seed)+1)%3;
        public static float HazardX(int kind,int lane,float ahead)
        {
            float offset=kind==12 ? 2.2f*Math.Max(0,Math.Min(1,(ahead-10)/35)) : 0;
            return LaneX(lane)+(lane==0 ? -offset:offset);
        }
        // Bike meets the runner at its row, but travels towards them before and after.
        public static float HazardZ(int kind, float rowZ, float distance) => rowZ + (kind == 37 || kind == 40 || kind == 44 ? .35f : kind == 6 ? .65f : kind==15 ? .35f : kind==17 ? .18f : kind==26 ? .25f : kind==8 ? .45f : kind==9 ? -.20f : 0)*(rowZ-distance);
        public static float HazardY(int kind, float ahead)
        {
            float t = Math.Max(0,Math.Min(1,(40-ahead)/32));
            if(kind==8) return .62f;
            if(kind==10)
            {
                float drop=Math.Max(0,Math.Min(1,(40-ahead)/30));
                return 6.5f*(1-drop*drop*(3-2*drop));
            }
            return kind == 7 ? 9f-8.2f*t*t*(3-2*t) : 0;
        }
        public static float HazardBaseY(int kind,float ahead) => kind==8 ? 0:HazardY(kind,ahead);
        public static float CamelX(int safeLane, float distanceAhead)
        {
            if (safeLane != 0 && safeLane != 2) throw new ArgumentOutOfRangeException(nameof(safeLane));
            float progress = Math.Max(0, Math.Min(1, (CamelWarningDistance - distanceAhead) / (CamelWarningDistance + 12)));
            return (safeLane == 0 ? 1 : -1) * LaneWidth * 2 * (1 - progress);
        }
        public static int ObstacleFor(int row, int lane, int seed)
        {
            if (row < 2 || RestBuffer(row) || lane == SafeLane(row, seed)) return 0;
            if (DynamicKind(row) != 0) return DynamicKind(row)==9 ? 9 : lane == HazardLane(row,seed) ? DynamicKind(row) : 0;
            if (NearDynamic(row)) return 0;
            if (HasCamel(row, seed)) return lane == 1 ? 5 : 0;
            if (RoadworksRow(row)) return lane==(SafeLane(row,seed)+1)%3 ? 11:13;
            return Kind(row, lane, seed);
        }
        public static bool IsBlocked(int row, int lane, int seed) => ObstacleFor(row, lane, seed) != 0;
        public static bool Overlaps(float aMin, float aMax, float bMin, float bMax) => aMin < bMax && aMax > bMin;
        private static uint Mix(uint x)
        {
            unchecked { x ^= x >> 16; x *= 0x7feb352d; x ^= x >> 15; x *= 0x846ca68b; return x ^ (x >> 16); }
        }
    }
}
