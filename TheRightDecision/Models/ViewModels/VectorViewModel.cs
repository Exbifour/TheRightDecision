using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models.ViewModels
{
    public class VectorViewModel
    {
        public Alternative Alternative { get; set; }
        public Criterion Criterion { get; set; }
    }
}
