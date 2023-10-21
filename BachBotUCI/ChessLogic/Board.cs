using BachBot.ChessLogic;
using BachBotUCI.Utilities;

namespace BachBotUCI.ChessLogic {
    public class Board {
        // Piece Representation
        public int[] Squares;

        // GameState things
        public bool WhiteToMove;
        public GameState CurrentGameState;
        public int EpFile;
        public int PlyCount;
        public int FiftyMoveCounter => CurrentGameState.fiftyMoveCounter;

        // Extra useful stuffs for later
        public string CurrentFEN => FenUtility.CurrentFen(this);
        public FenUtility.PositionInfo StartPositionInfo;
        public List<Move> AllGameMoves;
        public Stack<Move> moveHistory;
        public int[] KingSquare;

        // Utility
        public int MoveColor => WhiteToMove ? Piece.White : Piece.Black;
        public int OpponentColor => WhiteToMove ? Piece.Black : Piece.White;
        Stack<GameState> gameStateHistory;
        public int MoveColorIndex => WhiteToMove ? 0 : 1;
        
        public Board() {
            Squares = new int[64];
        }

        public void LoadPosition(string fen = FenUtility.StartPositionFEN) {
            FenUtility.PositionInfo posInfo = FenUtility.loadFen(fen);
            LoadPosition(posInfo);
        }

        public void LoadPosition(FenUtility.PositionInfo posInfo) {
            StartPositionInfo = posInfo;
            Initialize();

            // Load the pieces on the board
            for (int squareIndex = 0; squareIndex < 64; squareIndex++) {
                int piece = posInfo.squares[squareIndex];
                int pieceType = Piece.PieceType(piece);
                int colorIndex = Piece.IsWhite(piece) ? 0 : 1;
                Squares[squareIndex] = piece;

                if (pieceType == Piece.King) {
                    KingSquare[colorIndex] = squareIndex;
                }
            }
            
            // Set Side to Play
            WhiteToMove = posInfo.whiteToMove;

            // Create Gamestate
            int whiteCastle = ((posInfo.WhiteKingsideCastle) ? 1 << 0 : 0) | ((posInfo.WhiteQueensideCastle) ? 1 << 1 : 0);
            int blackCastle = ((posInfo.BlackKingsideCastle) ? 1 << 2 : 0) | ((posInfo.BlackQueensideCastle) ? 1 << 3 : 0);
            int castlingRights = whiteCastle | blackCastle;
            

            PlyCount = (posInfo.moveCount - 1) * 2 + (WhiteToMove ? 0 : 1);
            CurrentGameState = new GameState(Piece.None, posInfo.epFile, castlingRights, posInfo.fiftyMovePlyCount);
            gameStateHistory.Push(CurrentGameState);

            // TODO add additional tracking of gamestate history and repetitional history.
        }

        public void Initialize() {
            // Eventually this will be filled with extra things the AI may want to know. Let's just make sure the bot can make a legal move first though.
            AllGameMoves = new List<Move>();
            gameStateHistory = new Stack<GameState>(capacity: 64);
            Array.Clear(Squares);
            KingSquare = new int[2];
            moveHistory = new Stack<Move>(capacity: 256);
            CurrentGameState = new GameState();
            PlyCount = 0;
        }

