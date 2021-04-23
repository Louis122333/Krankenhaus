using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackEnd
{
    public class PatientQueue : IDepartment
    {
        private static readonly Random random = new Random();
        private Queue<Patient> patientList;
        private Queue<Patient> PatientListQueue
        {
            get { return patientList; }
            set { patientList = value; }
        }
        internal List<Patient> PatientList
        {
            get 
            {
                return patientList.ToList();
            }
            set 
            {
                Queue<Patient> pat = new Queue<Patient>(value);
                PatientListQueue = pat;
            }
        }
        public int GetWorseRisk { get; set; }
        public int RemainChance { get; set; }
        public int GetBetterChance { get; set; }
        public PatientQueue(int queue)
        {
            patientList = new Queue<Patient>();
            GetWorseRisk = 60;
            RemainChance = 35;
            GetBetterChance = 5;
            GeneratePatients(queue);
        }
        public void GeneratePatients(int que)
        {
            for (int i = 0; i < que; i++)
            {
                Patient patient = new Patient();
                patientList.Enqueue(patient);
            }
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
        internal Patient RemovePatient()
        {
            Patient pat = new Patient();
            pat = patientList.Dequeue();
            return pat;
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
            var clonedQueue = new Queue<Patient>();
            foreach (var patient in patientList)
            {
                var clonedPatient = new Patient();
                clonedPatient = patient;
                clonedQueue.Enqueue(clonedPatient);
            }
            return clonedQueue;
        }
    }
}
