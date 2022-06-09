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
                 
                    Remove<Player>(ref players, playerWithoutGame);
                   
                }

                return null;
            }


            if (game != null)
            {
                Remove<Game>(ref games, game);
            }

           
            var player = game.Player1.ConnectionId == Context.ConnectionId ? game.Player1 : game.Player2;

            if (player == null)
            {
                return null;
            }

            
            Remove<Player>(ref players, player);

            
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
          
            this.Clients.Client(connectionId).SendAsync("registrationComplete");
        }

        public void FindOpponent()
        {
          
            var player = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
               
                return;
            }

      
            player.IsSearchingOpponent = true;

          
            var opponent = players.Where(x => x.ConnectionId != Context.ConnectionId && x.IsSearchingOpponent && !x.IsPlaying && x.GameType==player.GameType && x.CollectionId!=null).OrderBy(x => x.RegisterTime).FirstOrDefault();
            if (opponent == null)
            {
               
                Clients.Client(Context.ConnectionId).SendAsync("opponentNotFound");
                return;
            }

          
            player.IsPlaying = true;
            player.IsSearchingOpponent = false; 

            opponent.IsPlaying = true;
            opponent.IsSearchingOpponent = false;

        
            player.Opponent = opponent;
            opponent.Opponent = player;

          
            Clients.Client(Context.ConnectionId).SendAsync("opponentFound", opponent.Name);
            Clients.Client(opponent.ConnectionId).SendAsync("opponentFound", player.Name); 
            
            games.Add(new Game(player, opponent,opponent.CollectionId));
            var game = games?.FirstOrDefault(x => x.Player1.ConnectionId == Context.ConnectionId || x.Player2.ConnectionId == Context.ConnectionId);

          
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
           
          
            



        }
        public void CreateGame(string CollectionId)
        {
           
            var player = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
               
                return;
            }
            player.CollectionId = Convert.ToInt32(CollectionId);
          
            player.IsSearchingOpponent = true;

          
            var opponent = players.Where(x => x.ConnectionId != Context.ConnectionId && x.IsSearchingOpponent && !x.IsPlaying && x.GameType == player.GameType && x.CollectionId==null).OrderBy(x => x.RegisterTime).FirstOrDefault();
            if (opponent == null)
            {
                Clients.Client(Context.ConnectionId).SendAsync("opponentNotFound");
                return;
            }

         
            player.IsPlaying = true;
            player.IsSearchingOpponent = false; 

            opponent.IsPlaying = true;
            opponent.IsSearchingOpponent = false;

        
            player.Opponent = opponent;
            opponent.Opponent = player;

        
            Clients.Client(Context.ConnectionId).SendAsync("opponentFound", opponent.Name);
            Clients.Client(opponent.ConnectionId).SendAsync("opponentFound", player.Name);

            games.Add(new Game(player, opponent, player.CollectionId));
            var game = games?.FirstOrDefault(x => x.Player1.ConnectionId == Context.ConnectionId || x.Player2.ConnectionId == Context.ConnectionId);

       
            var question = game.Player1.Test.GetNextQuestion();
            var question2 = game.Player2.Test.GetNextQuestion();
            if (opponent.GameType == TestType.WordTranslation)
            {
                Clients.Client(Context.ConnectionId).SendAsync("question", question.Word.Name, question.Answers);
                Clients.Client(opponent.ConnectionId).SendAsync("question", question2.Word.Name, question2.Answers);
            }
            if (opponent.GameType == TestType.TranslationWord)
            {
                Clients.Client(Context.ConnectionId).SendAsync("question", question.Word.Meaning, question.Answers);
                Clients.Client(opponent.ConnectionId).SendAsync("question", question2.Word.Meaning, question2.Answers);
            }
         





        }

        public void NextQuestion()
        {
     
            var game = games?.FirstOrDefault(x => x.Player1.ConnectionId == Context.ConnectionId || x.Player2.ConnectionId == Context.ConnectionId);

            if (game == null || game.IsOver)
            {
     
                return;
            }

            var player = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {

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
            if(question == null)
            {
                player.IsEndgame = true;
                if (player.Opponent.IsEndgame)
                {
                    EndGame();
                }else
                Clients.Client(Context.ConnectionId).SendAsync("endwords");
            }else
            Clients.Client(Context.ConnectionId).SendAsync("question", question.Word.Name, question.Answers);

        }


        public void Answer(string answer)
        {
            
            var game = games?.FirstOrDefault(x => x.Player1.ConnectionId == Context.ConnectionId || x.Player2.ConnectionId == Context.ConnectionId);

            if (game == null || game.IsOver)
            {
              
                return;
            }

            var player = players.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (player == null)
            {
                
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
