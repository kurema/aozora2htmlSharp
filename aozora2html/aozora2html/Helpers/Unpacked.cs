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
		private readonly byte[] inArray;

		public bool EncodeExceptionThrown { get; private set; } = false;

		public Unpacked(char character)
		{
			try
			{
				inArray = Aozora2Html.ShiftJisExceptionFallback.GetBytes(new[] { character });
			}
			catch
			{
				EncodeExceptionThrown = true;
				inArray = new byte[0];
			}
		}

		public static explicit operator ushort(Unpacked unpacked)
		{
			var array = unpacked.inArray;
			if (array.Length == 0) return 0;
			if (array.Length == 1) return (ushort)array[0];
			if (array.Length == 2) return (ushort)((array[0] << 8) + array[1]);
			throw new OverflowException();
		}


		#region
		//https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/HexConverter.cs
		public static byte[] FromHexString(string chars)
		{
			////if DOTNET6 OR GREATER:
			//return Convert.FromHexString(text);

			if (chars.Length == 0)
				return Array.Empty<byte>();
			if ((uint)chars.Length % 2 != 0)
				throw new FormatException();

			byte[] result = new byte[chars.Length >> 1];

			if (!TryDecodeFromUtf16(chars, ref result))
				throw new FormatException();

			return result;
		}

		public static bool TryDecodeFromUtf16(string chars, ref byte[] bytes)
		{
			static int FromChar(int c)
			{
				var CharToHexLookup = new byte[]
{
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 15
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 31
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 47
            0x0,  0x1,  0x2,  0x3,  0x4,  0x5,  0x6,  0x7,  0x8,  0x9,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 63
            0xFF, 0xA,  0xB,  0xC,  0xD,  0xE,  0xF,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 79
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 95
            0xFF, 0xa,  0xb,  0xc,  0xd,  0xe,  0xf,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 111
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 127
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 143
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 159
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 175
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 191
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 207
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 223
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 239
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF  // 255
};

				return c >= CharToHexLookup.Length ? 0xFF : CharToHexLookup[c];
			}

			int i = 0;
			int j = 0;
			int byteLo = 0;
			int byteHi = 0;
			while (j < bytes.Length)
			{
				byteLo = FromChar(chars[i + 1]);
				byteHi = FromChar(chars[i]);

				// byteHi hasn't been shifted to the high half yet, so the only way the bitwise or produces this pattern
				// is if either byteHi or byteLo was not a hex character.
				if ((byteLo | byteHi) == 0xFF)
					break;

				bytes[j++] = (byte)((byteHi << 4) | byteLo);
				i += 2;
			}

			//if (byteLo == 0xFF)
			//    i++;

			//charsProcessed = i;
			return (byteLo | byteHi) != 0xFF;
		}
		#endregion

		public static bool operator ==(Unpacked a, string b)
		{
			var bb = FromHexString(b);
			return CompareBasic(a.inArray, bb) == 0;
		}
		public static bool operator !=(Unpacked a, string b)
		{
			var bb = FromHexString(b);
			return CompareBasic(a.inArray, bb) != 0;
		}

		public static bool operator <(Unpacked a, string b)
		{
			var bb = FromHexString(b);
			return CompareBasic(a.InArray, bb) < 0;
		}

		public static bool operator >(Unpacked a, string b)
		{
			var bb = FromHexString(b);
			return CompareBasic(a.InArray, bb) > 0;
		}

		public static bool operator <=(Unpacked a, string b)
		{
			var bb = FromHexString(b);
			return CompareBasic(a.InArray, bb) <= 0;
		}

		public static bool operator >=(Unpacked a, string b)
		{
			var bb = FromHexString(b);
			return CompareBasic(a.InArray, bb) >= 0;
		}

		public static bool operator <(string a, Unpacked b)
		{
			var ba = FromHexString(a);
			return CompareBasic(ba, b.InArray) < 0;
		}

		public static bool operator >(string a, Unpacked b)
		{
			var ba = FromHexString(a);
			return CompareBasic(ba, b.InArray) > 0;
		}

		public static bool operator <=(string a, Unpacked b)
		{
			var ba = FromHexString(a);
			return CompareBasic(ba, b.InArray) <= 0;
		}

		public static bool operator >=(string a, Unpacked b)
		{
			var ba = FromHexString(a);
			return CompareBasic(ba, b.InArray) >= 0;
		}

		//private static int CompareZeroFill(byte[] a, byte[] b)
		//{
		//    if (a is null) throw new ArgumentNullException(nameof(a));
		//    if (b is null) throw new ArgumentNullException(nameof(b));

		//    static byte getFromLeft(byte[] bytes, int index)
		//    {
		//        if (index < 0) return 0;
		//        if (bytes.Length - index - 1 < 0) return 0;
		//        return bytes[bytes.Length - index - 1];
		//    }

		//    int m = Math.Max(a.Length, b.Length);
		//    for (int i = 0; i < m; i++)
		//    {
		//        byte ca = getFromLeft(a, m - i - 1);
		//        byte cb = getFromLeft(b, m - i - 1);
		//        if (ca < cb) return -1;
		//        if (ca > cb) return 1;
		//    }
		//    return 0;
		//}

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

		public override bool Equals(object? obj)
		{
			return obj is Unpacked unpacked && unpacked == this;
		}

		public override int GetHashCode()
		{
			return inArray.GetHashCode();
		}
	}
}
