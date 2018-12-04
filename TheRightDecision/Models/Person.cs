using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        [Required]
        public string Name { get; set; }
        public int? Rank { get; set; }

        public ICollection<Result> Results { get; set; }
    }
}
