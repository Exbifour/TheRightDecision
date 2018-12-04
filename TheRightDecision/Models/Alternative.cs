using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models
{
    public class Alternative
    {
        public int AlternativeId { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<Result> Results { get; set; }
        public ICollection<Vector> Vectors { get; set; }
    }
}
