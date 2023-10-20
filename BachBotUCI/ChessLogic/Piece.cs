/**
 *  Piece class that graciously borrows from Sebastion Lague.
 *  https://www.youtube.com/@SebastianLague
 */

namespace BachBotUCI.ChessLogic {
    public static class Piece {
        // Piece Type
        public const int None = 0;
        public const int Pawn = 1;
        public const int King = 2;
        public const int Knight = 3;
        public const int Bishop = 4;
        public const int Rook = 5;
        public const int Queen = 6;

        // Colors
        public const int White = 0;
        public const int Black = 8;

        // Masks
        const int typeMask = 0b0111;
        const int colorMask = 0b1000;

        // Pieces
        public const int WhitePawn = Pawn | White;
        public const int WhiteKing = King | White;
        public const int WhiteKnight = Knight | White;
        public const int WhiteBishop = Bishop | White;
        public const int WhiteRook = Rook | White;
        public const int WhiteQueen = Queen | White;

        public const int BlackPawn = Pawn | Black;
        public const int BlackKing = King | Black;
        public const int BlackKnight = Knight | Black;
        public const int BlackBishop = Bishop | Black;
        public const int BlackRook = Rook | Black;
        public const int BlackQueen = Queen | Black;

        // Move Capability
        public static int[] orthogonalOffsets = { 8, 1, -1, -8 };
        public static int[] diagonalOffsets = { -9, -7, 7, 9 };
        public static int[] knightOffsets = { 17, 15, 10, 6, -6, -10, -15, -17 };
        public static int[] kingOffsets = { -9, -8, -7, -1, 1, 7, 8, 9 }; // This is a bit redundant, but for the price of memory I don't mind the cleanliness.

        public static int MakePiece(int pieceType, int pieceColor) => pieceType | pieceColor;
        public static int MakePiece(int pieceType, bool pieceIsWhite) => MakePiece(pieceType, pieceIsWhite ? White : Black);
        public static bool IsColor(int piece, int color) => ((piece & colorMask) == color) && piece != 0;
        public static bool IsColor(int piece, bool pieceIsWhite) => ((piece & colorMask) == (pieceIsWhite ? White : Black)) && piece != 0;
        public static bool IsWhite(int piece) => IsColor(piece, White);
        public static int PieceColor(int piece) => piece & colorMask;
        public static int PieceType(int piece) => piece & typeMask;
        public static bool IsOrthogonalSlider(int piece) => PieceType(piece) is Queen or Rook;
        public static bool IsDiagonalSlider(int piece) => PieceType(piece) is Bishop or Queen;
        public static bool IsSlidingPiece(int piece) => PieceType(piece) is Queen or Bishop or Rook;
        public static char GetSymbol(int piece) {
            int pieceType = PieceType(piece);
            char symbol = pieceType switch {
                Rook => 'R',
                Knight => 'N',
                Bishop => 'B',
                Queen => 'Q',
                King => 'K',
                Pawn => 'P',
                _ => ' '
            };
            symbol = IsWhite(piece) ? symbol : char.ToLower(symbol);
            return symbol;
        }
        public static int GetPieceTypeFromSymbol(char symbol) {
            symbol = char.ToUpper(symbol);
            return symbol switch {
                'R' => Rook,
                'N' => Knight,
                'B' => Bishop,
                'Q' => Queen,
                'K' => King,
                'P' => Pawn,
                _ => None
            };
        }


    }
}
