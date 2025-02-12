﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class BoardGame
    {
        public int BoardGameId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public GameGenre Genre { get; set; }
        public bool Is18Plus { get; set; }
        public GameType GameType { get; set; }
        //picture of the game
        public byte[]? Image { get; set; }
        public virtual ICollection<BoardGameNight> BoardGameNights { get; set; } = new List<BoardGameNight>();


    }

    public enum GameGenre
    {
        Strategy,
        Party,
        Family,
        Adventure,
        Puzzle
    }

    public enum GameType
    {
        CardGame,
        BoardGame,
        DiceGame
    }

}
