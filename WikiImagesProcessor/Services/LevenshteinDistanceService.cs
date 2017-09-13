using System;
using WikiImagesProcessor.Abstractions.Services;

namespace WikiImagesProcessor.Services
{
    class LevenshteinDistanceService : IDistanceService
    {
        public int GetDistance(string s1, string s2)
        {
            if (string.Equals(s1, s2))
                return 0;

            if (String.IsNullOrEmpty(s1) || String.IsNullOrEmpty(s2))
                return (s1 ?? String.Empty).Length + (s2 ?? String.Empty).Length;

            if (s1.Length > s2.Length)
            {
                var tmp = s1;
                s1 = s2;
                s2 = tmp;
            }

            if (s2.Contains(s1))
                return s2.Length - s1.Length;

            int[,] distance = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; distance[i, 0] = i++) ;

            for (int j = 0; j <= s2.Length; distance[0, j] = j++) ;

            for (int i = 1; i <= s1.Length; i++)
            for (int j = 1; j <= s2.Length; j++)
            {
                int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }

            return distance[s1.Length, s2.Length];
        }
    }
}
