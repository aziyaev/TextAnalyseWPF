using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextAnalyseWPF.Models
{
    public class TextAnalyseModel
    {
        public double CompareTextsUsingLevenshtein(string textA, string textB, bool ignoreCase)
        {
            if (string.IsNullOrWhiteSpace(textA) && string.IsNullOrWhiteSpace(textB))
            {
                return 1;
            }
            if (string.IsNullOrWhiteSpace(textA) || string.IsNullOrWhiteSpace(textB))
            {
                return 0;
            }

            if (ignoreCase)
            {
                textA = textA.ToLower();
                textB = textB.ToLower();
            }

            textA = RemovePunctuation(textA);
            textB = RemovePunctuation(textB);

            return CalculateSimilarityOnLevenshtein(textA, textB);
        }

        public double CompareTextsUsingNGrams(string textA, string textB, bool ignoreCase)
        {
            if (string.IsNullOrWhiteSpace(textA) && string.IsNullOrWhiteSpace(textB))
            {
                return 1;
            }
            if (string.IsNullOrWhiteSpace(textA) || string.IsNullOrWhiteSpace(textB))
            {
                return 0;
            }

            if (ignoreCase)
            {
                textA = textA.ToLower();
                textB = textB.ToLower();
            }

            textA = RemovePunctuation(textA);
            textB = RemovePunctuation(textB);

            return CalculateSimilarityOnNgrams(textA, textB);
        }

        private double CalculateSimilarityOnLevenshtein(string textA, string textB)
        {
            int n = textA.Length;
            int m = textB.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;
            if (m == 0)
                return n;

            for(int i = 0; i <= n; d[i, 0] = i++) { }
            for(int j = 0; j <= m; d[0, j] = j++) { }

            for(int i = 1; i <= n; i++)
            {
                for(int j = 1; j<= m; j++)
                {
                    int cost = (textB[j - 1] == textA[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return 1.0 - ((double)d[n,m] / Math.Max(textA.Length, textB.Length));
        }

        private double CalculateSimilarityOnNgrams(string textA, string textB)
        {
            const int ngrams = 3;
            var ngramsA = GetNgrams(textA, ngrams);
            var ngramsB = GetNgrams(textB, ngrams);

            var intersection = ngramsA.Intersect(ngramsB).Count();
            var union = ngramsA.Union(ngramsB).Count();

            return (double)intersection / union;
        }

        private IEnumerable<string> GetNgrams(string text, int n)
        {
            for(int i = 0; i < text.Length - n + 1; i++)
            {
                yield return text.Substring(i, n);
            }
        }

        private string RemovePunctuation(string text)
        {
            text = text.Trim();
            return Regex.Replace(text, @"[^\w\s]", "");
        }
    }
}
