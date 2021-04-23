using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd
{
    public interface IDepartment : IEnumerable<Patient>, ICloneable
    {
        public int GetWorseRisk { get; set; }
        public int RemainChance { get; set; }
        public int GetBetterChance { get; set; }
        public Task UpdateSickness();
    }
}
