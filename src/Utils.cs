using System.Text;

namespace VCS
{
    public class Utils
    {
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