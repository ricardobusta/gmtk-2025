using UnityEngine;

namespace Busta.LoopRacers
{
    public static class Constants
    {
        public const int MaxPlayers = 4;

        public static readonly Color[] PlayerColors =
        {
            new(200f / 255f, 55f / 255f, 55f / 255f),
            new(0f / 255f, 136f / 255f, 170f / 255f),
            new(113f / 255f, 200f / 255f, 55f / 255f),
            new(255f / 255f, 212f / 255f, 42f / 255f)
        };
        
        public static readonly Color[] PositionColors =
        {
            new(255f / 255f, 212f / 255f, 42f / 255f),
            new(200f / 255f, 200f / 255f, 200f / 255f),
            new(235f / 255f, 160f / 255f, 85f / 255f),
            new(100f / 255f, 100f / 255f, 100f / 255f)
        };
    }
}