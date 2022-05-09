using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers;

public interface INewMidashiIdProvider
{
    int GenerateNewMidashiId(int size);
    int GenerateNewMidashiId(string size);
    bool BlockAllowedContext { get; }
}
