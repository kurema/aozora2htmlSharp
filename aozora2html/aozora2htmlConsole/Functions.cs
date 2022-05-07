using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aozora.Console;

public static class Functions
{
    //public static StreamReader? GetFirstEntryZip(Stream stream, bool forceShiftJis = false, Action? notShiftJisCallback = null)
    public static StreamReader? GetFirstEntryZip(Stream stream)
    {
        System.IO.Compression.ZipArchive archive = new(stream, System.IO.Compression.ZipArchiveMode.Read);
        var text = archive.Entries.FirstOrDefault(a => a.Name.ToUpperInvariant().EndsWith(".TXT") && a.CompressedLength > 0);
        if (text is null) return null;
        return new StreamReader(text.Open(), Aozora2Html.ShiftJis);
        //if (forceShiftJis) return new StreamReader(text.Open(), Aozora2Html.ShiftJis);
        //return DetectEncodingAndGetStreamReader(() => text.Open(), notShiftJisCallback);
    }

    ////Install ReadJEnc for this to function.
    //public static StreamReader DetectEncodingAndGetStreamReader(Func<Stream> func, Action? notShiftJisCallback = null)
    //{
    //    int codePage;
    //    {
    //        const int assumptionLength = 4096;
    //        using var streamZip = func();
    //        byte[] buffer = new byte[assumptionLength];
    //        streamZip.Read(buffer, 0, assumptionLength);
    //        var charCode = Hnx8.ReadJEnc.ReadJEnc.JP.GetEncoding(buffer, assumptionLength, out string _);
    //        codePage = charCode?.CodePage ?? 932;
    //    }
    //    if (codePage is not 932) notShiftJisCallback?.Invoke();
    //    return new StreamReader(func(), codePage is 932 ? Aozora2Html.ShiftJis : CodePagesEncodingProvider.Instance.GetEncoding(codePage) ?? Aozora2Html.ShiftJis);
    //}
}
