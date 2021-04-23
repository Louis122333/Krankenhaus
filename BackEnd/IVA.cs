using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd
{
    public class IVA : IDepartment
    {
        private static readonly Random random = new Random();
        private List<Patient> patientList;
        internal List<Patient> PatientList
        {
            get { return patientList; }
        }
        private int getWorseRisk;
        private int remainChance;
        private int getBetterChance;
        public int GetWorseRisk 
        { 
            get 
            {
                getWorseRisk -= CurrentDoctor.CompetenceLevel;
                if (getWorseRisk < 0)
                {
                    getWorseRisk = 0;
                }
                return getWorseRisk; 
            } 
            set 
            { 
                getWorseRisk = value; 
            } 
        }
        public int RemainChance 
        { 
            get 
            { 
                return remainChance; 
            } 
            set 
            { 
                remainChance = value; 
            } 
        }
        public int GetBetterChance 
        { 
            get 
            {
                getBetterChance += CurrentDoctor.CompetenceLevel;
                return getBetterChance; 
            } 
            set 
            { 
                getBetterChance = value; 
            } 
        }
        public ExtraDoctor CurrentDoctor { get; internal set; }
        public IVA()
        {
            patientList = new List<Patient>();
            getWorseRisk = 10;
            remainChance = 20;
            getBetterChance = 70;
        }
        public async Task UpdateSickness()
        {
            decimal getBetterChanceWDoctor = 0;
            if (CurrentDoctor != null)
            {
                getBetterChanceWDoctor = (getBetterChance + CurrentDoctor.CompetenceLevel);
            }
            else
            {
                getBetterChanceWDoctor = getBetterChance;
            }

            await Task.Run(() =>
            {
                foreach (var patient in patientList)
                {
                    int randomNumber = random.Next(1, 101);

                    //räknar ut nya procentvärden baserat på nya get better-värdet
                    decimal remainder = (100 - (getBetterChanceWDoctor) / 3);
                    decimal newRemain = Math.Round(remainder * 2);
                    decimal newWorse = Math.Round(remainder);

                    if (randomNumber <= getBetterChanceWDoctor)
                    {
                        patient.SickLevel -= 1;
                    }
                    else if (randomNumber > getBetterChanceWDoctor 
                    && randomNumber <= getBetterChanceWDoctor + newRemain)
                    {
                        //nothing happens
                    }
                    else if (randomNumber > getBetterChanceWDoctor + newRemain)
                    {
                        patient.SickLevel += 1;
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
