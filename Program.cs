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
                Console.WriteLine("Usage...");
                return;
            }

            TAPUtils tapUtils = new TAPUtils();
            tapUtils.ExportCodeBlocks(args[0]);
        }
    }
}
