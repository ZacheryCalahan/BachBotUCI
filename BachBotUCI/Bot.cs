﻿using BachBot.ChessLogic;
using BachBotUCI.ChessLogic;
using BachBotUCI.Utilities;

namespace BachBotUCI {
    public class Bot {
        public Board board;
        public event Action<string>? OnMoveChosen;

        public Bot() {
            board = new Board();
        }

        public void ChooseMove() { // Generates a random move.
            List<Move> moves = MoveGenerator.GenerateLegalMoves(board);
            var rand = new Random();
            OnSearchComplete(moves[rand.Next(moves.Count)]);
        }

        public void SetPosition(string fen) { 
            board.LoadPosition(fen);
        }

        public void SetPosition() {
            board.LoadPosition();
        }


        public void MakeMove(string UCIMove) {
            Move move = BoardUtility.GetMoveFromUCI(UCIMove);
            board.MakeMove(move);
        }

        public void OnSearchComplete(Move move) {
            string moveName = BoardUtility.GetMoveNameUCI(move);
            OnMoveChosen?.Invoke(moveName);
        }


    }
}
