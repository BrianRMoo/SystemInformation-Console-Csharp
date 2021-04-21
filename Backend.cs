using System;

namespace Brian_Moorman_TextFormatter
{
    class Backend
    {
        public void SetTextColor(ConsoleColor textColor)
        {
            //New Colors
            Console.ForegroundColor = textColor;
        }

        public void DefaultTextColor()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string StringFormatLeft(string str, int alignWidth)
        {
            //Example: Console.WriteLine(string.Format("|{0}|", centeredString("Desired Output", 25)));
            if (str.Length >= alignWidth)
            {
                return str;
            }

            int stringPaddingLeft = (alignWidth - str.Length) / 2;
            int stringPaddingRight = alignWidth - str.Length - stringPaddingLeft;

            return new string(' ', stringPaddingLeft) + str + new string(' ', stringPaddingRight);
        }
        public string StringFormatRight(string str, int alignWidth)
        {
            //Example: Console.WriteLine(string.Format("|{0}|", centeredString("Desired Output", 25)));
            if (str.Length >= alignWidth)
            {
                return str;
            }

            int stringPaddingRight = (alignWidth - str.Length) / 2;
            int stringPaddingLeft = alignWidth - str.Length - stringPaddingRight;

            return new string(' ', stringPaddingLeft) + str + new string(' ', stringPaddingRight);
        }
        public void TestPaddingFunction()
        {
            
        }


    }

}
