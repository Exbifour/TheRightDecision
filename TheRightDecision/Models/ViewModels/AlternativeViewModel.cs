using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models.ViewModels
{
    public class AlternativeViewModel
    {
        public Alternative Alternative { get; set; }

        public IEnumerable<Vector> Vectors { get; set; }
        public IEnumerable<Alternative> Alternatives { get; set; }
        public IEnumerable<Criterion> Criteria { get; set; }
    }
}
