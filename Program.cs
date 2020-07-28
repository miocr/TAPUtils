using System;
using System.IO;
using System.Collections.Generic;

namespace TAPUtils
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length < 1 || !File.Exists(args[0]))
            {
                Console.WriteLine("");
                Console.WriteLine("Extract codeblocks from TAP file, specialy blocks with Tasword or D-TEXT(SpectralWriter). CIDSOFT (C)2017, version 1.0");
                Console.WriteLine("");
                Console.WriteLine("Usage: dotnet run file");
                Console.WriteLine("");
                if (!File.Exists(args[0]))
                {
                    Console.WriteLine($"File not found ({args[0]}).");
                }
                return;
            }

            TAPUtils tapUtils = new TAPUtils();
            tapUtils.ExportCodeBlocks(args[0]);
        }
    }
}
