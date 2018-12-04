using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models.ViewModels
{
    public class ResultViewModel
    {
        public IEnumerable<ResultElement> ResultList { get; set; }
        public IEnumerable<Criterion> Criteria { get; set; }
    }

    public class ResultElement
    {
        public Alternative Alternative { get; set; }
        public double Weight { get; set; }
        public bool IsOneOfBest { get; set; }
    }
}
