using System.Diagnostics;
using System.Drawing;
using System.Threading;
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
                var question = GetText(GetScreenshot());
                question = ValidateQuestion(question);
                while (question.Length > 10 && question != previousQuestion)
                {
                    Process.Start("http://google.com/search?q=" + question);
                    previousQuestion = question;
                    Thread.Sleep(2000);
                }
            }
        }

        private static Bitmap GetScreenshot()
        {
            Bitmap bm = new Bitmap(644, 68);
            Graphics g = Graphics.FromImage(bm);

            // Repeating the CopyFromScreen method as the first try sometimes fails
            try
            {
                g.CopyFromScreen(159, 70, 0, 0, bm.Size);
            }
            catch
            {
                g.CopyFromScreen(159, 70, 0, 0, bm.Size);
            }

            return bm;
        }


        public static string GetText(Bitmap imgsource)
        {
            var ocrtext = string.Empty;
            using (var engine = new TesseractEngine(@"C:\work\daily\2022\q2\04_25\QuestionGoogler\packages\Tesseract.4.1.1", "eng", EngineMode.Default))
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

        public static string ValidateQuestion(string text)
        {
            text = text.Trim();
            var indexOfQuestionMark = text.LastIndexOf("?");
            return indexOfQuestionMark >= 0 ? text.Substring(0, indexOfQuestionMark + 1) : string.Empty;
        }
    }
}
