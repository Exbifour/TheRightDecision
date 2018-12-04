﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models.ViewModels
{
    public class SportComparisonVotingViewModel
    {
        public IEnumerable<SportComparisonVotingElement> Alternatives { get; set; }
    }

    public class SportComparisonVotingElement
    {
        public int AlternativeId { get; set; }
        public string AlternativeName { get; set; }
        public IDictionary<string, string> Marks { get; set; }
    }
}
