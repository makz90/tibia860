using System;

namespace OpenTibia.Common.Structures
{
    [Flags]
    public enum Addon : byte
    {
        None = 0,

        First = 1,

        Second = 2,

        Both = First | Second
    }
}