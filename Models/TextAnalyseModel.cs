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
            if (ValidateTextsIsBothEqualNullOrWhiteSpace(textA, textB)) return 1.0;
            if (ValidateTextsIsOneOfThemEqualNullOrWhiteSpace(textA, textB)) return 0;

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
            if (ValidateTextsIsBothEqualNullOrWhiteSpace(textA, textB)) return 1.0;
            if (ValidateTextsIsOneOfThemEqualNullOrWhiteSpace(textA, textB)) return 0;

            if (ignoreCase)
            {
                textA = textA.ToLower();
                textB = textB.ToLower();
            }

            textA = RemovePunctuation(textA);
            textB = RemovePunctuation(textB);

            return CalculateSimilarityOnNgrams(textA, textB);
        }

        public double CompareTextsUsingWordCount(string textA, string textB, bool ignoreCase)
        {
            if (ValidateTextsIsBothEqualNullOrWhiteSpace(textA, textB)) return 1.0;
            if (ValidateTextsIsOneOfThemEqualNullOrWhiteSpace(textA, textB)) return 0;

            if (ignoreCase)
            {
                textA = textA.ToLower();
                textB = textB.ToLower();
            }

            textA = RemovePunctuation(textA);
            textB = RemovePunctuation(textB);

            return CalculateSimilarityOnWordCount(textA, textB);
        }

        private double CalculateSimilarityOnWordCount(string textA, string textB)
        {
            if (ValidateTextsIsBothEqualNullOrWhiteSpace(textA, textB)) return 1.0;
            if (ValidateTextsIsOneOfThemEqualNullOrWhiteSpace(textA, textB)) return 0;

            var wordsA = textA.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var wordsB = textB.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var intersectionCount = wordsA.Intersect(wordsB).Count();
            var unionCount = wordsA.Union(wordsB).Count();
            return (double)intersectionCount / unionCount;
        }

        private double CalculateSimilarityOnLevenshtein(string textA, string textB)
        {
            if (ValidateTextsIsBothEqualNullOrWhiteSpace(textA, textB)) return 1.0;
            if (ValidateTextsIsOneOfThemEqualNullOrWhiteSpace(textA, textB)) return 0;

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
            if (ValidateTextsIsBothEqualNullOrWhiteSpace(textA, textB)) return 1.0;
            if (ValidateTextsIsOneOfThemEqualNullOrWhiteSpace(textA, textB)) return 0;

            const int minLength = 50;
            const int maxLength = 500;

            int lenghtA = textA.Length;
            int lenghtB = textB.Length;

            int nA = GetNgramSize(lenghtA, minLength, maxLength);
            int nB = GetNgramSize(lenghtB, minLength, maxLength);

            int ngrams = Math.Min(nA, nB);

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

        private int GetNgramSize(int lenght, int minLength, int maxLength)
        {
            if(lenght < minLength)
            {
                return 2;
            }
            else if(lenght > maxLength)
            {
                return 3;
            }
            else
            {
                return 2 + (int)Math.Floor((lenght - minLength) / (double)(maxLength - minLength) * 3);
            }
        }

        private string RemovePunctuation(string text)
        {
            text = text.Trim();
            return Regex.Replace(text, @"[^\w\s]", "");
        }

        private bool ValidateTextsIsBothEqualNullOrWhiteSpace(string textA, string textB)
        {
            if (string.IsNullOrWhiteSpace(textA) && string.IsNullOrWhiteSpace(textB))
            {
                return true;
            }

            return false;
        }

        private bool ValidateTextsIsOneOfThemEqualNullOrWhiteSpace(string textA, string textB)
        {
            if (string.IsNullOrWhiteSpace(textA) || string.IsNullOrWhiteSpace(textB))
            {
                return true;
            }

            return false;
        }
    }
}
