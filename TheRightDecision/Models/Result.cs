using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public int? PersonId { get; set; }
        public int AlternativeId { get; set; }
        public int Rank { get; set; }
        [Display(Name = "Weight")]
        public double AlternativeWeight { get; set; }

        public Person Person { get; set; }
        public Alternative Alternative { get; set; }
    }
}
