using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace ZXt2txt
{
    public enum CodingType
    {
        zxGraphics = 0,
        tasword2CZ = 1,
        dTextCZ = 2,
        tasword2BCS = 3
    }

    class Converter
    {
        private Dictionary<int, char> tasword2CZ = new Dictionary<int, char>
        {
          {0x80,'é'},
          {0x81,'ě'},
          {0x82,'š'},
          {0x83,'č'},
          {0x84,'ř'},
          {0x85,'ý'},
          {0x86,'á'},
          {0x87,'í'},
          {0x88,'ů'},
          {0x89,'ú'},
          {0x8A,'ó'},
          {0x8B,'ď'},
          {0x8C,'.'}, //char replacement for semigraphics
          {0x8D,'ň'},
          {0x8E,'L'}, // char replacement for semigraphics
          {0x8F,'ž'},
          {0x90,'A'},
          {0x91,'B'},
          {0x92,'C'},
          {0x93,'D'},
          {0x94,'E'},
          {0x95,'F'},
          {0x96,'G'},
          {0x97,'H'},
          {0x98,'I'},
          {0x99,'J'},
          {0x9A,'K'},
          {0x9B,'L'},
          {0x9C,'M'},
          {0x9D,'N'},
          {0x9E,'O'},
          {0x9F,'P'}
        };
        private Dictionary<int, char> tasword2BCS = new Dictionary<int, char>
        {
          {0xA0,' '},
          {0xA1,' '},
          {0xA2,' '},
          {0xA3,' '},
          {0xA4,' '},
          {0xA5,'ľ'},
          {0xA6,'š'},
          {0xA7,'č'},
          {0xA8,'ř'},
          {0xA9,'ž'},
          {0xAA,'ý'},
          {0xAB,'á'},
          {0xAC,'í'},
          {0xAD,'é'},
          {0xAE,'Ľ'},
          {0xAF,'Š'},
          {0xB0,'Č'},
          {0xB1,'Ř'},
          {0xB2,'Ž'},
          {0xB3,'Ý'},
          {0xB4,'Á'},
          {0xB5,'Í'},
          {0xB6,'É'},
          {0xB7,'ä'},
          {0xB8,'°'},
          {0xB9,'ď'},
          {0xBA,'ĺ'},
          {0xBB,'ó'},
          {0xBC,'ô'},
          {0xBD,'ť'},
          {0xBE,'ů'},
          {0xBF,'ú'},
          {0xC0,'Ä'},
          {0xC1,'O'},
          {0xC2,'Ď'},
          {0xC3,'Ľ'},
          {0xC4,'Ó'},
          {0xC5,'Ô'},
          {0xC6,'Ť'},
          {0xC7,'U'},
          {0xC8,'Ú'},
          {0xC9,'ň'},
          {0xCA,'ŕ'},
          {0xCB,'ě'},
          {0xCC,'Ň'},
          {0xCD,'Ŕ'},
          {0xCE,'É'},
          {0xCF,' '},
          {0xD0,' '},
          {0xD1,' '},
          {0xD2,' '},
          {0xD3,' '},
          {0xD4,' '},
          {0xD5,' '},
          {0xD6,' '},
          {0xD7,' '},
          {0xD8,' '},
          {0xD9,' '},
          {0xDA,' '},
          {0xDB,' '},
          {0xDC,' '},
          {0xDD,' '},
          {0xDE,' '},
          {0xDF,' '},
          {0xE0,' '},
          {0xE1,' '},
          {0xE2,' '},
          {0xE3,' '},
          {0xE4,' '},
          {0xE5,' '},
          {0xE6,' '},
          {0xE7,' '},
          {0xE8,' '},
          {0xE9,' '},
          {0xEA,' '},
          {0xEB,' '},
          {0xEC,' '},
          {0xED,' '},
          {0xEE,' '},
          {0xEF,' '},
          {0xF0,' '},
          {0xF1,' '},
          {0xF2,' '},
          {0xF3,' '},
          {0xF4,' '},
          {0xF5,' '},
          {0xF6,' '},
          {0xF7,' '},
          {0xF8,' '},
          {0xF9,' '},
          {0xFA,' '},
          {0xFB,' '},
          {0xFC,' '},
          {0xFD,' '},
          {0xFE,' '},
          {0xFF,' '}
        };
        private Dictionary<int, char> dTextCZ = new Dictionary<int, char>
        {
          {0x80,'Á'},
          {0x81,'Č'},
          {0x82,'Ď'},
          {0x83,'É'},
          {0x84,'Ě'},
          {0x85,'Í'},
          {0x86,'Ň'},
          {0x87,'Ó'},
          {0x88,'Ř'},
          {0x89,'Š'},
          {0x8A,'Ť'},
          {0x8B,'Ú'},
          {0x8C,'U'},
          {0x8D,'Ý'},
          {0x8E,'Ž'},
          {0x8F,' '},

          {0x90,'á'},
          {0x91,'č'},
          {0x92,'ď'},
          {0x93,'é'},
          {0x94,'ě'},
          {0x95,'í'},
          {0x96,'ň'},
          {0x97,'ó'},
          {0x98,'ř'},
          {0x99,'š'},
          {0x9A,'ť'},
          {0x9B,'ú'},
          {0x9C,'ů'},
          {0x9D,'ý'},
          {0x9E,'ž'},
          {0x9F,' '}
        };
        private Dictionary<int, char> zxGraphics = new Dictionary<int, char>
        {
          // char replacement for semigraphics
          {0x80,' '},
          {0x81,'•'},
          {0x82,'•'},
          {0x83,'—'},
          {0x84,'•'},
          {0x85,'|'},
          {0x86,'\\'},
          {0x87,'+'},
          {0x88,'•'},
          {0x89,'/'},
          {0x8A,'|'},
          {0x8B,'+'},
          {0x8C,'_'},
          {0x8D,'+'},
          {0x8E,'+'},
          {0x8F,'•'},
          {0x90,'A'},
          {0x91,'B'},
          {0x92,'C'},
          {0x93,'D'},
          {0x94,'E'},
          {0x95,'F'},
          {0x96,'G'},
          {0x97,'H'},
          {0x98,'I'},
          {0x99,'J'},
          {0x9A,'K'},
          {0x9B,'L'},
          {0x9C,'M'},
          {0x9D,'N'},
          {0x9E,'O'},
          {0x9F,'P'}
        };
        public Dictionary<int, string> zxTokens = new Dictionary<int, string>
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
        private Dictionary<int, char> codingDict;
        private StringBuilder sbTextLine = new StringBuilder();
        private FileStream input;
        private FileStream output;
        private Encoding enc = Encoding.GetEncoding("UTF-8");
        private byte[] charsEncoded;
        public Converter()
        {
        }

        public void Convert(string inputFilePath, string outputFilePath, CodingType coding)
        {
            // in/out files
            input = File.OpenRead(inputFilePath);
            output = File.OpenWrite(outputFilePath);

            // choose dictionary for recoding chars 0x80-0x9F
            switch (coding)
            {
                case CodingType.zxGraphics:
                    codingDict = zxGraphics;
                    break;
                case CodingType.tasword2CZ:
                    codingDict = tasword2CZ;
                    break;
                case CodingType.dTextCZ:
                    codingDict = dTextCZ;
                    break;
                case CodingType.tasword2BCS:
                    codingDict = tasword2BCS;
                    break;
            }

            int repeat = 0;
            int previousChar = 32;
            int currentChar = 32;

            while ((currentChar = input.ReadByte()) != -1)
            {
                // CodingType.tasword2BCS hasn't compression, but uses valus > 0XA0 for natioanal chars
                if (currentChar >= 0xA5 && coding != CodingType.tasword2BCS)
                {
                    // lastchar was packed > count total repetition of lastchar for unpack 
                    repeat = repeat + (currentChar - 0xA5 + 1);
                }
                else
                {
                    // if repeat > 0, lastchar isn packed
                    while (repeat > 0)
                    {
                        // unpack last character (repeat x)
                        WriteChar(previousChar);
                        repeat--;
                    }
                    // now repeat = 0;

                    // write current char 
                    WriteChar(currentChar);

                    // remember current char as previous for possible unpack 
                    previousChar = currentChar;
                }

            }

            // write last line, if exists (is shorten than 64 chars)
            if (sbTextLine.Length > 0)
            {
                WriteLine();
            }
        }

        private void WriteChar(int charInt)
        {
            if (charInt > 0x7F)// && charInt < 0xA0)
            {
                sbTextLine.Append(codingDict[charInt]);
            }
            else
                sbTextLine.Append(System.Convert.ToChar(charInt));

            if (sbTextLine.Length == 64)
            {
                sbTextLine.Append("\n");
                WriteLine();
            }
        }

        private void WriteLine()
        {
            Console.Write(sbTextLine.ToString());
            charsEncoded = enc.GetBytes(sbTextLine.ToString());
            output.Write(charsEncoded, 0, charsEncoded.Length);
            output.Flush();
            sbTextLine.Clear();
        }

    }
}