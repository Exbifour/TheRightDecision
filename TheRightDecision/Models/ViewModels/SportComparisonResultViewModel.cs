using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models.ViewModels
{
    public class SportComparisonResultViewModel
    {
        public int AlternativeId { get; set; }
        public string AlternativeName { get; set; }
        public int Points { get; set; }
        public bool IsOneOfWinners { get; set; }
        public List<int> WonNextAlternatives { get; set; }
    }
}
