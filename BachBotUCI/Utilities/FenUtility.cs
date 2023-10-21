using BachBotUCI.ChessLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BachBotUCI.Utilities {
    public static class FenUtility {
        public const string StartPositionFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public static PositionInfo loadFen(string fen = StartPositionFEN) {
            PositionInfo positionInfo = new PositionInfo(fen);
            return positionInfo;
        }

        public static string CurrentFen(Board board) {
            string fen = "";

            // Pieces on board
            for (int rank = 7; rank >= 0; rank--) {
                int numEmptyFiles = 0;
                for (int file = 0; file < 8; file++) {
                    int i = rank * 8 + file;
                    int piece = board.Squares[i];
                    if (piece != 0) {
                        if (numEmptyFiles != 0) {
                            fen += numEmptyFiles;
                            numEmptyFiles = 0;
                        }
                        bool isBlack = Piece.IsColor(piece, Piece.Black);
                        int pieceType = Piece.PieceType(piece);
                        char pieceChar = ' ';
                        switch (pieceType) {
                            case Piece.Rook:
                                pieceChar = 'R';
                                break;
                            case Piece.Knight:
                                pieceChar = 'K';
                                break;
                            case Piece.Bishop:
                                pieceChar = 'B';
                                break;
                            case Piece.Queen:
                                pieceChar = 'Q';
                                break;
                            case Piece.King:
                                pieceChar = 'K';
                                break;
                            case Piece.Pawn:
                                pieceChar = 'P';
                                break;
                        }
                        fen += (isBlack) ? pieceChar.ToString().ToLower() : pieceChar.ToString();
                    } else {
                        numEmptyFiles++;
                    }
                }
                if (numEmptyFiles != 0) {
                    fen += numEmptyFiles;
                }
                if (rank != 0) {
                    fen += '/';
                }
            }

            //Side to move
            fen += ' ';
            fen += (board.WhiteToMove) ? 'w' : 'b';

            // Castling Rights
            bool whiteKingside = (board.CurrentGameState.castlingRights & 1) == 1;
            bool whiteQueenside = (board.CurrentGameState.castlingRights >> 1 & 1) == 1;
            bool blackKingside = (board.CurrentGameState.castlingRights >> 2 & 1) == 1;
            bool blackQueenside = (board.CurrentGameState.castlingRights >> 3  & 1) == 1;
            fen += ' ';
            fen += (whiteKingside) ? "K" : "";
            fen += (whiteQueenside) ? "Q" : "";
            fen += (blackKingside) ? "k" : "";
            fen += (blackQueenside) ? "q" : "";
            fen += ((board.CurrentGameState.castlingRights) == 0) ? "-" : "";

            // Enpassant
            fen += ' ';
            int epFileIndex = board.CurrentGameState.enPassantFile - 1;
            int epRankIndex = (board.WhiteToMove) ? 5 : 2;

            bool isEnPassant = epFileIndex != -1;
            if (board.CurrentGameState.enPassantFile != 0) {
                fen += BoardUtility.fileNames.IndexOf(board.CurrentGameState.enPassantFile + 1 + "");
            } else {
                fen += "-";
            }

            // 50 Move Counter
            fen += ' ';
            fen += board.CurrentGameState.fiftyMoveCounter;

            // Total Move Counter
            fen += ' ';
            fen += (board.PlyCount / 2) + 1;

            return fen;
        }

        public readonly struct PositionInfo {
            public readonly string fen;

            // Board Representation
            public readonly ReadOnlyCollection<int> squares;

            // Gamestate
            public readonly bool whiteToMove;
            public readonly bool WhiteKingsideCastle = false;
            public readonly bool WhiteQueensideCastle = false;
            public readonly bool BlackKingsideCastle = false;
            public readonly bool BlackQueensideCastle = false;
            public readonly int epFile = 0;
            public readonly int fiftyMovePlyCount = 0;
            public readonly int moveCount = 0;

            public PositionInfo(string fen) {
                // Position Info
                this.fen = fen;
                int[] squarePieces = new int[64];
                
                // Get the pieces on the board
                string[] fenTokens = fen.Split(' ');
                
                int file = 0;
                int rank = 7;

                foreach (char symbol in fenTokens[0]) {
                    if (symbol == '/') {
                        file = 0;
                        rank--;
                    } else {
                        if (char.IsDigit(symbol)) {
                            file += (int)char.GetNumericValue(symbol);
                        } else {
                            int pieceColor = (char.IsUpper(symbol) ? Piece.White : Piece.Black);
                            int pieceType = char.ToLower(symbol) switch {
                                'k' => Piece.King,
                                'p' => Piece.Pawn,
                                'n' => Piece.Knight,
                                'b' => Piece.Bishop,
                                'r' => Piece.Rook,
                                'q' => Piece.Queen,
                                 _  => Piece.None
                            };
                            squarePieces[rank * 8 + file] = pieceType | pieceColor;
                            file++;
                        }
                    }
                }

                squares = new(squarePieces);
                whiteToMove = (fenTokens[1] == "w");

                // Castling Rights
                if (!fenTokens[2].Contains("-")) {
                    char[] castleChars = fenTokens[2].ToCharArray();
                    foreach (char castleChar in castleChars) {
                        switch (castleChar) {
                            case 'K':
                                WhiteKingsideCastle = true;
                                break;
                            case 'Q':
                                WhiteQueensideCastle = true;
                                break;
                            case 'k':
                                BlackKingsideCastle = true;
                                break;
                            case 'q':
                                BlackQueensideCastle = true;
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Enpassant File
                if (fenTokens.Length > 3) {
                    // Enpassant file
                    if (!fenTokens[3].Contains("-")) {
                        string enPassantFileName = fenTokens[3][0].ToString();
                        if (BoardUtility.fileNames.Contains(enPassantFileName)) {
                            epFile = BoardUtility.fileNames.IndexOf(enPassantFileName) + 1;
                        }
                    }
                }
                
                // 50 Move Rule
                if (fenTokens.Length > 4) {
                    int.TryParse(fenTokens[4], out fiftyMovePlyCount);
                }

                // Full Move Count
                if (fenTokens.Length > 5) {
                    int.TryParse(fenTokens[5], out moveCount);
                }
            }
        }
    }
}
