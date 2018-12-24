// Copyright (c) 2018, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: GPL, see LICENSE for more details.

namespace newsmake
{
internal static class StringExtension
{
    internal static string Erase(this string str, int _Off, int _Cnt)
    {
        return str.Replace(str.Substring(_Off, _Off + _Cnt), "");
    }
}
}
