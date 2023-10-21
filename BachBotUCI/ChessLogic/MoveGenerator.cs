using BachBot.ChessLogic;
using BachBotUCI.Utilities;

namespace BachBotUCI.ChessLogic {
    public static class MoveGenerator {
        public static List<Move> GeneratePseudoLegalMoves(Board board) {
            List<Move> moves = new List<Move>();

            // Loop through each piece on the board
            int squareIndex = 0;
            foreach (int piece in board.Squares) {
                
                if (Piece.IsColor(piece, board.WhiteToMove)) {
                    if (Piece.IsDiagonalSlider(piece)) {
                        moves.AddRange(GenerateDiagonalMoves(board, piece, squareIndex));
                    } if (Piece.IsOrthogonalSlider(piece)) {
                        moves.AddRange(GenerateOrthogonalMoves(board, piece, squareIndex));
                    } if (Piece.PieceType(piece) == Piece.King) {
                        moves.AddRange(GenerateKingMoves(board, piece, squareIndex));
                    } if (Piece.PieceType(piece) == Piece.Knight) {
                        moves.AddRange(GenerateKnightMoves(board, piece, squareIndex));
                    } if (Piece.PieceType(piece) == Piece.Pawn) {
                        moves.AddRange(GeneratePawnMoves(board, piece, squareIndex));
                    }
                    
                }
                squareIndex++;
            }

            return moves;
        }

        // Piece Move Generators

        private static List<Move> GenerateDiagonalMoves(Board board, int piece, int piecePosition) {
            List<Move> moves = new List<Move>();
            bool pieceIsWhite = Piece.IsWhite(piece);

            // Loop through every direction.
            foreach (int direction in Piece.diagonalOffsets) {
                int targetSquare = piecePosition;
                
                // Loop through every move the direction until piece is off the board.
                while (true) {
                    targetSquare += direction; // Increment in this direction.

                    // If targetSquare is an invalid coordinate, stop in this direction.
                    if (!BoardUtility.IsValidTileCoordinate(targetSquare)) { break; }

                    // If a piece is in an exempt column, stop in this direction.
                    if (BoardUtility.FIRST_COLUMN.Contains(targetSquare - direction) && (direction == -9 || direction == 7)) { break;}
                    if (BoardUtility.EIGHTH_COLUMN.Contains(targetSquare - direction) && (direction == -7 || direction == 9)) { break;}

                    if (board.Squares[targetSquare] == 0) {
                        moves.Add(new Move(piecePosition, targetSquare));
                    } else if (!Piece.IsColor(piece, Piece.PieceColor(board.Squares[targetSquare]))) { // Square contains an opponent piece, stop in this direction after move
                        moves.Add(new Move(piecePosition, targetSquare));
                        break;
                    } else { // Square contains friendly piece (by POE), don't add move and stop in this direction.
                        break;
                    }
                }
            }

            return moves;
        } // WORKING

        private static List<Move> GenerateOrthogonalMoves(Board board, int piece, int piecePosition) {
            List<Move> moves = new List<Move>();
            bool pieceIsWhite = Piece.IsWhite(piece);

            // Loop through every direction.
            foreach (int direction in Piece.orthogonalOffsets) {
                int targetSquare = piecePosition;

                // Loop through every move the direction until piece is off the board.
                while (true) {
                    targetSquare += direction; // Increment in this direction.

                    // If a piece is in an exempt column, stop in this direction.
                    if (!BoardUtility.IsValidTileCoordinate(targetSquare)) {
                        break;
                    } // Seems a bit extra, but this prevents it from breaking?
                    if (BoardUtility.FIRST_COLUMN.Contains(targetSquare - direction) && (direction == -1)) { break; }
                    if (BoardUtility.EIGHTH_COLUMN.Contains(targetSquare - direction) && (direction == 1)) { break; }

                    if (board.Squares[targetSquare] == 0) {
                        moves.Add(new Move(piecePosition, targetSquare));
                    } else if (!Piece.IsColor(piece, Piece.PieceColor(board.Squares[targetSquare]))) { // Square contains an opponent piece, stop in this direction after move
                        moves.Add(new Move(piecePosition, targetSquare));
                        break;
                    } else { // Square contains friendly piece (by POE), don't add move and stop in this direction.
                        break;
                    }
                }
            }

            return moves;
        } // WORKING

