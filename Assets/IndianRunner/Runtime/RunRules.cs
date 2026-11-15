using System;

namespace IndianRunner
{
    // Pure rules: deliberately independent of Unity, usable in Edit Mode checks.
    public static class RunRules
    {
        public const float LaneWidth = 2.65f;
        public const float LaneSpeed = 14f;
        public const float MaxSpeed = 18f;
        public const float RowSpacing = 26f;
        public const float JumpVelocity = 9f;
        public const float Gravity = 24f;
        // Cubic easing has peak derivative 1.5; this duration preserves LaneSpeed.
        public static float LaneTravelTime(float distance) => Math.Max(.09f, 1.5f * Math.Abs(distance) / LaneSpeed);
        public static float LaneProgress(float progress)
        {
            float t = Math.Max(0, Math.Min(1, progress));
            return t * t * (3 - 2 * t);
        }
        public static float Speed(float distance) => Math.Min(MaxSpeed, 9f + distance / 180f);
        public static int Score(float distance, int coins) => (int)Math.Max(0, distance) + coins * 10;
        public static float LaneX(int lane) => (Math.Max(0, Math.Min(2, lane)) - 1) * LaneWidth;
        public static int SafeLane(int row, int seed) => (int)(Mix((uint)row ^ (uint)seed) % 3);
        public static int Kind(int row, int lane, int seed) => 1 + (int)(Mix((uint)(row * 7 + lane + 71) ^ (uint)seed) % 3);
        public static bool IsBlocked(int row, int lane, int seed) => lane != SafeLane(row, seed);
        public static bool Overlaps(float aMin, float aMax, float bMin, float bMax) => aMin < bMax && aMax > bMin;
        private static uint Mix(uint x)
        {
            unchecked { x ^= x >> 16; x *= 0x7feb352d; x ^= x >> 15; x *= 0x846ca68b; return x ^ (x >> 16); }
        }
    }
}
