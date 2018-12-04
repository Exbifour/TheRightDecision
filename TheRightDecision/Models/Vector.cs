using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models
{
    public class Vector
    {
        public int VectorId { get; set; }
        public int AlternativeId { get; set; }
        public int MarkId { get; set; }

        public Alternative Alternative { get; set; }
        public Mark Mark { get; set; }
    }
}
