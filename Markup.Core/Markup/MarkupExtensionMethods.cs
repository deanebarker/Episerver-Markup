using System.Linq;
using System.Text;

namespace Markup
{
    public static class MarkupExtensionMethods
    {
        /// <summary>
        /// Derives the encoding from the Byte Order Mark and returns a string from it. Defaults to ASCII.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetString(this byte[] bytes)
        {
            var encoding = Encoding.ASCII;

            var bom = bytes.Take(5).ToArray();
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) encoding = Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) encoding = Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) encoding = Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) encoding = Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) encoding = Encoding.UTF32;

            return encoding.GetString(bytes);
        }
    }
}