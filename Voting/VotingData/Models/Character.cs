using System;
using System.Collections.Generic;
using System.Linq;

namespace VotingData.Models
{
    public class Character : ICloneable
    {
        public string Name { get; set; }

        public string RealName { get; set; }

        public string Origin { get; set; }

        public string Path { get; set; }

        public int Votes { get; set; }

        public object Clone()
        {
            return new Character()
            {
                Name = this.Name,
                RealName = this.RealName,
                Origin = this.Origin,
                Path = this.Path,
                Votes = this.Votes
            };
        }
    }
}
