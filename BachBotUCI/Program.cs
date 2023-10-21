using BachBotUCI.ChessLogic;
using BachBotUCI.Utilities;

namespace BachBotUCI {
    class Program {
        static void Main(string[] args) {
            UCIEngine engine = new UCIEngine();

            string command = String.Empty;
            while (command != "quit") {
                command = Console.ReadLine();
                engine.ReceiveCommand(command);
            }
            /*
             *  TODO:
             *  
             *  Engine doesn't understand check
             *  Engine doesn't understand enpassant
             *  Fen isn't technically fully loaded
             *  
             *  BUGS:
             *  Sometimes pushes pawns that aren't in the startpos forward two.
             */
        }
    }
}
