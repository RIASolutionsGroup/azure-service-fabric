using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingWeb.Models
{
    public class CharacterViewModel
    {
        public string Name { get; set; }

        public string RealName { get; set; }

        public string Origin { get; set; }

        public string Path { get; set; }

        public int Votes { get; set; }
    }
}
