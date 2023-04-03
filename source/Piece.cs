using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal readonly struct Piece {
        internal readonly Color color;
        internal readonly PieceType pieceType;

        internal Piece(Color color, PieceType pieceType) {
            this.color = color;
            this.pieceType = pieceType;
        }
    }
}
