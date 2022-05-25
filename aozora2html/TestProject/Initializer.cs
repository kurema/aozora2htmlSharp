using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject;

internal static class Initializer
{
    //.NET 6以降ならModuleInitializerで読み込み時一度実行することができます。

    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void Initialize()
    {
        Aozora.Resources.Resource.Culture = new System.Globalization.CultureInfo("ja-jp");
    }
}
