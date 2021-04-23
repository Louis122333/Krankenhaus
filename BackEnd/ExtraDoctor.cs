using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd
{
    public class ExtraDoctor
    {
        private static readonly Random random = new Random();
        public string Name { get; }
        private int exhaustedLevel;
        public int ExhaustedLevel
        {
            get
            {
                return exhaustedLevel;
            }
            internal set
            {
                exhaustedLevel = value;
                CompetenceLevel -= 5;
            }
        }
        public int CompetenceLevel { get; set; }
        public ExtraDoctor()
        {
            Name = NameGenerator();
            ExhaustedLevel = 0;
            CompetenceLevel = random.Next(-10, 30);
        }

        private string NameGenerator()
        {
            StringBuilder fullName = new StringBuilder();
            string[] firstNameArr = new string[] { "Fredrik", "Stefan", "Olof", "Louis", "Nils", "Johan", "Andreas", "Johannes", "David",
                                                   "Einar", "Emile", "Madelene", "Julia", "Paul", "Mattis", "Adam", "Klas", "Carl Fredrik",
                                                   "Albin", "Andrea", "Simon", "Ludvig" };
            string[] lastNameArr = new string[] { "Parkell", "Trenh", "Svahn", "Headlam" };

            fullName.Append("Dr. " + firstNameArr[random.Next(0, firstNameArr.Length)] + " ");
            fullName.Append(lastNameArr[random.Next(0, lastNameArr.Length)]);

            return fullName.ToString();
        }
    }
}
