using System;
using System.Linq;
using System.Threading;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace BackEnd
{
    public class FileLogger
    {
        public async void WriteLogInfoToFile(object sender, FileLogEventArgs e) 
        {
            await Task.Run(() =>
            {
                string dayMapInSubMap = AddDirectories(e);
                AddFiles(e, dayMapInSubMap);
            });
        }
        public string AddDirectories(FileLogEventArgs e)
        {
            string topMap = "Hospital Log Files";
            if (!Directory.Exists(topMap))
            {
                Directory.CreateDirectory(topMap); //Create top map
            }
            string subMap = Path.Combine(topMap, $"Hospital Run-{e.StartDate.ToString().Replace(':', '-')}");
            if (!Directory.Exists(subMap))
            {
                Directory.CreateDirectory(subMap); //Create sub map
            }
            string dayMapInSubMap = Path.Combine(subMap, $"Day- {e.Dayticker}");
            Directory.CreateDirectory(dayMapInSubMap);//Create day map
            return dayMapInSubMap;
        }
        public void AddFiles(FileLogEventArgs e, string dayMapInSubMap)
        {
            var loggArr = new string[5] { "QUEUE", "IVA", "SANATORIUM", "AFTERLIFE", "CURED" };
            for (int i = 0; i < loggArr.Length; i++)
            {
                string filePath = Path.Combine(dayMapInSubMap, $"Day {e.Dayticker} - {loggArr[i]}.txt");
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    if (i == 0) //if queue
                    {
                        sw.WriteLine($"Total patients in QUEUE: {e.TotalPatientsInQue.Count}\n\n" +
                            $"PATIENT LIST:");
                        foreach (var patient in e.TotalPatientsInQue)
                        {
                            sw.WriteLine(patient.ToString());
                        }
                    }
                    else if (i == 1) //if iva
                    {
                        sw.WriteLine($"Total patients in IVA: {e.TotalPatientsInIVA.Count}\n\n" +
                            $"PATIENT LIST:");
                        foreach (var patient in e.TotalPatientsInIVA)
                        {
                            sw.WriteLine(patient.ToString());
                        }
                    }
                    else if (i == 2) //if san
                    {
                        sw.WriteLine($"Total patients in SANATORIUM: {e.TotalPatientsInSanatoriet.Count}\n\n" +
                            $"PATIENT LIST:");
                        foreach (var patient in e.TotalPatientsInSanatoriet)
                        {
                            sw.WriteLine(patient.ToString());
                        }
                    }
                    else if (i == 3) //if afterlife
                    {
                        sw.WriteLine($"Total dead patients in AFTERLIFE: {e.TotalAfterLifePatients.Count}\n\n" +
                            $"PATIENT LIST:");
                        foreach (var patient in e.TotalAfterLifePatients)
                        {
                            sw.WriteLine(patient.ToString());
                        }
                    }
                    else if (i == 4) //if cured
                    {
                        sw.WriteLine($"Total cured patients: {e.TotalCuredPatients.Count}\n\n" +
                            $"PATIENT LIST:");
                        foreach (var patient in e.TotalCuredPatients)
                        {
                            sw.WriteLine(patient.ToString());
                        }
                    }
                }
            }
        }
    }
}