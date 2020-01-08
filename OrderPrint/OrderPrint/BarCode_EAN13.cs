using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace OrderPrint
{
    class BarCode_EAN13
    {
        #region 生成条码　　ＥＡＮ１３码

        private static string getEAN13(string s, int width, int height)
        {

            int checkcode_input = -1;//输入的校验码

            if (!Regex.IsMatch(s, @"^\d{12}$"))
            {

                if (!Regex.IsMatch(s, @"^\d{13}$"))
                {

                    return "存在不允许的字符！";

                }

                else
                {

                    checkcode_input = int.Parse(s[12].ToString());

                    s = s.Substring(0, 12);

                }

            }



            int sum_even = 0;//偶数位之和

            int sum_odd = 0;//奇数位之和



            for (int i = 0; i < 12; i++)
            {

                if (i % 2 == 0)
                {

                    sum_odd += int.Parse(s[i].ToString());

                }

                else
                {

                    sum_even += int.Parse(s[i].ToString());

                }

            }



            int checkcode = (10 - (sum_even * 3 + sum_odd) % 10) % 10;//校验码



            if (checkcode_input > 0 && checkcode_input != checkcode)
            {

                return "输入的校验码错误！";

            }



            s += checkcode;//变成13位



            // 000000000101左侧42个01010右侧35个校验7个101000000000

            // 6        101左侧6位 01010右侧5位 校验1位101000000000



            string result_bin = "";//二进制串

            result_bin += "000000000101";



            string type = ean13type(s[0]);

            for (int i = 1; i < 7; i++)
            {

                result_bin += ean13(s[i], type[i - 1]);

            }

            result_bin += "01010";

            for (int i = 7; i < 13; i++)
            {

                result_bin += ean13(s[i], 'C');

            }

            result_bin += "101000000000";



            string result_html = "";//HTML代码

            string color = "";//颜色

            int height_bottom = width * 5;

            foreach (char c in result_bin)
            {

                color = c == '0' ? "#FFFFFF" : "#000000";

                result_html += "<div style=\"width:" + width + "px;height:" + height + "px;float:left;background:" + color + ";\"></div>";

            }

            result_html += "<div style=\"clear:both\"></div>";



            result_html += "<div style=\"float:left;color:#000000;width:" + (width * 9) + "px;text-align:center;\">" + s[0] + "</div>";

            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";

            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#FFFFFF;\"></div>";

            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";

            for (int i = 1; i < 7; i++)
            {

                result_html += "<div style=\"float:left;width:" + (width * 7) + "px;color:#000000;text-align:center;\">" + s[i] + "</div>";

            }

            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#FFFFFF;\"></div>";

            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";

            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#FFFFFF;\"></div>";

            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";

            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#FFFFFF;\"></div>";

            for (int i = 7; i < 13; i++)
            {

                result_html += "<div style=\"float:left;width:" + (width * 7) + "px;color:#000000;text-align:center;\">" + s[i] + "</div>";

            }

            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";

            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#FFFFFF;\"></div>";

            result_html += "<div style=\"float:left;width:" + width + "px;height:" + height_bottom + "px;background:#000000;\"></div>";

            result_html += "<div style=\"float:left;color:#000000;width:" + (width * 9) + "px;\"></div>";

            result_html += "<div style=\"clear:both\"></div>";



            return "<div style=\"background:#FFFFFF;padding:0px;font-size:" + (width * 10) + "px;font-family:'楷体';\">" + result_html + "</div>";

        }



        private static string ean13(char c, char type)
        {

            switch (type)
            {

                case 'A':
                    {

                        switch (c)
                        {

                            case '0': return "0001101";

                            case '1': return "0011001";

                            case '2': return "0010011";

                            case '3': return "0111101";//011101

                            case '4': return "0100011";

                            case '5': return "0110001";

                            case '6': return "0101111";

                            case '7': return "0111011";

                            case '8': return "0110111";

                            case '9': return "0001011";

                            default: return "Error!";

                        }

                    }

                case 'B':
                    {

                        switch (c)
                        {

                            case '0': return "0100111";

                            case '1': return "0110011";

                            case '2': return "0011011";

                            case '3': return "0100001";

                            case '4': return "0011101";

                            case '5': return "0111001";

                            case '6': return "0000101";//000101

                            case '7': return "0010001";

                            case '8': return "0001001";

                            case '9': return "0010111";

                            default: return "Error!";

                        }

                    }

                case 'C':
                    {

                        switch (c)
                        {

                            case '0': return "1110010";

                            case '1': return "1100110";

                            case '2': return "1101100";

                            case '3': return "1000010";

                            case '4': return "1011100";

                            case '5': return "1001110";

                            case '6': return "1010000";

                            case '7': return "1000100";

                            case '8': return "1001000";

                            case '9': return "1110100";

                            default: return "Error!";

                        }

                    }

                default: return "Error!";

            }

        }

        private static string ean13type(char c)
        {

            switch (c)
            {

                case '0': return "AAAAAA";

                case '1': return "AABABB";

                case '2': return "AABBAB";

                case '3': return "AABBBA";

                case '4': return "ABAABB";

                case '5': return "ABBAAB";

                case '6': return "ABBBAA";//中国

                case '7': return "ABABAB";

                case '8': return "ABABBA";

                case '9': return "ABBABA";

                default: return "Error!";

            }

        }
        #endregion

        public static void Paint_EAN13(string ean13, Graphics g, Rectangle drawBounds)
        {
            string barCode = ean13.Substring(0, 12);

            int checkcode_input = -1;//输入的校验码

            if (!Regex.IsMatch(barCode, @"^\d{12}$"))
            {
                if (!Regex.IsMatch(barCode, @"^\d{13}$"))
                {
                    return;
                }
                else
                {
                    checkcode_input = int.Parse(barCode[12].ToString());
                    barCode = barCode.Substring(0, 12);
                }
            }

            int sum_even = 0;//偶数位之和
            int sum_odd = 0;//奇数位之和

            for (int i = 0; i < 12; i++)
            {
                if (i % 2 == 0)
                {
                    sum_odd += int.Parse(barCode[i].ToString());
                }
                else
                {
                    sum_even += int.Parse(barCode[i].ToString());
                }
            }

            int checkcode = (10 - (sum_even * 3 + sum_odd) % 10) % 10;//校验码
            if (checkcode_input > 0 && checkcode_input != checkcode)
            {
                return;
            }

            barCode += checkcode;//变成13位


            char[] symbols = barCode.ToCharArray();

            //--- Validate barCode -------------------------------------------------------------------//
            if (barCode.Length != 13)
            {
                return;
            }
            foreach (char c in symbols)
            {
                if (!Char.IsDigit(c))
                {
                    return;
                }
            }

            //--- Check barcode checksum ------------------------//
            int checkSum = Convert.ToInt32(symbols[12].ToString());
            int calcSum = 0;
            bool one_three = true;
            for (int i = 0; i < 12; i++)
            {
                if (one_three)
                {
                    calcSum += (Convert.ToInt32(symbols[i].ToString()) * 1);
                    one_three = false;
                }
                else
                {
                    calcSum += (Convert.ToInt32(symbols[i].ToString()) * 3);
                    one_three = true;
                }
            }

            char[] calcSumChar = calcSum.ToString().ToCharArray();
            if (checkSum != 0 && checkSum != (10 - Convert.ToInt32(calcSumChar[calcSumChar.Length - 1].ToString())))
            {
                return;
            }
            //--------------------------------------------------//
            //---------------------------------------------------------------------------------------//

            Font font = new Font("Microsoft Sans Serif", 8);

            // Fill backround with white color
            //   g.Clear(Color.White);

            int lineWidth = 2;
            int x = drawBounds.X;

            // Paint human readable 1 system symbol code
            g.DrawString(symbols[0].ToString(), font, new SolidBrush(Color.Black), x, drawBounds.Y + drawBounds.Height - 12);
            x += 10;

            // Paint left 'guard bars', always same '101'
            g.DrawLine(new Pen(Color.Black, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height);
            x += lineWidth;
            g.DrawLine(new Pen(Color.White, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height);
            x += lineWidth;
            g.DrawLine(new Pen(Color.Black, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height);
            x += lineWidth;

            // First number of barcode specifies how to encode each character in the left-hand 
            // side of the barcode should be encoded.
            bool[] leftSideParity = new bool[6];
            switch (symbols[0])
            {
                case '0':
                    leftSideParity[0] = true;  // Odd
                    leftSideParity[1] = true;  // Odd
                    leftSideParity[2] = true;  // Odd
                    leftSideParity[3] = true;  // Odd
                    leftSideParity[4] = true;  // Odd
                    leftSideParity[5] = true;  // Odd
                    break;
                case '1':
                    leftSideParity[0] = true;  // Odd
                    leftSideParity[1] = true;  // Odd
                    leftSideParity[2] = false; // Even
                    leftSideParity[3] = true;  // Odd
                    leftSideParity[4] = false; // Even
                    leftSideParity[5] = false; // Even
                    break;
                case '2':
                    leftSideParity[0] = true;  // Odd
                    leftSideParity[1] = true;  // Odd
                    leftSideParity[2] = false; // Even
                    leftSideParity[3] = false; // Even
                    leftSideParity[4] = true;  // Odd
                    leftSideParity[5] = false; // Even
                    break;
                case '3':
                    leftSideParity[0] = true;  // Odd
                    leftSideParity[1] = true;  // Odd
                    leftSideParity[2] = false; // Even
                    leftSideParity[3] = false; // Even
                    leftSideParity[4] = false; // Even
                    leftSideParity[5] = true;  // Odd
                    break;
                case '4':
                    leftSideParity[0] = true;  // Odd
                    leftSideParity[1] = false; // Even
                    leftSideParity[2] = true;  // Odd
                    leftSideParity[3] = true;  // Odd
                    leftSideParity[4] = false; // Even
                    leftSideParity[5] = false; // Even
                    break;
                case '5':
                    leftSideParity[0] = true;  // Odd
                    leftSideParity[1] = false; // Even
                    leftSideParity[2] = false; // Even
                    leftSideParity[3] = true;  // Odd
                    leftSideParity[4] = true;  // Odd
                    leftSideParity[5] = false; // Even
                    break;
                case '6':
                    leftSideParity[0] = true;  // Odd
                    leftSideParity[1] = false; // Even
                    leftSideParity[2] = false; // Even
                    leftSideParity[3] = false; // Even
                    leftSideParity[4] = true;  // Odd
                    leftSideParity[5] = true;  // Odd
                    break;
                case '7':
                    leftSideParity[0] = true;  // Odd
                    leftSideParity[1] = false; // Even
                    leftSideParity[2] = true;  // Odd
                    leftSideParity[3] = false; // Even
                    leftSideParity[4] = true;  // Odd
                    leftSideParity[5] = false; // Even
                    break;
                case '8':
                    leftSideParity[0] = true;  // Odd
                    leftSideParity[1] = false; // Even
                    leftSideParity[2] = true;  // Odd
                    leftSideParity[3] = false; // Even
                    leftSideParity[4] = false; // Even
                    leftSideParity[5] = true;  // Odd
                    break;
                case '9':
                    leftSideParity[0] = true;  // Odd
                    leftSideParity[1] = false; // Even
                    leftSideParity[2] = false; // Even
                    leftSideParity[3] = true;  // Odd
                    leftSideParity[4] = false; // Even
                    leftSideParity[5] = true;  // Odd
                    break;
            }

            // second number system digit + 5 symbol manufacter code
            string lines = "";
            for (int i = 0; i < 6; i++)
            {
                bool oddParity = leftSideParity[i];
                if (oddParity)
                {
                    switch (symbols[i + 1])
                    {
                        case '0':
                            lines += "0001101";
                            break;
                        case '1':
                            lines += "0011001";
                            break;
                        case '2':
                            lines += "0010011";
                            break;
                        case '3':
                            lines += "0111101";
                            break;
                        case '4':
                            lines += "0100011";
                            break;
                        case '5':
                            lines += "0110001";
                            break;
                        case '6':
                            lines += "0101111";
                            break;
                        case '7':
                            lines += "0111011";
                            break;
                        case '8':
                            lines += "0110111";
                            break;
                        case '9':
                            lines += "0001011";
                            break;
                    }
                }
                // Even parity
                else
                {
                    switch (symbols[i + 1])
                    {
                        case '0':
                            lines += "0100111";
                            break;
                        case '1':
                            lines += "0110011";
                            break;
                        case '2':
                            lines += "0011011";
                            break;
                        case '3':
                            lines += "0100001";
                            break;
                        case '4':
                            lines += "0011101";
                            break;
                        case '5':
                            lines += "0111001";
                            break;
                        case '6':
                            lines += "0000101";
                            break;
                        case '7':
                            lines += "0010001";
                            break;
                        case '8':
                            lines += "0001001";
                            break;
                        case '9':
                            lines += "0010111";
                            break;
                    }
                }
            }

            // Paint human readable left-side 6 symbol code
            string b1 = barCode.Substring(1, 6);
            b1 = "   " + b1.Substring(0, 1) + "  " + b1.Substring(1, 1) + "  " + b1.Substring(2, 1) + "  " + b1.Substring(3, 1) + "  " + b1.Substring(4, 1) + "  " + b1.Substring(5, 1);
            g.DrawString(b1, font, new SolidBrush(Color.Black), x, drawBounds.Y + drawBounds.Height - 12);

            char[] xxx = lines.ToCharArray();
            for (int i = 0; i < xxx.Length; i++)
            {
                if (xxx[i] == '1')
                {
                    g.DrawLine(new Pen(Color.Black, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height - 12);
                }
                else
                {
                    g.DrawLine(new Pen(Color.White, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height - 12);
                }
                x += lineWidth;
            }

            // Paint center 'guard bars', always same '01010'
            g.DrawLine(new Pen(Color.White, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height);
            x += lineWidth;
            g.DrawLine(new Pen(Color.Black, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height);
            x += lineWidth;
            g.DrawLine(new Pen(Color.White, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height);
            x += lineWidth;
            g.DrawLine(new Pen(Color.Black, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height);
            x += lineWidth;
            g.DrawLine(new Pen(Color.White, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height);
            x += lineWidth;

            // 5 symbol product code + 1 symbol parity
            lines = "";
            for (int i = 7; i < 13; i++)
            {
                switch (symbols[i])
                {
                    case '0':
                        lines += "1110010";
                        break;
                    case '1':
                        lines += "1100110";
                        break;
                    case '2':
                        lines += "1101100";
                        break;
                    case '3':
                        lines += "1000010";
                        break;
                    case '4':
                        lines += "1011100";
                        break;
                    case '5':
                        lines += "1001110";
                        break;
                    case '6':
                        lines += "1010000";
                        break;
                    case '7':
                        lines += "1000100";
                        break;
                    case '8':
                        lines += "1001000";
                        break;
                    case '9':
                        lines += "1110100";
                        break;
                }
            }

            // Paint human readable left-side 6 symbol code
            //    
            b1 = barCode.Substring(7, 6);
            b1 = "   " + b1.Substring(0, 1) + "  " + b1.Substring(1, 1) + "  " + b1.Substring(2, 1) + "  " + b1.Substring(3, 1) + "  " + b1.Substring(4, 1) + "  " + b1.Substring(5, 1);
            g.DrawString(b1, font, new SolidBrush(Color.Black), x, drawBounds.Y + drawBounds.Height - 12);

            xxx = lines.ToCharArray();
            for (int i = 0; i < xxx.Length; i++)
            {
                if (xxx[i] == '1')
                {
                    g.DrawLine(new Pen(Color.Black, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height - 12);
                }
                else
                {
                    g.DrawLine(new Pen(Color.White, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height - 12);
                }
                x += lineWidth;
            }

            // Paint right 'guard bars', always same '101'
            g.DrawLine(new Pen(Color.Black, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height);
            x += lineWidth;
            g.DrawLine(new Pen(Color.White, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height);
            x += lineWidth;
            g.DrawLine(new Pen(Color.Black, lineWidth), x, drawBounds.Y, x, drawBounds.Y + drawBounds.Height);
        }
    }
}
