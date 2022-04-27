using System;
using System.Collections.Generic;
using System.Text;

namespace Aozora.Helpers;

public interface INewMidashiIdProvider
{
    int new_midashi_id(int size);
    int new_midashi_id(string size);
    bool block_allowed_context { get; }
}
