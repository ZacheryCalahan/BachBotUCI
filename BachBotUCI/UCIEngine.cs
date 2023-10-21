using BachBot.ChessLogic;
using BachBotUCI.ChessLogic;
using BachBotUCI.Utilities;

namespace BachBotUCI {
    public class UCIEngine {
        Bot player;
        static readonly string[] positionLabels = new[] { "position", "fen", "moves" };
        static readonly string[] goLabels = new[] { "go", "movetime", "wtime", "btime", "winc", "binc", "movestogo" };

        public UCIEngine() {
            player = new Bot();
            player.OnMoveChosen += OnMoveChosen;
        }

        public void ReceiveCommand(string message) {
            message = message.Trim();
            string messageType = message.Split(' ')[0].ToLower();

            switch (messageType) {
                case "uci":
                    Respond("uciok");
                    break;

                case "isready":
                    Respond("readyok");
                    break;

                case "ucinewgame":
                    break;

                case "position":
                    ProcessPositionCommand(message);
                    break;

                case "go":
                    ProcessGoCommand(message);
                    break;

                case "stop":
                    break;

                case "quit":
                    break;

                case "d":
                    Console.Write(BoardUtility.CreateDiagram(player.board));
                    break;

                case "printmoves":
                    List<Move> moves;
                    moves = MoveGenerator.GeneratePseudoLegalMoves(player.board);
                    foreach (Move move in moves) {
                        Console.WriteLine(BoardUtility.GetMoveNameUCI(move));
                    }

                    break;

                default:
                    //Debugger.Launch();
                    break;

            }
        }

        public void Respond(string response) {
            // This seems weird to be stubbed out, but it allows debugging in the future.
            Console.WriteLine(response);
        }

        public void OnMoveChosen(string move) {
            // Also stubbed out for debugging in the future
            Respond("bestmove " + move);
        }

        public void ProcessGoCommand(string message) {
            int wtime = TryGetLabelledValueInt(message, "wtime", goLabels);
            int btime = TryGetLabelledValueInt(message, "btime", goLabels);
            
            player.ChooseMove();
        }

        public void ProcessPositionCommand(string message) {
            // Determines type of Position command
            if (message.ToLower().Contains("startpos")) {
                player.SetPosition();
            } else if (message.ToLower().Contains("fen")) {
                string customFen = TryGetLabelledValue(message, "fen", positionLabels);
                player.SetPosition(customFen);
            } else {
                Console.WriteLine("Invalid position command.");
            }

            // Makes the moves given.
            string allMoves = TryGetLabelledValue(message, "moves", positionLabels);
            if (!string.IsNullOrEmpty(allMoves)) {
                string[] moveList = allMoves.Split(' ');
                foreach (string move in moveList) {
                    player.MakeMove(move);
                }
            }
        }

        static int TryGetLabelledValueInt(string text, string label, string[] allLabels, int defaultValue = 0) {
            string valueString = TryGetLabelledValue(text, label, allLabels, defaultValue + "");
            if (int.TryParse(valueString.Split(' ')[0], out int result)) {
                return result;
            }

            return defaultValue;
        }

        static string TryGetLabelledValue(string text, string label, string[] allLabels, string defaultValue = "") {
            text = text.Trim();

            if (text.Contains(label)) {
                int valueStart = text.IndexOf(label) + label.Length;
                int valueEnd = text.Length;
                foreach (string otherID in allLabels) {
                    if (otherID != label && text.Contains(otherID)) {
                        int otherIDStartIndex = text.IndexOf(otherID);
                        if (otherIDStartIndex > valueStart && otherIDStartIndex < valueEnd) {
                            valueEnd = otherIDStartIndex;
                        }
                    }
                }

                return text.Substring(valueStart, valueEnd - valueStart).Trim();
            }
            return defaultValue;
        }
    }
}
