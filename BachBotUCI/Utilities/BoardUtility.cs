using BachBot.ChessLogic;
using BachBotUCI.ChessLogic;

namespace BachBotUCI.Utilities {
    public static class BoardUtility {
        public const string fileNames = "abcdefgh";
        public const string rankNames = "12345678";

        // These are used to find exempt moves in the Move Generator class.
        public static int[] FIRST_COLUMN = { 0, 8, 16, 24, 32, 40, 48, 56 };
        public static int[] SECOND_COLUMN = { 1, 9, 17, 25, 33, 41, 49, 57 };
        public static int[] SEVENTH_COLUMN = { 6, 14, 22, 30, 38, 46, 54, 62 };
        public static int[] EIGHTH_COLUMN = { 7, 15, 23, 31, 39, 47, 55, 63 };

        public static int[] SECOND_ROW = { 8, 9, 10, 11, 12, 13, 14, 15 };
        public static int[] SEVENTH_ROW = { 48, 49, 50, 51, 52, 53, 54, 55 };

        // These are used to find a rook startposition
        public static int[] ROOK_START_POSITIONS = { 0, 7, 56, 63 };

        public static int RankIndex(int squareIndex) {
            // Since a chess board is a beautiful power of two, this works.
            return squareIndex >> 3;
        }

        public static int FileIndex(int squareIndex) {
            // Since a chess board is a beautiful power of two, this works.
            return squareIndex & 0b000111;
        }

        public static int IndexFromCoord(int fileIndex, int rankIndex) {
            return rankIndex * 8 + fileIndex;
        }

        public static int SquareIndexFromName(string name) {
            char fileName = name[0];
            char rankName = name[1];
            int fileIndex = fileNames.IndexOf(fileName);
            int rankIndex = rankNames.IndexOf(rankName);
            return IndexFromCoord(fileIndex, rankIndex);
        }

        public static string SquareNameFromCoordinate(int fileIndex, int rankIndex) {
            return fileNames[fileIndex] + "" + (rankIndex + 1);
        }

        public static Move GetMoveFromUCI(string UCIMove) {
            string startIndex = UCIMove.Substring(0, 2);
            string endIndex = UCIMove.Substring(2);

            int startSquare = SquareIndexFromName(startIndex);
            int targetSquare = SquareIndexFromName(endIndex);

            return new Move(startSquare, targetSquare);
        }


        public static string GetMoveNameUCI(Move move) {
            string startSquare = "";
            string targetSquare = "";

            startSquare = SquareNameFromCoordinate(move.StartSquare % 8, move.StartSquare / 8);
            targetSquare = SquareNameFromCoordinate(move.TargetSquare % 8, move.TargetSquare / 8);

            return startSquare + targetSquare;
        }

        public static string CreateDiagram(Board board, bool blackAtTop = true, bool includeFen = true) {
            System.Text.StringBuilder result = new();

            for (int y = 0; y < 8; y++) {
                int rankIndex = blackAtTop ? 7 - y : y;
                result.AppendLine("+---+---+---+---+---+---+---+---+");

                for (int x = 0; x < 8; x++) {
                    int fileIndex = blackAtTop ? x : 7 - x;
                    int squareIndex = IndexFromCoord(fileIndex, rankIndex);
                    int piece = board.Squares[squareIndex];
                    result.Append($"|({Piece.GetSymbol(piece)})");
                    if (x == 7) {
                        result.AppendLine($"| {rankIndex + 1}");
                    }
                }

                if (y == 7) {
                    result.AppendLine("+---+---+---+---+---+---+---+---+");
                    const string fileNames = "  a   b   c   d   e   f   g   h  ";
                    const string fileNamesRev = "  h   g   f   e   d   c   b   a  ";
                    result.AppendLine(blackAtTop ? fileNames : fileNamesRev);
                    result.AppendLine();

                    if (includeFen) {
                        result.AppendLine($"Fen         : {FenUtility.CurrentFen(board)}");
                    }
                }
            }

            return result.ToString();
        }

        public static bool IsValidTileCoordinate(int squareIndex) => squareIndex >= 0 && squareIndex < 63;
    }
}
