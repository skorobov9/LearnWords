using System;

namespace LearnForeignWords.Models
{
    public class Player
    {
        /// <summary>
        ///  Gets or sets the name of the player. This would be set at the time user registers.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  Gets or sets the opponent player. The player against whom the player would be playing.
        ///  This is determined/ set when the players click Find Opponent Button in the UI.
        /// </summary>
        public Player Opponent { get; set; }


        public TestType GameType { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the player is playing.
        ///  This is set when the player starts a game.
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the player is waiting for opponent to make a move.
        /// </summary>
        public bool WaitingForMove { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the player is searching for opponent.
        /// </summary>
        public bool IsSearchingOpponent { get; set; }

        /// <summary>
        /// Gets or sets the time when the player registered.
        /// </summary>
        public DateTime RegisterTime { get; set; }



        /// <summary>
        /// Gets or sets the connection id of the player connection with the gameHub.
        /// </summary>
        public string ConnectionId { get; set; }

        public Test Test { get; set; }
    }
}
