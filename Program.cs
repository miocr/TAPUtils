using System;
using System.IO;
using System.Collections.Generic;

namespace ZXt2txt
{
    class Program
    {

        static void Main(string[] args)
        {

            if (false)
            {
                TAPUtils tapUtils = new TAPUtils();
                tapUtils.ExportCodeBlocks("CC-TXT5-test.tap");
            }

            if (true)
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), args[0]);
                string dirPath = Path.GetDirectoryName(fullPath);
                string fileName = Path.GetFileName(fullPath);
                List<string> inFiles = new List<string>(Directory.GetFiles(dirPath, fileName));
                if (inFiles.Count > 0)
                {
                    Converter converter = new Converter();
                    foreach (string inFile in inFiles)
                    {
                        string fileExtension = Path.GetExtension(inFile);
                        CodingType coding = CodingType.zxGraphics;
                        if (fileExtension == ".twt")
                            coding = CodingType.tasword2CZ;
                        else if (fileExtension == ".dtt")
                            coding = CodingType.dTextCZ;
                        else if (fileExtension == ".tbt")
                            coding = CodingType.tasword2BCS;

                        converter.Convert(inFile, inFile + ".txt", coding);
                    }
                }
            }

        }
    }
}
