using System;
using System.Diagnostics;
using System.IO;

namespace PreparePackaging
{
    static class Program
    {
        static void Main(string[] args)
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            var plainIn = new FileInfo(Path.Combine(dir.Parent.Parent.Parent.FullName, "EndOfTheLine", "license.txt"));
            var rtfOut = new FileInfo(Path.GetFileNameWithoutExtension(plainIn.Name) + ".rtf");

            if (args.Length > 0)
            {
                plainIn = new FileInfo(args[0]);
            }

            if (args.Length > 1)
            {
                rtfOut = new FileInfo(args[1]);
            }

            using (var rtfWriter = rtfOut.CreateText())
            {
                using (var plainReader = plainIn.OpenText())
                {
                    WriteToRtf(plainReader, rtfWriter);
                }
            }
            Console.WriteLine("Wrote " + rtfOut.FullName);
        }

        private static void WriteToRtf(TextReader plainReader, TextWriter rtfWriter)
        {
            rtfWriter.WriteLine("{\\rtf");
            rtfWriter.WriteLine("{\\fonttbl {\\f0 Times New Roman;}}");
            rtfWriter.Write("\\f0\\fs22");

            string prefix = " ";
            while (true)
            {
                var line = plainReader.ReadLine();

                if (line == null)
                {
                    break;
                }

                Debug.Assert(line.IndexOfAny(new[] { '{', '\\' }) == -1,
                    "Input that needs escaping not supported.");

                if (line == string.Empty)
                {
                    rtfWriter.WriteLine("\\par");
                    rtfWriter.WriteLine("\\par");
                    prefix = string.Empty;
                    continue;
                }

                rtfWriter.Write(prefix);
                rtfWriter.Write(line);
                prefix = " ";
            }
            rtfWriter.WriteLine();
            rtfWriter.WriteLine("}");
        }
    }
}
