namespace Stocktopus_2 {
    internal static class Core {
        internal static Board board = new Board();

        internal static Color eColor = Color.White;
        internal static Color pColor = Color.Black;

        internal static int nodes = 0;
        internal static int checks = 0;
        internal static int transpositions = 0;

        internal static string nextBookMove = "";

        internal static void Initialize() {
            MovePatternsInitialization.InitializeKingAttacks();
            MovePatternsInitialization.InitializeRankAttacks();
            MovePatternsInitialization.InitializeFileAttacks();
            MovePatternsInitialization.InitializeA1H8DiagonalAttacks();
            MovePatternsInitialization.InitializeH1A8DiagonalAttacks();
        }

        internal static void UCINewgame() {
            nodes = 0;
            transpositions = 0;
            TranspositionTable.Reset();
            Minimax.killSearch = false;
        }

        internal static void SetPosition(string[] args) {
            board.bitboards[1][0] = 0x000000000000FF00;
            board.bitboards[1][1] = 0x0000000000000042;
            board.bitboards[1][2] = 0x0000000000000024;
            board.bitboards[1][3] = 0x0000000000000081;
            board.bitboards[1][4] = 0x0000000000000008;
            board.bitboards[1][5] = 0x0000000000000010;

            board.bitboards[0][0] = 0x00FF000000000000;
            board.bitboards[0][1] = 0x4200000000000000;
            board.bitboards[0][2] = 0x2400000000000000;
            board.bitboards[0][3] = 0x8100000000000000;
            board.bitboards[0][4] = 0x0800000000000000;
            board.bitboards[0][5] = 0x1000000000000000;

            for (int i = 0; i < 64; i++) {
                board.mailbox[i] = new Piece(Color.None, PieceType.None);
            }

            board.mailbox[0] = new Piece(Color.Black, PieceType.Rook);
            board.mailbox[1] = new Piece(Color.Black, PieceType.Knight);
            board.mailbox[2] = new Piece(Color.Black, PieceType.Bishop);
            board.mailbox[3] = new Piece(Color.Black, PieceType.Queen);
            board.mailbox[4] = new Piece(Color.Black, PieceType.King);
            board.mailbox[5] = new Piece(Color.Black, PieceType.Bishop);
            board.mailbox[6] = new Piece(Color.Black, PieceType.Knight);
            board.mailbox[7] = new Piece(Color.Black, PieceType.Rook);

            board.mailbox[8] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[9] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[10] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[11] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[12] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[13] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[14] = new Piece(Color.Black, PieceType.Pawn);
            board.mailbox[15] = new Piece(Color.Black, PieceType.Pawn);

            board.mailbox[56] = new Piece(Color.White, PieceType.Rook);
            board.mailbox[57] = new Piece(Color.White, PieceType.Knight);
            board.mailbox[58] = new Piece(Color.White, PieceType.Bishop);
            board.mailbox[59] = new Piece(Color.White, PieceType.Queen);
            board.mailbox[60] = new Piece(Color.White, PieceType.King);
            board.mailbox[61] = new Piece(Color.White, PieceType.Bishop);
            board.mailbox[62] = new Piece(Color.White, PieceType.Knight);
            board.mailbox[63] = new Piece(Color.White, PieceType.Rook);

            board.mailbox[48] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[49] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[50] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[51] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[52] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[53] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[54] = new Piece(Color.White, PieceType.Pawn);
            board.mailbox[55] = new Piece(Color.White, PieceType.Pawn);

            board.UpdateGenericBitboards();

            eColor = Color.White;
            pColor = Color.Black;

            if (args.Length > 3) {
                int moveCount = 0;
                for (int i = 0; i < args.Length - 3; i++) {
                    moveCount++;
                    board.PerformMove(Move.StringToMove(board, args[i + 3]));
                }

                if (moveCount % 2 == 0) {
                    eColor = Color.White;
                    pColor = Color.Black;
                } else {
                    eColor = Color.Black;
                    pColor = Color.White;
                }
            }

            nextBookMove = "";
            OpeningsBook.CheckForBookMove(args.Length > 3 ? args.Skip(3).ToArray() : Array.Empty<string>(), out nextBookMove);
        }

