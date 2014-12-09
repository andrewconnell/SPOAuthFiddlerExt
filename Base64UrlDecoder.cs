using System;
using System.Globalization;
using System.Text;

namespace SPOAuthFiddlerExt
{
    public class Base64UrlDecoder
    {
        /// <summary>
        /// URL decodes a string.
        /// </summary>
        /// <param name="arg">The string to decode.</param>
        /// <returns>The URL decoded string.  </returns>
        public static string Decode(string arg)
        {
            return Encoding.UTF8.GetString(DecodeBytes(arg));
        }

        /// <summary>
        /// URL decodes a string into a byte array
        /// </summary>
        /// <param name="value">The string to decode</param>
        /// <returns>The decoded byte array</returns>
        public static byte[] DecodeBytes(string value)
        {                    
            const char BASE64_PAD_CHARACTER = '=';
            const string DOUBLE_BASE64_PAD_CHARACTER = "==";

            const char BASE64_CHARACTER_62 = '+';
            const char BASE64URL_CHARACTER_62 = '-';

            const char BASE64_CHARACTER_63 = '/';            
            const char BASE64URL_CHARACTER_63 = '\u005F';

            byte[] ret;

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value", "A null value cannot be decoded.");
            }

            string convertedValue = value;

            //Replace "-" with "+"
            convertedValue = convertedValue.Replace(BASE64URL_CHARACTER_62, BASE64_CHARACTER_62);
            //Replace the ENQ character with "/"
            convertedValue = convertedValue.Replace(BASE64URL_CHARACTER_63, BASE64_CHARACTER_63);
            
            switch (convertedValue.Length % 4)
            {
                case 0:
                    {
                        ret = Convert.FromBase64String(convertedValue);
                        break;
                    }
                case 2:
                    {
                        convertedValue += DOUBLE_BASE64_PAD_CHARACTER;
                        ret = Convert.FromBase64String(convertedValue);
                        break;
                    }
                case 3:
                    {
                        convertedValue += BASE64_PAD_CHARACTER;
                        ret = Convert.FromBase64String(convertedValue);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Not a valid base64 URL string", value);
                    }
            }

            return ret;
        }

    }
}