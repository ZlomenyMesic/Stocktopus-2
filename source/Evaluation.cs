using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class Evaluation {
        internal static int Evaluate(Board board) {
            int eval = 0;
            int rev = Core.eColor == Color.White ? 1 : -1;

            for (int i = 0; i < 64; i++) {
                int rev2 = board.mailbox[i].color == Core.eColor ? 1 : -1;
                eval += GetTableValue(board.mailbox[i], i, board.numberOfPieces) * rev2;
            }

            if (board.canWhiteCastleKingside) eval += 30 * rev;
            if (board.canWhiteCastleQueenside) eval += 15 * rev;
            if (board.canBlackCastleKingside) eval += 30 * -rev;
            if (board.canBlackCastleQueenside) eval += 15 * -rev;

            return eval;
        }

        internal static int BasicEval(Board board) {
            int eval = 0;
            for (int i = 0; i < 64; i++) {
                int rev = board.mailbox[i].color == Core.eColor ? 1 : -1;
                switch (board.mailbox[i].pieceType) {
                    case PieceType.Pawn: eval += 1 * rev; break;
                    case PieceType.Knight: eval += 3 * rev; break;
                    case PieceType.Bishop: eval += 3 * rev; break;
                    case PieceType.Rook: eval += 5 * rev; break;
                    case PieceType.Queen: eval += 9 * rev; break;
                }
            }
            return eval;
        }

        internal static int GetTableValue(Piece piece, int i, int numberOfPieces) {
            int mgValue = 0;
            int egValue = 0;
            i = piece.color == Color.White ? i + 1 : 64 - i;

            switch (piece.pieceType) {
                case PieceType.Pawn: mgValue = Constants.MidgameTables[64 - i]; egValue = Constants.EndgameTables[64 - i]; break;
                case PieceType.Knight: mgValue = Constants.MidgameTables[(2 * 64) - i]; egValue = Constants.EndgameTables[(2 * 64) - i]; break;
                case PieceType.Bishop: mgValue = Constants.MidgameTables[(3 * 64) - i]; egValue = Constants.EndgameTables[(3 * 64) - i]; break;
                case PieceType.Rook: mgValue = Constants.MidgameTables[(4 * 64) - i]; egValue = Constants.EndgameTables[(4 * 64) - i]; break;
                case PieceType.Queen: mgValue = Constants.MidgameTables[(5 * 64) - i]; egValue = Constants.EndgameTables[(5 * 64) - i]; break;
                case PieceType.King: mgValue = Constants.MidgameTables[(6 * 64) - i]; egValue = Constants.EndgameTables[(6 * 64) - i]; break;
            }

            // TODO: Gradual evaluation

            //return numberOfPieces > 24 ? mgValue : ((mgValue - egValue) / 12) * (24 - numberOfPieces);

            return numberOfPieces > 16 ? mgValue : egValue;
        }
    }
}
