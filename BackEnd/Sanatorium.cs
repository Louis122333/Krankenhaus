using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd
{
    public class Sanatorium : IDepartment
    {
        private static readonly Random random = new Random();
        private List<Patient> patientList;
        public List<Patient> PatientList
        {
            get { return patientList; }
        }
        public int GetWorseRisk { get; set; }
        public int RemainChance { get; set; }
        public int GetBetterChance { get; set; }
        public Sanatorium()
        {
            patientList = new List<Patient>();
            GetWorseRisk = 35; //35
            RemainChance = 30; //30
            GetBetterChance = 35; //35
        }
        public async Task UpdateSickness()
        {
            await Task.Run(() =>
            {
                foreach (var patient in patientList)
                {
                    int randomNumber = random.Next(1, 101);

                    //updates patients sick level
                    if (randomNumber <= GetWorseRisk)
                    {
                        patient.SickLevel += 1;
                    }
                    else if (randomNumber >= GetWorseRisk + RemainChance)
                    {
                        patient.SickLevel -= 1;
                    }

                    //updates patients to dead or cured
                    if (patient.SickLevel == 10)
                    {
                        patient.IsDead = true;
                    }
                    else if (patient.SickLevel == 0)
                    {
                        patient.IsCured = true;
                    }
                }
            });
        }
        public IEnumerator<Patient> GetEnumerator()
        {
            return patientList.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return patientList.GetEnumerator();
        }
        public object Clone()
        {
            var clonedQueue = new List<Patient>();
            foreach (var patient in PatientList)
            {
                var clonedPatient = new Patient();
                clonedPatient = patient;
                clonedQueue.Add(clonedPatient);
            }
            return clonedQueue;
        }

    }
}
