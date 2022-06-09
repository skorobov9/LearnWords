using LearnForeignWords.Data;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace LearnForeignWords.Models
{
    public class Game
    {
        private readonly WordTestContext _context;


        public bool IsOver { get; private set; }



        public Player Player1 { get; set; }


        public Player Player2 { get; set; }

        public Game(Player player1, Player player2, int? CollectionId)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WordTestContext>();
            var options = optionsBuilder.UseSqlServer("workstation id=WordTest.mssql.somee.com;packet size=4096;user id=wild76_SQLLogin_1;pwd=UY76@ddAFTtzZat;data source=WordTest.mssql.somee.com;persist security info=False;initial catalog=WordTest").Options;
            _context = new WordTestContext(options);
            var collection = _context.Collections.Where(c => c.Id == CollectionId).FirstOrDefault();
            List<Word> words;
            if (collection == null)
            {
                words = _context.Words.Include(w => w.Collection).Where(w => w.Collection.OwnerId == null).ToList();
            }
            else words = _context.Words.Include(w => w.Collection).Where(w => w.Collection.Id == collection.Id).ToList();
            Player1 = player1;
            Player2 = player2;

            Test test1 = new Test() { TestType = player1.GameType };
            test1.CreateGameTest(words);
            Test test2 = new Test() { TestType = player2.GameType, Questions = new List<Question>() };
            test1.Questions.ForEach((item) =>
            {

                test2.Questions.Add((Question)item.Clone());
            });

            Player1.Test = test1;
            Player2.Test = test2;
        }
        public string CheckWinner()
        {
            string result = "";
            if (Player1.Test.CountRightAnswers > Player2.Test.CountRightAnswers)
            {
                result = $"{Player1.Name} выиграл";
            }
            if (Player1.Test.CountRightAnswers < Player2.Test.CountRightAnswers)
            {
                result = $"{Player2.Name} выиграл";
            }
            if (Player2.Test.CountRightAnswers == Player1.Test.CountRightAnswers)
            {

            }
            return result;
        }

    }
}
