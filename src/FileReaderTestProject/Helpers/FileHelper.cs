

using System;
using System.Linq;
using System.Text;

namespace FileReader.Helpers
{
    public class FileHelperMock
    {
        /// <summary>
        /// Get mock string data.
        /// </summary>
        /// <param name="filename"> file name </param>
        /// <returns> Random byte data </returns>
        /// <remarks> If filename is null or length > 50 will return null. </remarks> 
        public static byte[]? GetFileDataMock(string fileName)
        {
            if (fileName == null || fileName.Length > 50)
                return null;
            return Encoding.ASCII.GetBytes(RandomString(random.Next(10, 100)));
        }

        private static Random random = new Random();

        /// <summary>
        /// Get random string given length.
        /// </summary>
        /// <param name="length"> length of random string </param>
        /// <returns> Random string </returns>
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}