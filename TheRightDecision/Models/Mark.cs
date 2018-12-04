using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models
{
    public class Mark
    {
        public int MarkId { get; set; }
        public int CriterionId { get; set; }
        [Required]
        public string Name { get; set; }
        public int Rank { get; set; }
        [Display(Name = "Value")]
        public double Number { get; set; }
        [Display(Name = "Normalized")]
        public double Normalized { get; set; }

        public Criterion Criterion { get; set; }
        public ICollection<Vector> Vectors { get; set; }
    }
}
