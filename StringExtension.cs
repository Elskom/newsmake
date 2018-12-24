// Copyright (c) 2018, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: GPL, see LICENSE for more details.

namespace Newsmake
{
    internal static class StringExtension
    {
        internal static string Erase(this string str, int offset, int count)
            => str.Replace(str.Substring(offset, offset + count), string.Empty);
    }
}