        private static List<Move> GenerateKingMoves(Board board, int piece, int piecePosition) {
            List<Move> moves = new List<Move>();
            bool pieceIsWhite = Piece.IsWhite(piece);

            // Loop through each direction
            foreach (int direction in Piece.kingOffsets) {
                int targetSquare = piecePosition + direction;
                // Don't add move if off board
                if (!BoardUtility.IsValidTileCoordinate(targetSquare)) { continue; }

                // If piece is in an exempt column, don't add move in exempt direction.
                if (BoardUtility.FIRST_COLUMN.Contains(piecePosition) && (direction == -1 || direction == -9 || direction == 7)) { continue; } 
                if (BoardUtility.EIGHTH_COLUMN.Contains(piecePosition) && (direction == 1 || direction == 9 || direction == -7)) { continue; }

                if (board.Squares[targetSquare] == 0) { // Square is empty
                    moves.Add(new Move(piecePosition, targetSquare));
                } else if (!Piece.IsColor(piece, Piece.PieceColor(board.Squares[targetSquare]))) { // Target square contains opponent piece
                    moves.Add(new Move(piecePosition, targetSquare));
                }
            }

            // Calculate Castles


            return moves;
        } 

        private static List<Move> GenerateKnightMoves(Board board, int piece, int piecePosition) {
            List<Move> moves = new List<Move>();
            bool pieceIsWhite = Piece.IsWhite(piece);

            // Loop through each direction
            foreach (int direction in Piece.knightOffsets) {
                int targetSquare = piecePosition + direction;
                // Don't add move if off board
                if (!BoardUtility.IsValidTileCoordinate(targetSquare)) { continue; }

                // If piece is in an exempt column, don't add move in exempt direction.
                if (BoardUtility.FIRST_COLUMN.Contains(piecePosition) && (direction == -17 || direction == -10 || direction == 6 || direction == 15)) { continue; }
                if (BoardUtility.SECOND_COLUMN.Contains(piecePosition) && (direction == -10 || direction == 6)) { continue; }
                if (BoardUtility.SEVENTH_COLUMN.Contains(piecePosition) && (direction == -6 || direction == 10)) { continue; }
                if (BoardUtility.EIGHTH_COLUMN.Contains(piecePosition) && (direction == 17 || direction == 10 || direction == -6 || direction == -15)) { continue; }

                if (board.Squares[targetSquare] == 0) { // Square is empty
                    moves.Add(new Move(piecePosition, targetSquare));
                } else if (!Piece.IsColor(piece, Piece.PieceColor(board.Squares[targetSquare]))) { // Target square contains opponent piece
                    moves.Add(new Move(piecePosition, targetSquare));
                }

            }

            return moves;
        }

        private static List<Move> GeneratePawnMoves(Board board, int piece, int piecePosition) {
            List<Move> moves = new List<Move>();
            bool isPieceWhite = Piece.IsWhite(piece);
            int direction = 8 * (isPieceWhite ? 1 : -1); // Move forward as white, and backward as black.

            // Single Pawn Push
            int targetSquare = piecePosition + direction;
            if (board.Squares[targetSquare] == 0 && BoardUtility.IsValidTileCoordinate(targetSquare)) {
                moves.Add(new Move(piecePosition, targetSquare));
            }

            // Double Push Pawn
            targetSquare += direction;
            if (board.Squares[targetSquare] == 0 && board.Squares[targetSquare - direction] == 0 && BoardUtility.IsValidTileCoordinate(targetSquare)) {
                // Check that piece is in starting position before a double push
                if (isPieceWhite && BoardUtility.SECOND_ROW.Contains(piecePosition)) {  // Check piece is white an is located in the second row
                    moves.Add(new Move(piecePosition, targetSquare, Move.PawnTwoUpFlag));
                } else if (!isPieceWhite && BoardUtility.SEVENTH_ROW.Contains(piecePosition)) { // If not white, check if piece is located in the seventh row.
                    moves.Add(new Move(piecePosition, targetSquare));
                }
            }

            // Pawn Attacks
            int attackLeft = piecePosition + 7 * (isPieceWhite ? 1 : -1);
            int attackRight = piecePosition + 9 * (isPieceWhite ? 1 : -1);
            
            if (board.Squares[attackLeft] != 0 && BoardUtility.IsValidTileCoordinate(attackLeft) && !(BoardUtility.FIRST_COLUMN.Contains(piecePosition) && (attackLeft - piecePosition == 7 || attackLeft - piecePosition == -9))) {
                if (!Piece.IsColor(piece, Piece.PieceColor(board.Squares[attackLeft]))) { // Check if attack is on an opponent piece
                    moves.Add(new Move(piecePosition, attackLeft));
                }
            }

            
            if (board.Squares[attackRight] != 0 && BoardUtility.IsValidTileCoordinate(attackRight) && (!BoardUtility.EIGHTH_COLUMN.Contains(piecePosition) && (attackRight - piecePosition == -7 || attackRight - piecePosition == 9))) {
                if (!Piece.IsColor(piece, Piece.PieceColor(board.Squares[attackRight]))) {
                    moves.Add(new Move(piecePosition, attackRight));
                }
            }

            // TODO enpassant

            return moves;
        }
    }
}
