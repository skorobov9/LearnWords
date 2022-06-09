using System;

namespace LearnForeignWords.Models
{
    public class Player
    {
  
        public string Name { get; set; }

        public Player Opponent { get; set; }


        public TestType GameType { get; set; }

 
        public bool IsPlaying { get; set; }

        public bool IsEndgame { get; set; }

        public int? CollectionId { get; set; } = null;
   
        public bool IsSearchingOpponent { get; set; }

 
        public DateTime RegisterTime { get; set; }


        public string ConnectionId { get; set; }

        public Test Test { get; set; }
    }
}
