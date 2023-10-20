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
        public static int[] squares = new int[64];
        public static bool IsWhitesTurn;

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
                    int piece = board.squares[i];
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
            fen += (board.IsWhiteToPlay()) ? 'w' : 'b';

            // TODO implement castling rights, en passant, and draw conditions.

            return fen;
        }

        public readonly struct PositionInfo {
            public readonly string fen;

            // Board Representation
            public readonly ReadOnlyCollection<int> squares;

            // Who to move
            public readonly bool whiteToMove;

            // TODO Castling Rights
            
            // TODO Draw conditions (50 move, repetition count, etc.)

            // TODO Enpassant condtions

            public PositionInfo(string fen) {
                this.fen = fen;
                int[] squarePieces = new int[64];

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

                // TODO castling rights, enpassant file, and draw conditions.
            }


        }


    }
}
