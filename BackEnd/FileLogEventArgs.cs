using System;
using System.Collections.Generic;

namespace BackEnd
{ 
    public class FileLogEventArgs : EventArgs
    {
        /*
        Stefan: Class inherted with Eventargs to raise events for the subscriber.
        Used six propertys of intergers datatypes with "counters as solution".
        The datetime datatype is used to log the current time from the local machine.
         */
       
        public List<Patient> TotalPatientsInQue { get; set; }
        public List<Patient> TotalPatientsInIVA { get; set; }
        public List<Patient> TotalPatientsInSanatoriet { get; set; }
        public List<Patient> TotalAfterLifePatients { get; set; }
        public List<Patient> TotalCuredPatients { get; set; }
        public List<ExtraDoctor> TotalExtraDoctorsUsed { get; set; }
        public DateTime StartDate { get; set; }
        public int Dayticker { get; set; }
        public ExtraDoctor CurrDoctor { get; set; }
        public FileLogEventArgs(DateTime startDate, List<Patient> totalCuredPatients, List<Patient> totalAfterLifePatients,
            int dayTicker)
        {
            this.StartDate = startDate;
            this.TotalAfterLifePatients = totalAfterLifePatients;
            this.TotalCuredPatients = totalCuredPatients;
            this.Dayticker = dayTicker;
        }
        public FileLogEventArgs(DateTime startDate,List<Patient> totalPatientsInQue, List<Patient> totalPatientsInIVA, 
            List<Patient> totalPatientsInSanatoriet, List<Patient> totalAfterLifePatients, 
            List<Patient> totalCuredPatients, List<ExtraDoctor> totalExtraDoctorsUsed, int dayTicker, ExtraDoctor currentDoctor)
        {
            this.StartDate = startDate;
            this.TotalPatientsInQue = totalPatientsInQue;
            this.TotalPatientsInIVA = totalPatientsInIVA;
            this.TotalPatientsInSanatoriet = totalPatientsInSanatoriet;
            this.TotalAfterLifePatients = totalAfterLifePatients;
            this.TotalCuredPatients = totalCuredPatients;
            this.TotalExtraDoctorsUsed = totalExtraDoctorsUsed;
            this.Dayticker = dayTicker;
            this.CurrDoctor = currentDoctor;
        }
    }
}