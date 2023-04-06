using Stocktopus_2;

class UCI {
    static void Main(string[] args) {
        Core.Initialize();
        while (true) {
            string[] cmd = Console.ReadLine().Split(" ");
            if (cmd.Length != 0) {
                switch (cmd[0]) {
                    case "uci": Console.WriteLine("uciok"); break;
                    case "isready": Console.WriteLine("readyok"); break;
                    case "position": Core.SetPosition(cmd); break;
                    case "go": Console.WriteLine(Core.Bestmove()); break;
                }
            }
        }
    }
}