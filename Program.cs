using System;
using System.Collections.Generic;

namespace StringSimilarity
{
    class Program
    {
        static void Main(string[] args)
        {
            List<(string, string)> li = new List<(string, string)>
            {
                ("hello world!", "HELLO WORLD!"),
                ("hello world!", "   hello world!"),
                ("hello world!", "привет мир!"),
                ("hello world!", "hello world!"),
                ("hello world!", "world hello!"),
                ("hello world!", "hllwrld"),
                ("hello world!", "h"),

            };
            foreach (var item in li)
                Check(item.Item1, item.Item2);
        }

        private static void Check(string src, string dst)
        {
            var s1 = Similar(src, dst);
            Console.WriteLine($"Similar(\"{src}\",\t\"{dst}\")\t= {s1:#0.0#####}");
        }

        /// <summary>
        /// Оценка похожести строк. 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static double Similar(string src, string dst)
        {
            if (string.IsNullOrEmpty(src) && string.IsNullOrEmpty(dst))
                return 1d;
            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(dst))
                return 0d;

            var srcLength = src.Length;
            var dstLength = dst.Length;            
            var matches = 0d;
            
            for (int srcI = 0, dstI = 0; srcI < srcLength && dstI < dstLength; srcI++, dstI++)
            {
                var s = Similar(src[srcI], dst[dstI]);
                if(s > 0)
                {
                    matches += s;
                    continue;
                }
                else
                {
                    int dstToSrcDelta = 1;
                    double dstToSrcSimilarity = 0d;
                    //поиск дальше по dst
                    for (; dstToSrcDelta + dstI < dstLength; dstToSrcDelta++)
                    {
                        dstToSrcSimilarity = Similar(src[srcI], dst[dstI + dstToSrcDelta]);
                        if (dstToSrcSimilarity > 0)
                            break;
                    }

                    int srcToDstDelta = 1;
                    double srcToDstSimilarity = 0d;
                    //поиск дальше по dst
                    for (; srcToDstDelta + srcI < srcLength && (srcToDstDelta <= dstToSrcDelta); srcToDstDelta++)
                    {
                        srcToDstSimilarity = Similar(src[srcI + srcToDstDelta], dst[dstI]);
                        if (srcToDstSimilarity > 0)
                            break;
                    }
                    // применение результата поиска
                    if(srcToDstSimilarity > 0 && dstToSrcSimilarity > 0)
                    {
                        if (srcToDstDelta < dstToSrcDelta)
                        {
                            matches += srcToDstSimilarity * .9d;
                            srcI = srcI + srcToDstDelta;
                        }

                        if (srcToDstDelta > dstToSrcDelta)
                        {
                            matches += dstToSrcSimilarity * .9d;
                            dstI = dstI + dstToSrcDelta;
                        }
                    }
                    else if (srcToDstSimilarity > 0 && dstToSrcSimilarity == 0)
                    {
                        matches += srcToDstSimilarity * .9d;
                        srcI = srcI + srcToDstDelta;
                    }
                    else if (srcToDstSimilarity == 0 && dstToSrcSimilarity > 0)
                    {
                        matches += dstToSrcSimilarity * .9d;
                        dstI = dstI + dstToSrcDelta;
                    }
                }
            }
            
            var grade = matches * 2d / (srcLength + dstLength);
            // Если не только проблемы с регистром, то учтем статистику появления символов, так как слова могут быть поменяны местами
            if (grade < .9d)
            {
                grade = (grade + SimilarByCharStat(src, dst)) / 2d;
            }

            return grade;
        }

        private static double SimilarByCharStat(string src, string dst)
        {
            var s = new Dictionary<char, int>();
            src = src.ToLower();
            dst = dst.ToLower();

            for (int i = 0; i < src.Length; i++)
                s[src[i]] = s.GetValueOrDefault(src[i], 0) + 1;

            for (int i = 0; i < dst.Length; i++)
                s[dst[i]] = s.GetValueOrDefault(dst[i], 0) - 1;

            s.Remove(' ');
            var err = 0;
            var errs = s.Values.GetEnumerator();
            while (errs.MoveNext())
                if (errs.Current > 0)
                    err += errs.Current;
                else if (errs.Current < 0)
                    err -= errs.Current;

            return 1d - 1d * err / (dst.Length + src.Length);
        }

        private static double Similar(char a, char b)
        {
            if (a == b)
                return 1d;

            if(Math.Abs(a - b) == 32)
                return 0.9d;
            
            return 0d;
        }
    }
}
