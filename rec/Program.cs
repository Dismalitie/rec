using ScreenRecorderLib;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace rec
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: <filepath>");
                Console.WriteLine("filepath: Where the file that the recorded snapshot will be written to as an MP4");
                return;
            }

            Recorder _rec; // init with def cfg
            

            string videoPath = Path.Combine(args[0]);
            _rec = Recorder.CreateRecorder();

            _rec.Record(videoPath);
            string oldtitle = Console.Title; // save the title so we can revert it back
            Console.Title = "rec - " + Path.GetFileName(videoPath); // rec status

            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            var clearingTask = clr(ct);

            Console.ReadKey();
            Console.Write("•"); // does the notif sound cos special char

            cts.Cancel();
            await clearingTask;

            _rec.Stop();
            Console.WriteLine();
            Console.WriteLine("Recording saved at: \"" + Path.GetFullPath(videoPath) + "\"");
            Console.Title = oldtitle; // revert title
        }

        static async Task clr(CancellationToken cancellationToken)
        {
            int elapsedSecs = 0;
            int elapsedSecsTens = 0;
            int elapsedMins = 0;
            int elapsedMinsTens = 0;
            bool redtick = true; // cool thing

            while (!cancellationToken.IsCancellationRequested)
            {
                Console.Clear();
                if (redtick)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[rec] ");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write("[rec] ");
                }

                Console.WriteLine("Press any key to stop recording.");
                Console.WriteLine(elapsedMinsTens.ToString() + elapsedMins.ToString() + ":" + elapsedSecsTens.ToString() + elapsedSecs.ToString());

                await Task.Delay(1000);

                elapsedSecs++;
                redtick = !redtick;

                if (elapsedSecs == 10)
                {
                    elapsedSecs = 0;
                    elapsedSecsTens++;
                }
                if (elapsedSecsTens == 7)
                {
                    elapsedSecsTens = 0;
                    elapsedMins++;
                }
                if (elapsedMins == 10)
                {
                    elapsedMins = 0;
                    elapsedMinsTens++;
                }
            }
        }
    }
}
