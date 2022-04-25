using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Tesseract;

namespace QuestionGoogler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string previousQuestion = "";
            while (true)
            {
                var screenShot = GetSreenshot();
                var question = GetText(screenShot);
                question = ExtractText(question);
                if (question.Length > 20 && !IsSameQuestion(previousQuestion, question))
                {
                    Process.Start("http://google.com/search?q=" + question);
                    previousQuestion = question;
                }
            }
        }

        public static string GetText(Bitmap imgsource)
        {
            var ocrtext = string.Empty;
            using (var engine = new TesseractEngine(@"C:\work\daily\2022\q2\04_24\QuestionGoogler\packages\Tesseract.4.1.1", "eng", EngineMode.Default))
            {
                using (var img = PixConverter.ToPix(imgsource))
                {
                    using (var page = engine.Process(img))
                    {
                        ocrtext = page.GetText();
                    }
                }
            }

            return ocrtext;
        }

        public static bool IsSameQuestion(string previousQuestion, string currentQuestion)
        {
            int cost = Compute(previousQuestion, currentQuestion);
            return cost <= 20;
        }

        public static string ExtractText(string text)
        {
            foreach (var character in text)
            {
                if (character == '?')
                {
                    text.Append(character);
                    return text;
                }
                text.Append(character);
            }
            text = text.TrimStart('\r', '\n');
            text = text.TrimEnd('\r', '\n');
            return text;
        }

        private static Bitmap GetSreenshot()
        {
            Size size = new Size
            {
                Width = 800,
                Height = 100
            };
            Bitmap bm = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(bm);
            g.CopyFromScreen(50, 50, 0, 0, size);
            bm.Save($"images\\img.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return bm;
        }

        static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }
}
