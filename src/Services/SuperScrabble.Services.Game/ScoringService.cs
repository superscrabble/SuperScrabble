﻿using SuperScrabble.InputModels.Game;
using SuperScrabble.Services.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperScrabble.Services.Game
{
    public class ScoringService : IScoringService
    {
        public int CalculatePointsFromPlayerInput(WriteWordInputModel input, IBoard board, IEnumerable<WordBuilder> words)
        {
            throw new NotImplementedException();
        }
    }
}