        internal static string UCIBestmove() {
            Move pick;
            if (nextBookMove != "") {
                Console.WriteLine("book move");
                pick = Move.StringToMove(board, nextBookMove);
            } else {
                pick = Minimax.FindBestMove(Board.Clone(board), 3);
            }

            board.PerformMove(pick);
            Console.WriteLine($"nodes: {nodes}");
            Console.WriteLine($"transpositions: {transpositions}");
            nodes = 0;
            transpositions = 0;

            TranspositionTable.Reset();

            return $"bestmove {pick}";
        }

        internal static bool IsCheck(Board inpboard, Color kingColor) {
            Move[] moves;

            for (int i = 0; i < 5; i++) {
                moves = new Move[64];
                int j = 0;

                if (i == 0) {
                    MoveGeneration.GetKingMoves(new Bitboard(inpboard.bitboards[(byte)kingColor][5]), inpboard, kingColor, moves, ref j);
                    for (int k = 0; k < j; k++)
                        if ((moves[k].capture == 6 || moves[k].capture == 5) && inpboard.mailbox[moves[k].end].color != kingColor) return true;
                } else if (i == 1) {
                    MoveGeneration.GetPawnMoves(new Bitboard(inpboard.bitboards[(byte)kingColor][5]), inpboard, kingColor, moves, ref j);
                    for (int k = 0; k < j; k++)
                        if ((moves[k].capture == 1) && inpboard.mailbox[moves[k].end].color != kingColor) return true;
                } else if (i == 2) {
                    MoveGeneration.GetKnightMoves(new Bitboard(inpboard.bitboards[(byte)kingColor][5]), inpboard, kingColor, moves, ref j);
                    for (int k = 0; k < j; k++)
                        if ((moves[k].capture == 2) && inpboard.mailbox[moves[k].end].color != kingColor) return true;
                } else if (i == 3) {
                    MoveGeneration.GetRookMoves(new Bitboard(inpboard.bitboards[(byte)kingColor][5]), inpboard, kingColor, moves, ref j);
                    for (int k = 0; k < j; k++)
                        if ((moves[k].capture == 4 || moves[k].capture == 5) && inpboard.mailbox[moves[k].end].color != kingColor) return true;
                } else if (i == 4) {
                    MoveGeneration.GetBishopMoves(new Bitboard(inpboard.bitboards[(byte)kingColor][5]), inpboard, kingColor, moves, ref j);
                    for (int k = 0; k < j; k++)
                        if ((moves[k].capture == 3 || moves[k].capture == 5) && inpboard.mailbox[moves[k].end].color != kingColor) return true;
                }
            }

            return false;
        }

        internal static bool IsMoveLegal(Board inpboard, Move move, Color color) {
            Board temp = Board.Clone(inpboard);

            bool isLegal = true;

            if (move.isCastling) {
                if (IsCheck(Board.Clone(inpboard), color)) isLegal = false;
                if (move.end == 2 && !IsMoveLegal(temp, new Move(4, 3, 6, 0, 0, false), color)) isLegal = false;
                else if (move.end == 6 && !IsMoveLegal(temp, new Move(4, 5, 6, 0, 0, false), color)) isLegal = false;
                else if (move.end == 58 && !IsMoveLegal(temp, new Move(60, 59, 6, 0, 0, false), color)) isLegal = false;
                else if (move.end == 62 && !IsMoveLegal(temp, new Move(60, 61, 6, 0, 0, false), color)) isLegal = false;
            }

            if (inpboard.mailbox[move.start].pieceType != PieceType.None) temp.PerformMove(move);
            else isLegal = false;

            return isLegal && !IsCheck(temp, color);
        }
    }
}
