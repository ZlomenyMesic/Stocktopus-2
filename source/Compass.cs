using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal class Compass {
        internal static ulong North(ulong bitBoard) => bitBoard >> 8;
        internal static ulong South(ulong bitBoard) => bitBoard << 8;
        internal static ulong West(ulong bitBoard) => bitBoard >> 1 & 0x7F7F7F7F7F7F7F7F;
        internal static ulong East(ulong bitBoard) => bitBoard << 1 & 0xFEFEFEFEFEFEFEFE;

        internal static ulong SouthEast(ulong bitBoard) => bitBoard << 9 & 0xFEFEFEFEFEFEFEFE;
        internal static ulong SouthWest(ulong bitBoard) => bitBoard << 7 & 0x7F7F7F7F7F7F7F7F;
        internal static ulong NorthEast(ulong bitBoard) => bitBoard >> 7 & 0xFEFEFEFEFEFEFEFE;
        internal static ulong NorthWest(ulong bitBoard) => bitBoard >> 9 & 0x7F7F7F7F7F7F7F7F;
    }
}
