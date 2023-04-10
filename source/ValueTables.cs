using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class ValueTables {
        

        public static int GetValue(Piece piece, int i, int numberOfPieces) {
            int mgValue = 0;
            int egValue = 0;
            i = piece.color == Color.White ? i + 1 : 64 - i;

            switch (piece.pieceType) {
                case PieceType.Pawn: mgValue =  Constants.MidgameTables[64 - i]; egValue = Constants.EndgameTables[64 - i]; break;
                case PieceType.Knight: mgValue = Constants.MidgameTables[(2 * 64) - i]; egValue = Constants.EndgameTables[(2 * 64) - i]; break;
                case PieceType.Bishop: mgValue = Constants.MidgameTables[(3 * 64) - i]; egValue = Constants.EndgameTables[(3 * 64) - i]; break;
                case PieceType.Rook: mgValue = Constants.MidgameTables[(4 * 64) - i]; egValue = Constants.EndgameTables[(4 * 64) - i]; break;
                case PieceType.Queen: mgValue = Constants.MidgameTables[(5 * 64) - i]; egValue = Constants.EndgameTables[(5 * 64) - i]; break;
                case PieceType.King: mgValue = Constants.MidgameTables[(6 * 64) - i]; egValue = Constants.EndgameTables[(6 * 64) - i]; break;
            }

            return numberOfPieces > 16 ? mgValue : egValue;
        }
    }
}
