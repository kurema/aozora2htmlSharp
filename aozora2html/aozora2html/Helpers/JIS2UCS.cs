using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace Aozora.Helpers;

public static partial class YamlValues
{
	public static string? Jisx0213ToHtmlEntity(string code)
	{
		var codes = code.Split('-').Select(a => int.TryParse(a, out int b) ? b : -1).ToArray();
		return Jisx0213ToHtmlEntity(codes);
	}

	public static string? Jisx0213ToHtmlEntity(int men, int ku, int ten)
	{
		return Jisx0213ToHtmlEntity(new int[] { men, ku, ten });
	}

	private static string? Jisx0213ToHtmlEntity(int[] codes)
	{
		const string resultHeader = "&#x";
		const string resultFooter = ";";

		return Jisx0213ToUnicodeGeneral(codes, new Func<ReadOnlyMemory<byte>, string>[]
		{
			r =>$"{resultHeader}{r.Span[0]:X2}{r.Span[1]:X2}{resultFooter}",
			r =>
			{
				var span=r.Span;
				return $"{resultHeader}{span[1]:X2}{span[2] & 0x0F:X}{span[3] >> 4:X}{resultFooter}" + $"{resultHeader}{span[3] & 0x0F:X}{span[4] & 0x0F:X}{span[5]:X2}{resultFooter}";
			},
			r=>
			{
				var span=r.Span;
				return $"{resultHeader}{span[0] & 0xF:X}{span[1]:X2}{span[3]:X2}{resultFooter}";
			}
		});
	}

	public static string? Jisx0213NumberToFormated((int men, int ku, int ten) tuple) => Jisx0213NumberToFormated(tuple.men, tuple.ku, tuple.ten);

	public static string? Jisx0213NumberToFormated(int men, int ku, int ten) => $"第{men+2}水準{men}面{ku}区{ten}点";


	public static string? Jisx0213ToString(string code)
	{
		var codes = code.Split('-').Select(a => int.TryParse(a, out int b) ? b : -1).ToArray();
		return Jisx0213ToString(codes);
	}

	public static string? Jisx0213ToString(int men, int ku, int ten)
	{
		return Jisx0213ToString(new[] { men, ku, ten });
	}

	private static string? Jisx0213ToString(int[] codes)
	{
		int[]? codePoints = Jisx0213ToUnicodeGeneral(codes, new Func<ReadOnlyMemory<byte>, int[]?>[]
		{
			r =>new[]{ (r.Span[0] << 8)+r.Span[1] },
			r =>
			{
				var span=r.Span;
				return new[]
				{
					(span[1] << 8)+((span[2] & 0x0F) << 4)+(span[3] >> 4),
					((span[3] & 0xF) << 12) +((span[4] & 0xF) << 8) + span[5],
				};
			},
			r=>
			{
				var span=r.Span;
				return new[]{((span[0] & 0xF) << 16)+(span[1] << 8) + span[3], };
			}
		}); ;
		if (codePoints is null or { Length: 0 }) return null;
		var sb = new StringBuilder();
		foreach (var cp in codePoints)
		{
			sb.Append(char.ConvertFromUtf32(cp));
		}
		return sb.ToString();
	}

	private static T? Jisx0213ToUnicodeGeneral<T>(int[] codes, Func<ReadOnlyMemory<byte>, T>[] funcs)
	{
		static int VirtualPosToRealPos(int pos)
		{
			//kurema:
			//デカい空白領域を省略する簡易的な圧縮。
			//この値はバイナリに埋め込みたいが、設計上微妙。末尾なら8バイト、先頭なら4バイト必要。どちらも美しくない。
			const int BlankStart = 0x50AA;//kurema:2面16区79点
			const int BlankEnd = 0x7D96;

			return pos switch
			{
				>= BlankStart and < BlankEnd => -1,
				>= BlankStart => pos - (BlankEnd - BlankStart),
				_ => pos,
			};
		}

		if (codes.Length < 3) return default;
		if (!(codes[0] is 1 or 2) || !(codes[1] is > 0 and <= 94) || !(codes[2] is > 0 and <= 94)) return default;
		var dic = Jisx0213ToUnicodeBytes;
		if (dic is null) return default;

		int pos = (((codes[0] - 1) * 94 + (codes[1] - 1)) * 94 + codes[2]) * 2;//kurema:ここで-2を加えると2バイト節約できる。その代わりこの領域をヘッダにした。
		pos = VirtualPosToRealPos(pos);
		if (pos < 0) return default;
		if (pos >= dic.Value.Length) return default;


		var slice1 = dic.Value.Slice(pos);
		var span1 = slice1.Span;

		//kurema:自分が作ったファイルなのでLengthチェックなど要らぬ。
		if (span1[0] is >= 0xA0 and <= 0xAF || (span1[0] == 0x00 && span1[1] == 0x00))
		{
			return default;
		}
		else if (span1[0] is >= 0xA0 and <= 0xEF)
		{
			int posRef = (((span1[0] - 0xB0) << 8) + span1[1]) * 2;
			posRef = VirtualPosToRealPos(posRef);
			if (posRef < 0) return default;
			var slice2 = dic.Value.Slice(posRef);
			if (slice2.Span[0] == 0xA0)
			{
				return funcs[1](slice2);

			}
			else if ((slice2.Span[0] & 0xF0) == 0xA0)//kurema:0xA1-0xAFの場合
			{
				return funcs[2](slice2);
			}
			else return default;
		}
		else
		{
			return funcs[0](slice1);
			//return $"{resultHeader}{span1[0]:X2}{span1[1]:X2}{resultFooter}";
		}
	}

	static ReadOnlyDictionary<string, (int men, int ku, int ten)>? _Jisx0213ReverseDictionary = null;

	public static ReadOnlyDictionary<string, (int men, int ku, int ten)> Jisx0213ReverseDictionary
	{
		get
		{
			if (_Jisx0213ReverseDictionary is not null) return _Jisx0213ReverseDictionary;
			//kurema:非効率な逆辞書作成。しかも空白が途中にはない前提。ただし実際に空白は途中にないし、当環境でデバッグ実行でも20ミリ秒程度なので無視出来る。
			var dic = new Dictionary<string, (int men, int ku, int ten)>();
			for (int m = 1; m <= 2; m++)
			{
				for (int k = 1; k <= 94; k++)
				{
					for (int t = 1; t <= 94; t++)
					{
						var s = Jisx0213ToString(m, k, t);
						if (s is null) break;
						dic.Add(s, (m, k, t));
					}
				}
			}

			return _Jisx0213ReverseDictionary = new(dic);
		}
	}

	private static ReadOnlyMemory<byte>? _Jisx0213ToUnicodeBytes;
	public static ReadOnlyMemory<byte>? Jisx0213ToUnicodeBytes { get { return _Jisx0213ToUnicodeBytes ??= LoadJisx0213ToUnicodeBytes(); } }

	private static ReadOnlyMemory<byte>? LoadJisx0213ToUnicodeBytes()
	{
		try
		{
			using var s1 = Assembly.GetAssembly(typeof(YamlValues))?.GetManifestResourceStream("Aozora.jis2ucs.bin");
			if (s1 is null) throw new Exceptions.FailedToLoadJIS2UCSException();
			var result = new byte[s1.Length];
			s1.Seek(0, SeekOrigin.Begin);
			s1.Read(result, 0, (int)s1.Length);
			return result.AsMemory();
		}
		catch
		{
			return null;
		}
	}

}


