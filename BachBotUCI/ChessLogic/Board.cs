using BachBot.ChessLogic;
using BachBotUCI.Utilities;

namespace BachBotUCI.ChessLogic {
    public class Board {
        // Piece Representation
        public int[] Squares;

        // GameState things
        public bool WhiteToMove;
        public GameState GameState;
        public bool WhiteKingsideCastleRight;
        public bool WhiteQueensideCastleRight;
        public bool BlackKingsideCastleRight;
        public bool BlackQueensideCastleRight;
        public int EpFile;
        public int PlyCount;
        public int FiftyMoveCounter => GameState.fiftyMoveCounter;

        // Extra useful stuffs for later
        public string CurrentFEN => FenUtility.CurrentFen(this);
        public FenUtility.PositionInfo StartPositionInfo;
        public List<Move> AllGameMoves;


        public void LoadPosition(string fen = FenUtility.StartPositionFEN) {
            FenUtility.PositionInfo posInfo = FenUtility.loadFen(fen);
            LoadPosition(posInfo);
        }

        public void LoadPosition(FenUtility.PositionInfo posInfo) {
            StartPositionInfo = posInfo;
            Initialize();

            // Load the pieces on the board
            Squares = posInfo.squares.ToArray();
            
            // Set Side to Play
            WhiteToMove = posInfo.whiteToMove;

            // Create Gamestate
            int castlingRights = posInfo.castlingRights;
            PlyCount = (posInfo.moveCount - 1) * 2 + (WhiteToMove ? 0 : 1);
            GameState = new GameState(Piece.None, posInfo.epFile, castlingRights, posInfo.fiftyMovePlyCount);

            // TODO add additional tracking of gamestate history and repetitional history.
        }

        public void Initialize() {
            // Eventually this will be filled with extra things the AI may want to know. Let's just make sure the bot can make a legal move first though.
            AllGameMoves = new List<Move>();
            

            GameState = new GameState();
            PlyCount = 0;
        }

        // This function assumes that the move you want to make is legal, which if you're calling this by NORMAL MEANS the move is legal anyway. 
        // Besides, the code doesn't break if the move is illegal, but the soul of the person making the illegal move does indeed break. (I'm looking at you author.)
        public void MakeMove(Move move) {
            // Get Information about the move
            int startIndex = move.StartSquare;
            int endIndex = move.TargetSquare;
            int moveFlag = move.MoveFlag;
            bool isPromotion = move.IsPromotion;
            bool isEnPassant = moveFlag is Move.EnPassantCaptureFlag;

            // Save some info for later, in the case the value is changed.
            int movedPiece = Squares[startIndex];

            // If king castles, move the rook as well.
            if (Piece.PieceType(Squares[startIndex]) == Piece.King && (endIndex == startIndex + 2 || endIndex == startIndex - 2)) {
                // Castle offsets in long algebraic are just to move king by 2
                int piece = Squares[startIndex];
                int direction = ((endIndex - startIndex) > 0) ? 1 : -1; // Gets +1 or -1 depending on where the king is moving on the board.

                // Find the rook
                foreach (int possibleRookLocation in BoardUtility.ROOK_START_POSITIONS) {
                    if (Math.Abs(possibleRookLocation - startIndex) > 4) { continue; } // Rule out opponent rooks.
                    // Get the rook in direction
                    if (possibleRookLocation > startIndex && direction == 1) {
                        Squares[possibleRookLocation - 2] = Squares[possibleRookLocation];
                        Squares[possibleRookLocation] = 0;
                    } else if (possibleRookLocation < startIndex && direction == -1) {
                        Squares[possibleRookLocation + 3] = Squares[possibleRookLocation];
                        Squares[possibleRookLocation] = 0;
                    }
                }
            }


            //
            Squares[endIndex] = Squares[startIndex];
            Squares[startIndex] = 0;
            WhiteToMove = !WhiteToMove; // On each move made, transfer control to opponent player.
        }

    }
}
