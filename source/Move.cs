using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal readonly struct Move {
        internal readonly byte start;
        internal readonly byte end;
        internal readonly byte piece;
        internal readonly byte capture;
        internal readonly byte promotion;
        internal readonly bool isCastling;

        internal Move(byte start, byte end, byte piece, byte capture, byte promotion, bool isCastling) {
            this.start = start;
            this.end = end;
            this.piece = piece;
            this.capture = capture;
            this.promotion = promotion;
            this.isCastling = isCastling;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj is not Move) return false;
            else {
                Move compare = (Move)obj;
                return compare.start == start && compare.end == end;
            }
        }

        public override string ToString() {
            string sfile = "abcdefgh"[start % 8].ToString();
            string srank = (8 - ((start - (start % 8)) / 8)).ToString();
            string efile = "abcdefgh"[end % 8].ToString();
            string erank = (8 - ((end - (end % 8)) / 8)).ToString();
            string prom = "nbrq"[promotion - 2].ToString();

            return $"{sfile}{srank}{efile}{erank}{prom}";
        }
    }
}
