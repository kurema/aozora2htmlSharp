using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aozora.Helpers
{
    public class Unpacked
    {
        public byte[] InArray { get => inArray; }
        private byte[] inArray;

        public Unpacked(char character)
        {
            inArray = Aozora2Html.ShiftJis.GetBytes(new[] { character });
        }

        public static bool operator <(Unpacked a, string b)
        {
            var bb = Convert.FromHexString(b);
            return CompareBasic(a.InArray, bb) < 0;
        }

        public static bool operator >(Unpacked a, string b)
        {
            var bb = Convert.FromHexString(b);
            return CompareBasic(a.InArray, bb) > 0;
        }

        public static bool operator <=(Unpacked a, string b)
        {
            var bb = Convert.FromHexString(b);
            return CompareBasic(a.InArray, bb) <= 0;
        }

        public static bool operator >=(Unpacked a, string b)
        {
            var bb = Convert.FromHexString(b);
            return CompareBasic(a.InArray, bb) >= 0;
        }

        public static bool operator <(string a, Unpacked b)
        {
            var ba = Convert.FromHexString(a);
            return CompareBasic(ba, b.InArray) < 0;
        }

        public static bool operator >(string a, Unpacked b)
        {
            var ba = Convert.FromHexString(a);
            return CompareBasic(ba, b.InArray) > 0;
        }

        public static bool operator <=(string a, Unpacked b)
        {
            var ba = Convert.FromHexString(a);
            return CompareBasic(ba, b.InArray) <= 0;
        }

        public static bool operator >=(string a, Unpacked b)
        {
            var ba = Convert.FromHexString(a);
            return CompareBasic(ba, b.InArray) >= 0;
        }

        private static int CompareZeroFill(byte[] a, byte[] b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));

            byte getFromLeft(byte[] bytes, int index)
            {
                if (index < 0) return 0;
                if (bytes.Length - index - 1 < 0) return 0;
                return bytes[bytes.Length - index - 1];
            }

            int m = Math.Max(a.Length, b.Length);
            for (int i = 0; i < m; i++)
            {
                byte ca = getFromLeft(a, m - i - 1);
                byte cb = getFromLeft(b, m - i - 1);
                if (ca < cb) return -1;
                if (ca > cb) return 1;
            }
            return 0;
        }

        private static int CompareBasic(byte[] a, byte[] b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            if (a.Length < b.Length) return -1;
            if (a.Length > b.Length) return 1;

            for (int i = 0; i < a.Length; i++)
            {
                byte ca = a[i];
                byte cb = b[i];
                if (ca < cb) return -1;
                if (ca > cb) return 1;
            }
            return 0;
        }
    }
}
