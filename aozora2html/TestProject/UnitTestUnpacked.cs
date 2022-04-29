using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aozora.Helpers;
using Xunit;

namespace TestProject;

public static class UnitTestUnpacked
{
    //kurema:UnpackedはC#移植時に追加したクラスです。テストも新規です。

    [Fact]
    public static void TestNew()
    {
        var unpacked = new Unpacked('ア');
#pragma warning disable IDE0150 // 型のチェックよりも 'null 値' チェックを優先する
        Assert.True(unpacked is Unpacked);
#pragma warning restore IDE0150 // 型のチェックよりも 'null 値' チェックを優先する
    }

    [Fact]
    public static void TestInArray()
    {
        {
            var unpacked = new Unpacked('亜');
            //Assert.True(unpacked is { InArray: [0x88, 0x9f] });
            Assert.True(unpacked.InArray[0] is 0x88 && unpacked.InArray[1] is 0x9f);
        }
        {
            var unpacked = new Unpacked('0');
            Assert.True(unpacked.InArray[0] is 0x30);
        }
        {
            var unpacked = new Unpacked('あ');
            Assert.True(unpacked.InArray[0] is 0x82 && unpacked.InArray[1] is 0xa0);
        }
        {
            var unpacked = new Unpacked('０');
            Assert.True(unpacked.InArray[0] is 0x82 && unpacked.InArray[1] is 0x4f);
        }
    }

    [Fact]
    public static void TestCompare()
    {
        {
            var unpacked = new Unpacked('亜');
            Assert.True(unpacked > "00");
            Assert.True(unpacked >= "5555");
            Assert.True(unpacked == "889f");
            Assert.True(unpacked < "FFFF");
        }
        {
            var unpacked = new Unpacked('0');
            Assert.True(unpacked > "00");
            Assert.True(unpacked == "30");
            Assert.True(unpacked <= "889f");
            Assert.True(unpacked < "FFFF");

            Assert.False(unpacked < "00");
            Assert.False(unpacked != "30");
            Assert.False(unpacked > "889f");
            Assert.False(unpacked > "FFFF");
        }
    }
}
