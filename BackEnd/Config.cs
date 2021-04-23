using System.IO;

namespace BackEnd
{
   public class Config
    {
        private int que;
        public int Que
        {
            get { return que; }
        }

        private int ivaSize;
        public int IVASize
        {
            get { return ivaSize; }
        }

        private int sanSize;
        public int SanSize
        {
            get { return sanSize; }
        }

        private int extraDoctors;
        public int ExtraDoctors
        {
            get { return extraDoctors; }
        }

        private int tickSpeed;
        public int TickSpeed
        {
            get { return tickSpeed; }
        }

        public Config()
        {
            GetDataFromConfigFile();
            que = this.Que;
            ivaSize = this.IVASize;
            sanSize = this.SanSize;
            extraDoctors = this.ExtraDoctors;
            tickSpeed = this.TickSpeed;
        }
        public void GetDataFromConfigFile()
        {
            if (!File.Exists("config.txt"))
            {
                StreamWriter createConfigFile = new StreamWriter("config.txt");
                using (createConfigFile)
                {
                    createConfigFile.WriteLine("You can only choose between low,medium,high as :Level");
                    createConfigFile.WriteLine("Hospital Size:medium");
                    createConfigFile.WriteLine("Ticker Speed:medium");
                }
            }
            StreamReader readConfigFile = new StreamReader("config.txt");
            
            using (readConfigFile)
            {
                string line = readConfigFile.ReadLine();
                while (line != null)
                {
                    string[] split = line.Split(':');
                    string[] levels = new string[] { "low", "medium", "high" };

                    if (split[0] == "Hospital Size")
                    {

                        if (levels[0] == split[1].ToLower())
                        {
                            que = 75;
                            ivaSize = 2;
                            sanSize = 5;
                            extraDoctors = 5;
                        }
                        else if (levels[1] == split[1].ToLower())
                        {
                            que = 150;
                            ivaSize = 5;
                            sanSize = 10;
                            extraDoctors = 10;
                        }
                        else if (levels[2] == split[1].ToLower())
                        {
                            que = 300;
                            ivaSize = 10;
                            sanSize = 20;
                            extraDoctors = 20;
                        }
                    }

                    if (split[0] == "Ticker Speed")
                    {
                        if (levels[0] == split[1].ToLower())
                        {
                            tickSpeed = 6000;
                        }
                        else if (levels[1] == split[1].ToLower())
                        {
                            tickSpeed = 4000;
                        }
                        else if (levels[2] == split[1].ToLower())
                        {
                            tickSpeed = 2000;
                        }
                    }
                    line = readConfigFile.ReadLine();

                }
            }
        }
    }
}
