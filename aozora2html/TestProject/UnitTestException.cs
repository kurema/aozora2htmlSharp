using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Aozora;
using Aozora.Helpers;
using Aozora.Helpers.Tag;
using Aozora.Exceptions;

namespace TestProject;

public static class UnitTestException
{
    [Fact]
    public static void TestRaiseAozoraException()
    {
#pragma warning disable IDE0059 // 値の不必要な代入
        bool raised = false;
#pragma warning restore IDE0059 // 値の不必要な代入
        try
        {
            throw new AozoraException();
        }
        catch (AozoraException)
        {
            raised = true;
        }
        Assert.True(raised);
    }

    [Fact]
    public static void TestRaiseAozoraError()
    {
        const string Message = "sample error";
        string? error_msg;
        try
        {
            throw new AozoraException(Message);
        }
        catch (AozoraException e)
        {
            error_msg = e.Message;
        }
        Assert.Equal(Message, error_msg);
    }
}