        // This function assumes that the move you want to make is legal, which if you're calling this by NORMAL MEANS the move is legal anyway. 
        // Besides, the code doesn't break if the move is illegal, but the soul of the person making the illegal move does indeed break. (I'm looking at you author.)
        public void MakeMove(Move move) {
            // Get Information about the move
            int startSquare = move.StartSquare;
            int targetSquare = move.TargetSquare;
            int moveFlag = move.MoveFlag;
            bool isPromotion = move.IsPromotion;
            bool isEnPassant = moveFlag is Move.EnPassantCaptureFlag;

            
            int movedPiece = Squares[startSquare];
            int movedPieceType = Piece.PieceType(movedPiece);
            int capturedPiece = Squares[targetSquare]; // TODO Implement captures of enpassant
            int capturedPieceType = Piece.PieceType(capturedPiece);

            int prevCastleState = CurrentGameState.castlingRights;
            int prevEnPassandFile = CurrentGameState.enPassantFile;

            int newCastlingRights = prevCastleState;
            int newEnPassantFile = 0;

            // Make Move
            Squares[startSquare] = Piece.None;
            Squares[targetSquare] = movedPiece;

            // Handle Captured Pieces
            if (capturedPieceType != Piece.None) {
                int captureSquare = targetSquare;
                // TODO Implement enpassant here
            }

            // Handle King
            if (movedPieceType == Piece.King) {
                KingSquare[MoveColorIndex] = targetSquare;
                newCastlingRights &= (WhiteToMove) ? 0b1100 : 0b0011;

                // Handle castling
                if (moveFlag == Move.CastleFlag) {
                    int rookPiece = Piece.MakePiece(Piece.Rook, MoveColor);
                    bool kingside = targetSquare == 6 || targetSquare == 62; // 6 = g1 and 62 = g8.
                    int castlingRookFromIndex = (kingside) ? targetSquare + 1 : targetSquare - 2;
                    int castlingRookToIndex = (kingside) ? targetSquare - 1 : targetSquare + 1;

                    // Move the rook
                    Squares[castlingRookFromIndex] = Piece.None;
                    Squares[castlingRookToIndex] = Piece.Rook | MoveColor;
                }
            }
            
            // Handle Promotions
            if (isPromotion) {
                int promotionPieceType = moveFlag switch {
                    Move.PromoteToQueenFlag => Piece.Queen,
                    Move.PromoteToRookFlag => Piece.Rook,
                    Move.PromoteToKnightFlag => Piece.Knight,
                    Move.PromoteToBishopFlag => Piece.Bishop,
                    _ => 0
                };

                int promotionPiece = Piece.MakePiece(promotionPieceType, MoveColor);
                Squares[targetSquare] = promotionPiece;
            }

            // Handle Pawn Double Push
            if (moveFlag == Move.PawnTwoUpFlag) {
                int file = BoardUtility.FileIndex(startSquare) + 1;
                newEnPassantFile = file;
            }

            // Update Castling Rights On Rook Capture/Move
            if (prevCastleState != 0) {
                if (targetSquare == 7 || startSquare == 7) { // 7 is h1
                    newCastlingRights &= GameState.ClearWhiteKingsideMask;
                } else if (targetSquare == 0 || startSquare == 0) { // 0 is a1
                    newCastlingRights &= GameState.ClearWhiteQueensideMask;
                }

                if (targetSquare == 56 || startSquare == 56) { // 56 is a8
                    newCastlingRights &= GameState.ClearBlackQueensideMask;
                } else if (targetSquare == 63 || startSquare == 63) { // 63 is h8
                    newCastlingRights &= GameState.ClearBlackKingsideMask;
                }
            }

            // Change Side To Move
            WhiteToMove = !WhiteToMove;

            PlyCount++;
            int newFiftyMoveCounter = CurrentGameState.fiftyMoveCounter + 1;

            // Update 50 Move Counter TODO implement 3-fold repetition
            if (movedPieceType == Piece.Pawn || capturedPieceType != Piece.None) {
                newFiftyMoveCounter = 0;
            }

            GameState newState = new GameState(capturedPieceType, newEnPassantFile, newCastlingRights, newFiftyMoveCounter);
            gameStateHistory.Push(newState);
            CurrentGameState = newState;
            moveHistory.Push(move);
        }

        public void UnMakeMove(Move move) {
            // Swap color to move
            WhiteToMove = !WhiteToMove;
            bool undoingWhiteMove = WhiteToMove;

            // Get Move info 
            int movedFrom = move.StartSquare;
            int movedTo = move.TargetSquare;
            int moveFlag = move.MoveFlag;

            bool undoingEnPassant = moveFlag == Move.EnPassantCaptureFlag;
            bool undoingPromotion = move.IsPromotion;
            bool undoingCapture = CurrentGameState.capturedPieceType != Piece.None;

            int movedPiece = undoingPromotion ? Piece.MakePiece(Piece.Pawn, MoveColor) : Squares[movedTo];
            int movedPieceType = Piece.PieceType(movedPiece);
            int capturedPieceType = CurrentGameState.capturedPieceType;

            // Undo Promotion
            if (undoingPromotion) {
                int promotedPiece = Squares[movedTo];
                int pawnPiece = Piece.MakePiece(Piece.Pawn, MoveColor);
            }

            // Move Piece
            Squares[movedTo] = Piece.None;
            Squares[movedFrom] = movedPiece;

            // Undo Capture
            if (undoingCapture) {
                int captureSquare = movedTo;
                int capturedPiece = Piece.MakePiece(capturedPieceType, OpponentColor);

                if (undoingEnPassant) {
                    captureSquare = movedTo + ((undoingWhiteMove) ? -8 : 8);
                }

                Squares[captureSquare] = capturedPiece;
            }

            // Update King
            if (movedPieceType is Piece.King) {
                // Undo Castling
                if (moveFlag is Move.CastleFlag) {
                    int rookPiece = Piece.MakePiece(Piece.Rook, MoveColor);
                    bool kingside = movedTo == 6 || movedTo == 62;
                    int rookSquareBeforeCastling = (kingside) ? movedTo + 1 : movedTo - 2;
                    int rookSquareAfterCastling = (kingside) ? movedTo - 1 : movedTo + 1;

                    // Return rook to original square
                    Squares[rookSquareAfterCastling] = Piece.None;
                    Squares[rookSquareBeforeCastling] = rookPiece;
                }
            }

            gameStateHistory.Pop();
            CurrentGameState = gameStateHistory.Peek();
            PlyCount--;
            moveHistory.Pop();
        }

    }
}
