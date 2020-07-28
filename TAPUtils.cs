using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TAPUtils
{

    class TAPUtils
    {
        private Dictionary<int, string> zxTokens = new Dictionary<int, string>
        {
          {0xA0,"(Q)"},
          {0xA1,"(R)"},
          {0xA2,"(S)"},
          {0xA3,"(T)"},
          {0xA4,"(U)"},
          {0xA5,"RND"},
          {0xA6,"INKEY$"},
          {0xA7,"PI"},
          {0xA8,"FN"},
          {0xA9,"POINT"},
          {0xAA,"SCREEN$"},
          {0xAB,"ATTR"},
          {0xAC,"AT"},
          {0xAD,"TAB"},
          {0xAE,"VAL$"},
          {0xAF,"CODE"},
          {0xB0,"VAL"},
          {0xB1,"LEN"},
          {0xB2,"SIN"},
          {0xB3,"COS"},
          {0xB4,"TAN"},
          {0xB5,"ASN"},
          {0xB6,"ACS"},
          {0xB7,"ATN"},
          {0xB8,"LN"},
          {0xB9,"EXP"},
          {0xBA,"INT"},
          {0xBB,"SQR"},
          {0xBC,"SGN"},
          {0xBD,"ABS"},
          {0xBE,"PEEK"},
          {0xBF,"IN"},
          {0xC0,"USR"},
          {0xC1,"STR$"},
          {0xC2,"CHR$"},
          {0xC3,"NOT"},
          {0xC4,"BIN"},
          {0xC5,"OR"},
          {0xC6,"AND"},
          {0xC7,"<="},
          {0xC8,">="},
          {0xC9,"<>"},
          {0xCA,"LINE"},
          {0xCB,"THEN"},
          {0xCC,"TO"},
          {0xCD,"STEP"},
          {0xCE,"DEF FN"},
          {0xCF,"CAT"},
          {0xD0,"FORMAT"},
          {0xD1,"MOVE"},
          {0xD2,"ERASE"},
          {0xD3,"OPEN #"},
          {0xD4,"CLOSE #"},
          {0xD5,"MERGE"},
          {0xD6,"VERIFY"},
          {0xD7,"BEEP"},
          {0xD8,"CIRCLE"},
          {0xD9,"INK"},
          {0xDA,"PAPER"},
          {0xDB,"FLASH"},
          {0xDC,"BRIGHT"},
          {0xDD,"INVERSE"},
          {0xDE,"OVER"},
          {0xDF,"OUT"},
          {0xE0,"LPRINT"},
          {0xE1,"LLIST"},
          {0xE2,"STOP"},
          {0xE3,"READ"},
          {0xE4,"DATA"},
          {0xE5,"RESTORE"},
          {0xE6,"NEW"},
          {0xE7,"BORDER"},
          {0xE8,"CONTINUE"},
          {0xE9,"DIM"},
          {0xEA,"REM"},
          {0xEB,"FOR"},
          {0xEC,"GO TO"},
          {0xED,"GO SUB"},
          {0xEE,"INPUT"},
          {0xEF,"LOAD"},
          {0xF0,"LIST"},
          {0xF1,"LET"},
          {0xF2,"PAUSE"},
          {0xF3,"NEXT"},
          {0xF4,"POKE"},
          {0xF5,"PRINT"},
          {0xF6,"PLOT"},
          {0xF7,"RUN"},
          {0xF8,"SAVE"},
          {0xF9,"RANDOMIZE"},
          {0xFA,"IF"},
          {0xFB,"CLS"},
          {0xFC,"DRAW"},
          {0xFD,"CLEAR"},
          {0xFE,"RETURN"},
          {0xFF,"COPY"}
        };

        private FileStream input;
        private FileStream output;
        int lhFfilesCounter;
        byte[] tapWord;
        byte[] tapFileName;
        byte[] tapHeaderBlock;
        byte[] tapDataBlock;
        byte tapBlockFlag;
        bool fileNameFromHeader;
        byte tapHeaderBlockDataType;
        int tapHeaderCodeLength;
        int tapHeaderCodeStart;
        int tapHeaderExtWord;
        int tapBlockLength;
        int tapCheckSum;

        int ret;

        public TAPUtils()
        {
            fileNameFromHeader = false;
            lhFfilesCounter = 0;
            tapWord = new byte[2];
            tapFileName = new byte[10];
            tapHeaderBlock = new byte[19];
        }

        public void ExportCodeBlocks(string tapFileNamePath)
        {
            input = File.OpenRead(tapFileNamePath);

            while ((ret = input.Read(tapWord, 0, 2)) > 0)
            {
                tapBlockLength = BitConverter.ToUInt16(tapWord, 0);
                ReadTapBlock(tapBlockLength);
                if (tapBlockFlag > 0)
                {
                    // no header block type
                    bool saveResult = SaveDataBlock();
                }
            }
        }

        private void ReadTapBlock(int tapBlockLength)
        {
            // First byte from data block = flag
            tapBlockFlag = (byte)input.ReadByte();
            if (tapBlockFlag == 0 && tapBlockLength == 19)
            {
                // header 
                tapHeaderBlock[0] = tapBlockFlag;
                // flag for next data block, that have header
                fileNameFromHeader = true;

                // read rest header values (without tapDataBlockFlag )
                input.Read(tapHeaderBlock, 1, tapBlockLength - 1);
                tapHeaderBlockDataType = tapHeaderBlock[1]; 
                tapHeaderCodeLength = BitConverter.ToUInt16(tapHeaderBlock, 12);
                tapHeaderCodeStart = BitConverter.ToUInt16(tapHeaderBlock, 14);
                tapHeaderExtWord = BitConverter.ToUInt16(tapHeaderBlock, 16);

                //FileHeader header = new FileHeader(tapHeaderBlock);
            }
            else if (fileNameFromHeader && tapBlockFlag == 255
                    && ((tapBlockLength - 2) == tapHeaderCodeLength))
            {
                // flag of header data block (data for last header)
                tapDataBlock = new byte[tapBlockLength - 2];
                input.Read(tapDataBlock, 0, tapBlockLength - 2);
                tapCheckSum = input.ReadByte();
            }
            else
            {
                // unsuported blocktype, only read for seek 
                // block length - 2 (dw = length) + 1 (byte = checksum)
                tapDataBlock = new byte[tapBlockLength - 2 + 1];
                input.Read(tapDataBlock, 0, tapBlockLength - 2 + 1);
            }
        }

        private bool SaveDataBlock()
        {
            // ZX file name conversion (codes > 0x7F)
            Array.Copy(tapHeaderBlock, 2, tapFileName, 0, 10);

            // create usable file name from zx header name 
            StringBuilder sbFileName = new StringBuilder();
            if (fileNameFromHeader)
            {
                // prepare output filename from last header block
                for (int i = 0; i < 10; i++)
                {
                    if (tapFileName[i] > 0x7F || (tapFileName[i] < 0x20))
                    {
                        // replace codes in header name as ZX BASIC TOKENS
                        sbFileName.Append(GetZXTokenString(tapFileName[i]));
                    }
                    else
                    {
                        sbFileName.Append(Encoding.UTF8.GetString(tapFileName, i, 1));
                    }
                }
            }

            bool textFileCheck = CheckTextFileContent();
            string fileExtension = String.Empty;
            if (fileNameFromHeader && 
                textFileCheck && tapHeaderCodeStart == 32000)
            {
                // Tasword2CZ or Tasword2BSC (can't be distinguished)
                fileExtension = ".tw2cz";
            }
            else if (fileNameFromHeader && 
                    textFileCheck && tapHeaderCodeStart == 32768)
            {
                // D-textCZ (SpectralWriter)
                fileExtension = ".dtcz";
            }
            else if (fileNameFromHeader && tapHeaderBlockDataType == 3 &&
                    tapHeaderCodeStart == 16384 && tapHeaderCodeLength == 6912)
            {
                // compatibility extension with ZX Paintbrush
                // http://www.zx-modules.de
                fileExtension = ".scr";
            }
            else if (fileNameFromHeader)
            {   
                // other code block with header (not recognized type)
                // extension from according to tapHeaderBlockDataType
                if (tapHeaderBlockDataType == 3)
                {
                     fileExtension = ".code";   // code (or screen)
                }
                else if (tapHeaderBlockDataType == 2)
                {
                    if (tapHeaderCodeStart == 0xC620 && tapHeaderExtWord == 0x0D20)
                        fileExtension = ".mf09"; // MasterFile V0.9 (original)
                    else if (tapHeaderCodeStart == 0xC600 && tapHeaderExtWord == 0x0D00)
                        fileExtension = ".mf104"; // MasterFile V1.04 (CZmod)
                    else
                        fileExtension = ".aarray"; // alphanumeric data array
                }
                else if (tapHeaderBlockDataType == 1)
                {
                     fileExtension = ".narray"; // numeric data array                     
                }
                else if (tapHeaderBlockDataType == 0)
                {
                     fileExtension = ".basic"; // basic block 
                }
                else 
                     fileExtension = ".unknown";
            }
            else
            {
                // code block without header, file name is generated
                sbFileName.Append("LessHeaderDataBlock");
                sbFileName.Append((lhFfilesCounter++).ToString().PadLeft(4,'0'));
                fileExtension = ".unknown";
            }

            sbFileName.Append(fileExtension);
            string fileName = sbFileName.ToString();
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            output = File.OpenWrite(Path.Combine("files", fileName));
            output.Write(tapDataBlock, 0, tapDataBlock.Length);

            // filename from last header is used
            fileNameFromHeader = false;

            output.Flush();
            output.Dispose();

            Console.WriteLine("Saved: " + fileName);

            return true;
        }
        private bool CheckTextFileContent()
        {
            bool result;
            float vowelCount = 0;
            float vowelRatio;

            string vowels = "aeiou";
            for (int i = 0; i < tapBlockLength - 2; i++)
            {
                if (vowels.IndexOf((char)tapDataBlock[i]) > 0)
                    vowelCount++;
            }
            // if (vowelCount == 0)
            //     return false;

            vowelRatio = vowelCount / ((tapDataBlock.Length - 2) );
            // more vowels then 10% = sign as text data block
            result = (vowelRatio * 100 > 10); 

            return result;
        }

        private string GetZXTokenString(int code)
        {
            if (zxTokens.ContainsKey(code))
                return zxTokens[code];
            else
                return string.Empty;
        }

    }

    /*
    class FileHeader
    {
        byte flag;
        byte dataType;
        //byte[] fileName = new byte[10];
        string fileName;
        UInt16 dataLength;
        UInt16 dataExtens;
        //various for header type (for CODE = start)
        UInt16 unused;
        byte checksum;

        UInt16 DataExtens
        {
            get { return dataExtens; }
            set { dataExtens = value; }
        }

        public FileHeader()
        { }

        public FileHeader(byte[] headBytes)
        {
            flag = headBytes[0];
            dataType = headBytes[1];
            //Array.Copy(headBytes,2,fileName,0,10);

            StringBuilder sb = new StringBuilder();
            TAPUtils utils = new TAPUtils();

            for (int i = 2; i < 12; i++)
            {
                if (headBytes[i] >= 0xA0)
                {
                    sb.Append(utils.GetZXTokenString(headBytes[i]));
                }
                else
                {
                    sb.Append(Encoding.UTF8.GetString(headBytes, i, 1));
                }
            }
            fileName = sb.ToString();

            //fileName = Encoding.UTF8.GetString(headBytes,2,10);
            dataLength = BitConverter.ToUInt16(headBytes, 12);
            dataExtens = BitConverter.ToUInt16(headBytes, 14);
            unused = BitConverter.ToUInt16(headBytes, 16);
            checksum = headBytes[18];
        }

    }
     */

}