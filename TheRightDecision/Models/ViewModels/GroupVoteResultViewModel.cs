using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheRightDecision.Models.ViewModels
{
    public class GroupVoteResultViewModel
    {
        public string WinnerName { get; set; }
        public int Points { get; set; }
        public List<GroupVoteResultModelElement> Details { get; set; }
    }

    public class GroupVoteResultModelElement
    {
        public string PersonName { get; set; }
        public List<string> SelectedAlternatives { get; set; }
    }
}
