using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackEnd;
using Figgle;

namespace Krankenhaus
{
    public class UserInterface
    {
        static Config con;
        private int dayCounter;
        private int prevDayTicker;
        public void Run()
        {
            MainMenu();
            con = new Config();
            Hospital testh = new Hospital(con);
            FileLogger fileLog = new FileLogger();
            testh.SendLogInfo += ShowLogStatus;
            testh.SendLogInfo += fileLog.WriteLogInfoToFile;
            //testh.SendLogInfo += TelegramSendMessage;

            TickerTimer test = new TickerTimer(con.TickSpeed);
            testh.SendFinalLog += test.OnSendFinalInfo;
            testh.SendFinalLog += ShowFinalInfo;
            test.SendTick += testh.RunAll;

            Thread.Sleep(1000);
            test.StartTimer();
            Console.ReadKey();
        }
        public async void ShowLogStatus(object sender, FileLogEventArgs e)
        {
            await Task.Run(() =>
            {
                dayCounter += 1;
                Console.CursorVisible = false;
                if (e.Dayticker == 1)
                {
                    Console.Clear();
                }
                int weekDayPosition = 60;
                int infoPosition = 50;
                int patientPosition = 1;
                int weekDisplayPosition = 25;
                string[] days = new string[7] { "[Monday],", "[Tuesday],", "[Wednesday],", "[Thursday],", "[Friday],", "[Saturday],", "[Sunday]" };
                if (prevDayTicker % 7 == 0)
                {
                    dayCounter = 1;
                    prevDayTicker = 1;
                }

                Console.SetCursorPosition(weekDisplayPosition, 1);
                for (int i = 0; i < days.Length; i++)
                {
                    if (i+1 == dayCounter)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"{days[i]} ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write($"{days[i]} ");
                    }
                }

                Console.SetCursorPosition(weekDayPosition, 3); Console.Write($"[Day: {e.Dayticker}]");
                Console.SetCursorPosition(infoPosition, 5); Console.Write($"Patients in Que: {e.TotalPatientsInQue.Count}     ");
                Console.SetCursorPosition(infoPosition, 6); Console.Write($"Patients in IVA: {e.TotalPatientsInIVA.Count}     ");
                Console.SetCursorPosition(infoPosition, 7); Console.Write($"Patients in Sanatorium: {e.TotalPatientsInSanatoriet.Count}     ");
                Console.SetCursorPosition(infoPosition, 8); Console.Write($"Total dead patients: {e.TotalAfterLifePatients.Count}     ");
                Console.SetCursorPosition(infoPosition, 9); Console.Write($"Total cured patients: {e.TotalCuredPatients.Count}     ");
                Console.SetCursorPosition(patientPosition, 10);
                Console.Write("------------------------------------------------------------------------------------------------------------------------------");
                Console.SetCursorPosition(infoPosition, 11);
                if (e.CurrDoctor != null) { Console.Write($"Current doctor on duty: {e.CurrDoctor.Name}                   "); }
                else { Console.Write("Current doctor on duty: No doctor                     "); }

                Console.SetCursorPosition(infoPosition, 12); Console.Write($"Total doctors used: {e.TotalExtraDoctorsUsed.Count}/{con.ExtraDoctors}\n\n");
                Console.SetCursorPosition(patientPosition, 13);
                Console.Write("------------------------------------------------------------------------------------------------------------------------------");
                Console.SetCursorPosition(weekDayPosition, 14); Console.Write("IVA:");
                Console.SetCursorPosition(patientPosition, 15);
                Console.Write("------------------------------------------------------------------------------------------------------------------------------");

                int lastPosition = 16;
                lastPosition = PrintPatients(con.IVASize,e.TotalPatientsInIVA, patientPosition, lastPosition);

                Console.SetCursorPosition(patientPosition, lastPosition);
                Console.Write("------------------------------------------------------------------------------------------------------------------------------");
                lastPosition++;
                Console.SetCursorPosition(weekDayPosition, lastPosition); Console.Write("Sanatorium:"); lastPosition++;
                Console.SetCursorPosition(patientPosition, lastPosition);
                Console.Write("------------------------------------------------------------------------------------------------------------------------------");
                lastPosition++;

                lastPosition = PrintPatients(con.SanSize,e.TotalPatientsInSanatoriet, patientPosition, lastPosition);

                prevDayTicker = e.Dayticker;
            });
        }
        public int PrintPatients(int size,List<Patient> clonedPatientList, int weekNumberPosition, int lastPosition)
        {
            for (int i = 0; i < size; i++) // clonedPatientList.Count
            {
                if (i < clonedPatientList.Count)
                {
                    StringBuilder str = new StringBuilder();
                    string healthBar = "";
                    for (int j = 0; j < clonedPatientList[i].SickLevel; j++)
                    {
                        str.Append("[X]");
                    }
                    healthBar = str.ToString();
                    string data = String.Format("{0,-30} {1,-20} {2,-10}",
                                                    clonedPatientList[i].Name, clonedPatientList[i].BirthDate, clonedPatientList[i].Arrival);
                    string data2 = String.Format("{0, 40}", healthBar);
                    string data3 = String.Format("{0, 20}", clonedPatientList[i].SickLevel);
                    Console.SetCursorPosition(weekNumberPosition, lastPosition); Console.Write(data);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(data2);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(data3);
                }
                else
                {
                    Console.SetCursorPosition(weekNumberPosition, lastPosition); 
                    Console.Write("                                                                                                                            ");
                }
                lastPosition++;
            }
            return lastPosition;
        }
        public void ShowFinalInfo(object sender, FileLogEventArgs e)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            string finalPrompt = @"
 ██ ▄█▀ ██▀███   ▄▄▄       ███▄    █  ██ ▄█▀▓█████  ███▄    █  ██░ ██  ▄▄▄       █    ██   ██████        
 ██▄█▒ ▓██ ▒ ██▒▒████▄     ██ ▀█   █  ██▄█▒ ▓█   ▀  ██ ▀█   █ ▓██░ ██▒▒████▄     ██  ▓██▒▒██    ▒        
▓███▄░ ▓██ ░▄█ ▒▒██  ▀█▄  ▓██  ▀█ ██▒▓███▄░ ▒███   ▓██  ▀█ ██▒▒██▀▀██░▒██  ▀█▄  ▓██  ▒██░░ ▓██▄          
▓██ █▄ ▒██▀▀█▄  ░██▄▄▄▄██ ▓██▒  ▐▌██▒▓██ █▄ ▒▓█  ▄ ▓██▒  ▐▌██▒░▓█ ░██ ░██▄▄▄▄██ ▓▓█  ░██░  ▒   ██▒       
▒██▒ █▄░██▓ ▒██▒ ▓█   ▓██▒▒██░   ▓██░▒██▒ █▄░▒████▒▒██░   ▓██░░▓█▒░██▓ ▓█   ▓██▒▒▒█████▓ ▒██████▒▒       
▒ ▒▒ ▓▒░ ▒▓ ░▒▓░ ▒▒   ▓▒█░░ ▒░   ▒ ▒ ▒ ▒▒ ▓▒░░ ▒░ ░░ ▒░   ▒ ▒  ▒ ░░▒░▒ ▒▒   ▓▒█░░▒▓▒ ▒ ▒ ▒ ▒▓▒ ▒ ░       
░ ░▒ ▒░  ░▒ ░ ▒░  ▒   ▒▒ ░░ ░░   ░ ▒░░ ░▒ ▒░ ░ ░  ░░ ░░   ░ ▒░ ▒ ░▒░ ░  ▒   ▒▒ ░░░▒░ ░ ░ ░ ░▒  ░ ░       
░ ░░ ░   ░░   ░   ░   ▒      ░   ░ ░ ░ ░░ ░    ░      ░   ░ ░  ░  ░░ ░  ░   ▒    ░░░ ░ ░ ░  ░  ░         
░  ░      ░           ░  ░         ░ ░  ░      ░  ░         ░  ░  ░  ░      ░  ░   ░           ░         
 █     █░ ▄▄▄        ██████      ██████  ██░ ██  █    ██ ▄▄▄█████▓   ▓█████▄  ▒█████   █     █░███▄    █ 
▓█░ █ ░█░▒████▄    ▒██    ▒    ▒██    ▒ ▓██░ ██▒ ██  ▓██▒▓  ██▒ ▓▒   ▒██▀ ██▌▒██▒  ██▒▓█░ █ ░█░██ ▀█   █ 
▒█░ █ ░█ ▒██  ▀█▄  ░ ▓██▄      ░ ▓██▄   ▒██▀▀██░▓██  ▒██░▒ ▓██░ ▒░   ░██   █▌▒██░  ██▒▒█░ █ ░█▓██  ▀█ ██▒
░█░ █ ░█ ░██▄▄▄▄██   ▒   ██▒     ▒   ██▒░▓█ ░██ ▓▓█  ░██░░ ▓██▓ ░    ░▓█▄   ▌▒██   ██░░█░ █ ░█▓██▒  ▐▌██▒
░░██▒██▓  ▓█   ▓██▒▒██████▒▒   ▒██████▒▒░▓█▒░██▓▒▒█████▓   ▒██▒ ░    ░▒████▓ ░ ████▓▒░░░██▒██▓▒██░   ▓██░
░ ▓░▒ ▒   ▒▒   ▓▒█░▒ ▒▓▒ ▒ ░   ▒ ▒▓▒ ▒ ░ ▒ ░░▒░▒░▒▓▒ ▒ ▒   ▒ ░░       ▒▒▓  ▒ ░ ▒░▒░▒░ ░ ▓░▒ ▒ ░ ▒░   ▒ ▒ 
  ▒ ░ ░    ▒   ▒▒ ░░ ░▒  ░ ░   ░ ░▒  ░ ░ ▒ ░▒░ ░░░▒░ ░ ░     ░        ░ ▒  ▒   ░ ▒ ▒░   ▒ ░ ░ ░ ░░   ░ ▒░
  ░   ░    ░   ▒   ░  ░  ░     ░  ░  ░   ░  ░░ ░ ░░░ ░ ░   ░          ░ ░  ░ ░ ░ ░ ▒    ░   ░    ░   ░ ░ 
    ░          ░  ░      ░           ░   ░  ░  ░   ░                    ░        ░ ░      ░            ░ 
                                                                      ░                                  
";
            Console.WriteLine(finalPrompt);
            Console.WriteLine($"\nTotal days the hospital was open: {e.Dayticker-1}");
            Console.WriteLine($"Total patients who successfully died: {e.TotalAfterLifePatients.Count}");
            Console.WriteLine($"Total patients who unfortunately survived: {e.TotalCuredPatients.Count}");
            Console.ReadKey();
            return;
        }
        //public async void TelegramSendMessage(object sender, FileLogEventArgs e)
        //{
        //    await Task.Run(() =>
        //    {
        //        string apilToken = "1601007760:AAGhNgn5iQJDYmePV1ifstUh8jYF8eNKaow";
        //        string destID = "-515639364";
        //        string text = $"Day: {e.Dayticker}\nPatients in Que: {e.TotalPatientsInQue.Count}";
        //        string urlString = $"https://api.telegram.org/bot{apilToken}/sendMessage?chat_id={destID}&text={text}";

        //        WebClient webclient = new WebClient();

        //        webclient.DownloadString(urlString);
        //    });
        //}
        public static void MainMenu()
        {
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Red;
            string menuPrompt = @"
 ██ ▄█▀ ██▀███   ▄▄▄       ███▄    █  ██ ▄█▀▓█████  ███▄    █  ██░ ██  ▄▄▄       █    ██   ██████                      
 ██▄█▒ ▓██ ▒ ██▒▒████▄     ██ ▀█   █  ██▄█▒ ▓█   ▀  ██ ▀█   █ ▓██░ ██▒▒████▄     ██  ▓██▒▒██    ▒                      
▓███▄░ ▓██ ░▄█ ▒▒██  ▀█▄  ▓██  ▀█ ██▒▓███▄░ ▒███   ▓██  ▀█ ██▒▒██▀▀██░▒██  ▀█▄  ▓██  ▒██░░ ▓██▄                        
▓██ █▄ ▒██▀▀█▄  ░██▄▄▄▄██ ▓██▒  ▐▌██▒▓██ █▄ ▒▓█  ▄ ▓██▒  ▐▌██▒░▓█ ░██ ░██▄▄▄▄██ ▓▓█  ░██░  ▒   ██▒                     
▒██▒ █▄░██▓ ▒██▒ ▓█   ▓██▒▒██░   ▓██░▒██▒ █▄░▒████▒▒██░   ▓██░░▓█▒░██▓ ▓█   ▓██▒▒▒█████▓ ▒██████▒▒                     
▒ ▒▒ ▓▒░ ▒▓ ░▒▓░ ▒▒   ▓▒█░░ ▒░   ▒ ▒ ▒ ▒▒ ▓▒░░ ▒░ ░░ ▒░   ▒ ▒  ▒ ░░▒░▒ ▒▒   ▓▒█░░▒▓▒ ▒ ▒ ▒ ▒▓▒ ▒ ░                     
░ ░▒ ▒░  ░▒ ░ ▒░  ▒   ▒▒ ░░ ░░   ░ ▒░░ ░▒ ▒░ ░ ░  ░░ ░░   ░ ▒░ ▒ ░▒░ ░  ▒   ▒▒ ░░░▒░ ░ ░ ░ ░▒  ░ ░                     
░ ░░ ░   ░░   ░   ░   ▒      ░   ░ ░ ░ ░░ ░    ░      ░   ░ ░  ░  ░░ ░  ░   ▒    ░░░ ░ ░ ░  ░  ░                       
░  ░      ░           ░  ░         ░ ░  ░      ░  ░         ░  ░  ░  ░      ░  ░   ░           ░                       
▓█████▄ ▓█████ ▄▄▄     ▄▄▄█████▓ ██░ ██      ██████  ██▓ ███▄ ▄███▓ █    ██  ██▓    ▄▄▄     ▄▄▄█████▓ ▒█████   ██▀███  
▒██▀ ██▌▓█   ▀▒████▄   ▓  ██▒ ▓▒▓██░ ██▒   ▒██    ▒ ▓██▒▓██▒▀█▀ ██▒ ██  ▓██▒▓██▒   ▒████▄   ▓  ██▒ ▓▒▒██▒  ██▒▓██ ▒ ██▒
░██   █▌▒███  ▒██  ▀█▄ ▒ ▓██░ ▒░▒██▀▀██░   ░ ▓██▄   ▒██▒▓██    ▓██░▓██  ▒██░▒██░   ▒██  ▀█▄ ▒ ▓██░ ▒░▒██░  ██▒▓██ ░▄█ ▒
░▓█▄   ▌▒▓█  ▄░██▄▄▄▄██░ ▓██▓ ░ ░▓█ ░██      ▒   ██▒░██░▒██    ▒██ ▓▓█  ░██░▒██░   ░██▄▄▄▄██░ ▓██▓ ░ ▒██   ██░▒██▀▀█▄  
░▒████▓ ░▒████▒▓█   ▓██▒ ▒██▒ ░ ░▓█▒░██▓   ▒██████▒▒░██░▒██▒   ░██▒▒▒█████▓ ░██████▒▓█   ▓██▒ ▒██▒ ░ ░ ████▓▒░░██▓ ▒██▒
 ▒▒▓  ▒ ░░ ▒░ ░▒▒   ▓▒█░ ▒ ░░    ▒ ░░▒░▒   ▒ ▒▓▒ ▒ ░░▓  ░ ▒░   ░  ░░▒▓▒ ▒ ▒ ░ ▒░▓  ░▒▒   ▓▒█░ ▒ ░░   ░ ▒░▒░▒░ ░ ▒▓ ░▒▓░
 ░ ▒  ▒  ░ ░  ░ ▒   ▒▒ ░   ░     ▒ ░▒░ ░   ░ ░▒  ░ ░ ▒ ░░  ░      ░░░▒░ ░ ░ ░ ░ ▒  ░ ▒   ▒▒ ░   ░      ░ ▒ ▒░   ░▒ ░ ▒░
 ░ ░  ░    ░    ░   ▒    ░       ░  ░░ ░   ░  ░  ░   ▒ ░░      ░    ░░░ ░ ░   ░ ░    ░   ▒    ░      ░ ░ ░ ▒    ░░   ░ 
   ░       ░  ░     ░  ░         ░  ░  ░         ░   ░         ░      ░         ░  ░     ░  ░            ░ ░     ░     
 ░                                                                                                                     
 A project by: Fredrik Parkell, Olof Svahn, Stefan Trenh, Louis Headlam";
            Console.SetCursorPosition(50, 1);
            Console.WriteLine(menuPrompt);
            Console.WriteLine();
            Console.ResetColor();
            Console.Write("Press "); Console.ForegroundColor = ConsoleColor.Red; Console.Write("<Enter>"); Console.ResetColor(); Console.Write(" to start the simulation...");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.SetCursorPosition(0, 35); Console.Write("To change settings for the simulation go to the config.txt-file:");
            Console.ResetColor(); Console.SetCursorPosition(0, 36); Console.Write(@"Krankenhaus\Krankenhaus\bin\Debug\net5.0");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.SetCursorPosition(0, 37); Console.Write("And to check each simulations Hospital Logs go to:");
            Console.ResetColor(); Console.SetCursorPosition(0, 38); Console.Write(@"Krankenhaus\Krankenhaus\bin\Debug\net5.0\Hospital Log Files");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
            for (int i = 0; i <= 100; i++)
            {
                Thread.Sleep(50);
                Console.SetCursorPosition(0, 25);
                Console.Write("Loading: {0}% ", i);
                while (i == 21 || i == 46 || i == 58)
                {
                    Thread.Sleep(1000);
                    Console.Write("Spreading the disease..");
                    break;
                }
                while (i == 59 || i == 99)
                {
                    Thread.Sleep(1000);
                    Console.Write("Crippling the doctors..");
                    break;
                }
            }
            Console.Write("Complete! Starting..");
        }
    }
}