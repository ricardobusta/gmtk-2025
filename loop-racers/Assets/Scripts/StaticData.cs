using System.Collections.Generic;

namespace Busta.LoopRacers
{
    public static class StaticData
    {
        private static Dictionary<string, int> _data = new();

        public static int GetValue(string key, int defaultValue)
        {
            return _data.GetValueOrDefault(key, defaultValue);
        }

        public static void SetValue(string key, int value)
        {
            _data[key] = value;
        }

        public static void DeleteValue(string key)
        {
            _data.Remove(key);
        }
    }
}