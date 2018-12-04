using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models
{
    public class Criterion
    {
        public int CriterionId { get; set; }
        [Required]
        public string Name { get; set; }
        public int? Rank { get; set; }
        public int? Weight { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        [Display(Name = "Optimality type")]
        public string OptimalityType { get; set; }
        public string Units { get; set; }
        [Required]
        public string Scale { get; set; }

        public ICollection<Mark> Marks { get; set; }
    }
}
