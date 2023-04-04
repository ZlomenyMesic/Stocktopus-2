using Stocktopus_2;

class UCI {
    static void Main(string[] args) {
        while (true) {
            string[] cmd = Console.ReadLine().Split(" ");
            if (cmd.Length != 0) {
                switch (cmd[0]) {
                    case "uci": Console.WriteLine("uciok"); break;
                    case "isready": Console.WriteLine("readyok"); break;
                    case "position": Core.SetPosition(cmd); break;
                    case "go": Console.WriteLine(Core.Bestmove()); break;
                }

                Console.WriteLine();
                Core.board.Print();

                Move[] moves = new Move[64];
                int i = 0;
                MoveGen.GetPawnMoves(Core.board.bitboards[0][0], Core.board, Color.White, moves, ref i);

                foreach (Move move in moves)
                    Console.WriteLine($"{move.start} {move.end}");
            }
        }
    }
}