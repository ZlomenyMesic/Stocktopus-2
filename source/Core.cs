using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocktopus_2 {
    internal static class Core {
        public static Board board = new Board();

        public static void SetPosition(string[] args) {
            board.bitboards[0][0] = 0x000000000000FF00;
            board.bitboards[0][1] = 0x0000000000000042;
            board.bitboards[0][2] = 0x0000000000000024;
            board.bitboards[0][3] = 0x0000000000000081;
            board.bitboards[0][4] = 0x0000000000000008;
            board.bitboards[0][5] = 0x0000000000000010;

            board.bitboards[0][0] = 0x00FF000000000000;
            board.bitboards[0][1] = 0x4200000000000000;
            board.bitboards[0][2] = 0x2400000000000000;
            board.bitboards[0][3] = 0x8100000000000000;
            board.bitboards[0][4] = 0x0800000000000000;
            board.bitboards[0][5] = 0x1000000000000000;

            if (args.Length > 3) {
                for (int i = 0; i < args.Length - 3; i++) {
                    // TODO
                }
            }
        }

        public static string Bestmove() {
            return "bestmove e2e4";
        }
    }
}
