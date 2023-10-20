using BachBot.ChessLogic;
using BachBotUCI.Utilities;

namespace BachBotUCI.ChessLogic {
    public class Board {
        public int[] squares;
        private bool whiteToMove;

        public void loadPosition() {
            FenUtility.PositionInfo posInfo = FenUtility.loadFen();
            squares = posInfo.squares.ToArray();
            whiteToMove = posInfo.whiteToMove;

        }

        public void loadPosition(string fen) {
            FenUtility.PositionInfo posInfo = FenUtility.loadFen(fen);
            squares = posInfo.squares.ToArray();
            whiteToMove = posInfo.whiteToMove;
        }

        public bool IsWhiteToPlay() {
            return whiteToMove;
        }

        public void MakeMove(int startIndex, int endIndex) {
            if (Piece.PieceType(squares[startIndex]) == Piece.King) {
                // Castle offsets in long algebraic are just to move king by 2
            }
            // This function assumes that the move you want to make is legal. MUST PASS IN A LEGAL MOVE!!!
            squares[endIndex] = squares[startIndex];
            squares[startIndex] = 0;
            whiteToMove = !whiteToMove; // On each move made, transfer control to opponent player.
        }

        public void MakeMove(Move move) {
            MakeMove(move.StartSquare, move.TargetSquare);
        }

    }
}
