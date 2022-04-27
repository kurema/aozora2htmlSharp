using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Aozora;
using Aozora.Helpers;
using Aozora.Helpers.Tag;

namespace TestProject;

public static class UnitTestI18n
{
    //kurema:
    //原文では`Aozora2Html::I18n.t`のテストをしていましたが、こちらでは実装していないのでテストも省略しています。
    //dotnetは内部Unicodeなので文字コード変換は最初と最後になります。それに伴う変更が若干あり、これも一つです。
    //代わりにDictionaryとstring.Format()のテストを追加しました。
    //
    //ちなみにshift_jisとcp932を区別しません。

    [Fact]
    public static void TestText()
    {
        Assert.Equal("警告(123行目):JIS外字「①」が使われています", string.Format(I18n.MSG["warn_jis_gaiji"], 123, "①"));
    }
}
