using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Concurrent;
using LearnForeignWords.Models;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR.Hubs;

namespace LearnForeignWords
{
    public class GameHub : Hub
    {

        private static  ConcurrentBag<Player> players = new ConcurrentBag<Player>();


        private static  ConcurrentBag<Game> games = new ConcurrentBag<Game>();




        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {

            var game = games?.FirstOrDefault(j => j.Player1.ConnectionId == Context.ConnectionId || j.Player2.ConnectionId == Context.ConnectionId);
            if (game == null)
            {

                var playerWithoutGame = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if (playerWithoutGame != null)
                {
                    //// Remove this player from our player list.
                    Remove<Player>(ref players, playerWithoutGame);
                   
                }

                return null;
            }


            if (game != null)
            {
                Remove<Game>(ref games, game);
            }

            //// Though we have removed the game from our list, we still need to notify the opponent that he has a walkover.
            //// If the current connection Id matches the player 1 connection Id, its him who disconnected else its player 2
            var player = game.Player1.ConnectionId == Context.ConnectionId ? game.Player1 : game.Player2;

            if (player == null)
            {
                return null;
            }

            //// Remove this player as he is disconnected and was in the game.
            Remove<Player>(ref players, player);

            //// Check if there was an opponent of the player. If yes, tell him, he won/ got a walk over.
            if (player.Opponent != null)
            {
                return OnOpponentDisconnected(player.Opponent.ConnectionId, player.Name);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task OnOpponentDisconnected(string connectionId, string playerName)
        {
            return Clients.Client(connectionId).SendAsync("opponentDisconnected", playerName);
        }

        public void RegisterPlayer(string name,int gameType)
        {

            var player = players?.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
                player = new Player { ConnectionId = Context.ConnectionId, Name = name, GameType= (TestType)gameType , IsPlaying = false, IsSearchingOpponent = false, RegisterTime = DateTime.UtcNow};
                if (!players.Any(j => j.Name == name))
                {
                    players.Add(player);
                }
            }
            else
            {
                player.IsPlaying = false;
                player.IsSearchingOpponent = false;
            }
            this.OnRegisterationComplete(Context.ConnectionId);
        }
        public void OnRegisterationComplete(string connectionId)
        {
            //// Notify this connection id that the registration is complete.
            this.Clients.Client(connectionId).SendAsync("registrationComplete");
        }

        public void FindOpponent()
        {
            //// First fetch the player from our players collection having current connection id
            var player = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
                //// Since player would be registered before making this call,
                //// we should not reach here. If we are here, something somewhere in the flow above is broken.
                return;
            }

            //// Set that player is seraching for opponent.
            player.IsSearchingOpponent = true;

            //// We will follow a queue, so find a player who registered earlier as opponent. 
            //// This would only be the case if more than 2 players are looking for opponent.
            var opponent = players.Where(x => x.ConnectionId != Context.ConnectionId && x.IsSearchingOpponent && !x.IsPlaying && x.GameType==player.GameType).OrderBy(x => x.RegisterTime).FirstOrDefault();
            if (opponent == null)
            {
                //// Could not find any opponent, invoke opponentNotFound method in the client.
                Clients.Client(Context.ConnectionId).SendAsync("opponentNotFound");
                return;
            }

            //// Set both players as playing.
            player.IsPlaying = true;
            player.IsSearchingOpponent = false; //// Make him unsearchable for opponent search

            opponent.IsPlaying = true;
            opponent.IsSearchingOpponent = false;

            //// Set each other as opponents.
            player.Opponent = opponent;
            opponent.Opponent = player;

            //// Notify both players that they can play the game by invoking opponentFound method for both the players.
            //// Also pass the opponent name and opoonet image, so that they can visualize it.
            //// Here we are directly using connection id, but group is a good candidate and use here.
            Clients.Client(Context.ConnectionId).SendAsync("opponentFound", opponent.Name);
            Clients.Client(opponent.ConnectionId).SendAsync("opponentFound", player.Name); 
            
            games.Add(new Game(player, opponent));
            var game = games?.FirstOrDefault(x => x.Player1.ConnectionId == Context.ConnectionId || x.Player2.ConnectionId == Context.ConnectionId);

            //// Tell player that its his turn
            var question = game.Player1.Test.GetNextQuestion();  
            var question2 = game.Player2.Test.GetNextQuestion();
            if (opponent.GameType == TestType.WordTranslation)
            {
                Clients.Client(Context.ConnectionId).SendAsync("question", question.Word.Name, question.Answers);
                Clients.Client(opponent.ConnectionId).SendAsync("question", question2.Word.Name, question2.Answers);
            } 
            if(opponent.GameType == TestType.TranslationWord)
            {
                Clients.Client(Context.ConnectionId).SendAsync("question", question.Word.Meaning, question.Answers);
                Clients.Client(opponent.ConnectionId).SendAsync("question", question2.Word.Meaning, question2.Answers);
            }
            //    //// Tell opponent player to wait for the move
          
            



        }


        public void NextQuestion()
        {
            //// Lets find a game from our list of games where one of the player has the same connection Id as the current connection has.
            var game = games?.FirstOrDefault(x => x.Player1.ConnectionId == Context.ConnectionId || x.Player2.ConnectionId == Context.ConnectionId);

            if (game == null || game.IsOver)
            {
                //// No such game exist!
                return;
            }

            var player = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
                //// Since player would be registered before making this call,
                //// we should not reach here. If we are here, something somewhere in the flow above is broken.
                return;
            }
            Question question;
            if (game.Player1.ConnectionId == Context.ConnectionId)
            {
                question = game.Player1.Test.GetNextQuestion();
            }
            else
            {
                question = game.Player2.Test.GetNextQuestion();
            }
            Clients.Client(Context.ConnectionId).SendAsync("question", question.Word.Name, question.Answers);

        }


        public void Answer(string answer)
        {
            //// Lets find a game from our list of games where one of the player has the same connection Id as the current connection has.
            var game = games?.FirstOrDefault(x => x.Player1.ConnectionId == Context.ConnectionId || x.Player2.ConnectionId == Context.ConnectionId);

            if (game == null || game.IsOver)
            {
                //// No such game exist!
                return;
            }

            var player = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
                //// Since player would be registered before making this call,
                //// we should not reach here. If we are here, something somewhere in the flow above is broken.
                return;
            }
            

            Clients.Client(Context.ConnectionId).SendAsync("answerresult", player.Test.CheckAnswer(answer));

        }
        public void EndGame()
        {
            var game = games?.FirstOrDefault(x => x.Player1.ConnectionId == Context.ConnectionId || x.Player2.ConnectionId == Context.ConnectionId);  
            if (game == null)
                return;
            Player player;
            Player opponent;
            if (game.Player1.ConnectionId == Context.ConnectionId)
            {
                player = game.Player1;
                opponent = game.Player2;
            }
            else
            {
                player = game.Player2;
                opponent = game.Player1;
            }
            Clients.Client(player.ConnectionId).SendAsync("gameover",player.Test.GetAnswers(), opponent.Test.GetAnswers(),game.CheckWinner());
            game.Player1.IsPlaying = false;
            game.Player1.Opponent.IsPlaying = false;
        }
        private void Remove<T>(ref ConcurrentBag<T> players, T playerWithoutGame)
        {
            players = new ConcurrentBag<T>(players?.Except(new[] { playerWithoutGame }));

        }



    }
}
