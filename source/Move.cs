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
        internal readonly bool isEnPassant;

        internal Move(byte start, byte end, byte piece, byte capture, byte promotion, bool isCastling = false, bool isEnPassant = false) {
            this.start = start;
            this.end = end;
            this.piece = piece;
            this.capture = capture;
            this.promotion = promotion;
            this.isCastling = isCastling;
            this.isEnPassant = isEnPassant;
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
            string prom = promotion != 0 ? "nbrq"[promotion - 2].ToString() : "";

            return $"{sfile}{srank}{efile}{erank}{prom}";
        }

        internal static Move StringToMove(Board board, string s) {
            string startStr = s[..2];
            string endStr = s.Substring(2, 2);

            byte startFile = (byte)((byte)"abcdefgh".IndexOf(startStr[0]) + 1);
            byte startRank = (byte)(9 - byte.Parse(startStr[1].ToString()));
            byte endFile = (byte)((byte)"abcdefgh".IndexOf(endStr[0]) + 1);
            byte endRank = (byte)(9 - byte.Parse(endStr[1].ToString()));

            byte start = (byte)(((startRank - 1) * 8) + startFile - 1);
            byte end = (byte)(((endRank - 1) * 8) + endFile - 1);
            byte prom = s.Length == 5
                ? (byte)("nbrq".IndexOf(s[4]) + 2)
                : (byte)PieceType.None;

            bool isCastling = false;
            bool isEnPassant = false;
            if ((byte)board.mailbox[start].pieceType == 1 && (start % 8) + 1 != (end % 8) + 1 && board.mailbox[end].pieceType == 0) isEnPassant = true;
            if ((byte)board.mailbox[start].pieceType == (byte)PieceType.King) {
                if ((start == 4 && (end == 2 || end == 6)) || (start == 60 && (end == 58 || end == 62))) isCastling = true;
            }

            return new(start, end, (byte)board.mailbox[start].pieceType, isEnPassant ? (byte)1 : (byte)board.mailbox[end].pieceType, prom, isCastling, isEnPassant);
        }
    }
}
