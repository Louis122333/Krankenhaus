using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd
{
    public class Patient
    {
        private static readonly Random random = new Random();
        public string Name { get; }
        public string BirthDate { get; }
        public int SickLevel { get; internal set; }
        public bool IsDead { get; internal set; }
        public bool IsCured { get; internal set; }
        public int TickerCount { get; internal set; }
        public string Arrival { get; }
        public Patient()
        {
            Name = NameGenerator();
            BirthDate = BirthDateGenerator();
            SickLevel = random.Next(1, 10);
            IsDead = false;
            IsCured = false;
            TickerCount = 0;
            Arrival = DateTime.Now.ToString("MM/dd/yyyy");
        }

        private string NameGenerator()
        {
            StringBuilder fullName = new StringBuilder();
            string[] firstNameArr = new string[] { "Fredrik", "Stefan", "Olof", "Louis", "Nils", "Johan", "Andreas", "Johannes", "David",
                                                   "Einar", "Emile", "Madelene", "Julia", "Paul", "Mattis", "Adam", "Klas", "Carl Fredrik",
                                                   "Albin", "Andrea", "Simon", "Ludvig" }; 
            string[] lastNameArr = new string[] { "Parkell", "Trenh", "Svahn", "Headlam" }; 

            fullName.Append(firstNameArr[random.Next(0, firstNameArr.Length)] + " ");
            fullName.Append(lastNameArr[random.Next(0, lastNameArr.Length)]);

            return fullName.ToString();
        }
        private string BirthDateGenerator()
        {
            StringBuilder fullDate = new StringBuilder();
            string[] yearDateArr = new string[] { "1940", "1941", "1942", "1943", "1944", "1945", "1946", "1947", "1948", "1949",
                                               "1950", "1951", "1952", "1953", "1954", "1955", "1956", "1957", "1958", "1959",
                                               "1960", "1961", "1962", "1963", "1964", "1965", "1966", "1967", "1968", "1969",
                                               "1970", "1971", "1972", "1973", "1974", "1975", "1976", "1977", "1978", "1979",
                                               "1980", "1981", "1982", "1983", "1984", "1985", "1986", "1987", "1988", "1989",
                                               "1990", "1991", "1992", "1993", "1994", "1995", "1996", "1997", "1998", "1999",
                                               "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009",
                                               "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019", "2020", "2021" };
            string[] monthDateArr = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
            string[] dayDateArr = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12",
                                              "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24",
                                              "25", "26", "27", "28", "29", "30", "31" };

            fullDate.Append(yearDateArr[random.Next(0, yearDateArr.Length)] + "-");
            fullDate.Append(monthDateArr[random.Next(0, monthDateArr.Length)] + "-");

            string monthCheck = fullDate.ToString();

            if (monthCheck.Contains("-02-")) 
                fullDate.Append(dayDateArr[random.Next(0, dayDateArr.Length - 3)]);
            else if (monthCheck.Contains("-04-") || monthCheck.Contains("-06-") || monthCheck.Contains("-09-") || monthCheck.Contains("-11-"))
                fullDate.Append(dayDateArr[random.Next(0, dayDateArr.Length - 1)]);
            else
                fullDate.Append(dayDateArr[random.Next(0, dayDateArr.Length)]);

            return fullDate.ToString();
        }
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            string healthBar = "";
            for (int i = 0; i < SickLevel; i++)
            {
                str.Append("[X]");
            }
            healthBar = str.ToString();
            string data = String.Format("{0,-30} {1,-20} {2,-10} {3, 40} {4, 20}",
                                            Name, BirthDate, Arrival, healthBar, SickLevel);
            return data;
        }
    }
}
