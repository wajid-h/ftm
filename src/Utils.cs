using System.Text;

namespace VCS
{
    public class Utils
    {   

        /// <summary>
        /// Generates a random ASCII string from given character range 
        /// </summary>
        /// <param name="len">length of string</param>
        /// <param name="characterRangeStart">starting index</param>
        /// <param name="characterRangeEnd">end index</param>
        /// <returns></returns>
        internal static string GetRandomString(int len, ushort characterRangeStart = 97, ushort characterRangeEnd = 122)
        {

            StringBuilder builder = new();
            ushort start = characterRangeStart;
            ushort end = characterRangeEnd;

            Random random = new();

            for (int i = 0; i < len; i++)
            {
                builder.Append((char)(random.Next(start, end) % 255));
            }
            return builder.ToString();
        }
    }
}