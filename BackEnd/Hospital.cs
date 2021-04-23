using BackEnd;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BackEnd
{
    public class Hospital
    {
        public event EventHandler<FileLogEventArgs> SendLogInfo;
        public event EventHandler<FileLogEventArgs> SendFinalLog;
        private static readonly Random random = new Random();
        private DateTime startDate;
        private int elaspedDays;
        private PatientQueue queueDep;
        private IVA ivaDep;
        private Sanatorium sanatoriumDep;
        private List<ExtraDoctor> availableDoctorsList;
        private List<ExtraDoctor> usedDoctors;
        private List<Patient> afterlifePatients;
        private List<Patient> curedPatients;
        internal Config Conn { get; set; }
        public IDepartment QueueDep { get { return queueDep; } } // PatientQueue
        public IDepartment IVADep { get { return ivaDep; } } // IVA
        public IDepartment SanatoriumDep { get { return sanatoriumDep; } } // Sanatorium
        public Hospital(Config config)
        {
            Conn = config;
            startDate = DateTime.Now;
            Task.Run(() => { availableDoctorsList = GenerateDoctors(); });
            usedDoctors = new List<ExtraDoctor>();

            afterlifePatients = new List<Patient>();
            curedPatients = new List<Patient>();

            Task.Run(() => { queueDep = new PatientQueue(Conn.Que); });
            Task.Run(() => { ivaDep = new IVA(); });
            Task.Run(() => { sanatoriumDep = new Sanatorium(); });
        }
        public async void RunAll(object sender, TimerEventArgs e)
        {
            elaspedDays = e.DayTicker;

            if (elaspedDays > 1 && queueDep.PatientList.Count == 0 && ivaDep.PatientList.Count == 0 
                && sanatoriumDep.PatientList.Count == 0)
            {
                OnSendFinalInfo();
            }

            var task1 = Task.Run(() => AddDoctorToIVA());
            var task2 = Task.Run(() => MoveFromQueueToDep());
            var tasks = new[] { task1, task2 };
            await Task.WhenAll(tasks);
            await Task.Run(() => OnSendLogInfo(e.DayTicker));
        } // SUBSCRIBER TO THE TICKEVENT WHICH STARTS UP THREADS THAT DOES THINGS IN THE HOSPITAL

        #region EVENT HANDLERS
        public virtual void OnSendLogInfo(int dayTicker)
        {
            var totPatientsInQueue = (Queue<Patient>)queueDep.Clone();
            var totPatientsInIva = (List<Patient>)ivaDep.Clone();
            var totPatientsInSan = (List<Patient>)sanatoriumDep.Clone();
            var totPatientsInAfterLife = afterlifePatients.ToList<Patient>();
            var totPatientsInCured = curedPatients.ToList<Patient>();
            var totUsedDoctors = usedDoctors.ToList<ExtraDoctor>();
            var currentDoctor = ivaDep.CurrentDoctor;

            FileLogEventArgs daylog = new FileLogEventArgs(startDate, totPatientsInQueue.ToList<Patient>(),
                totPatientsInIva, totPatientsInSan, totPatientsInAfterLife, totPatientsInCured,
                totUsedDoctors, dayTicker, currentDoctor);

            if (SendLogInfo != null)
            {
                SendLogInfo(this, daylog);
            }
        } // SENDS OUT EVENT AT EVERY TICK FOR A VISUAL UPDATE
        public virtual void OnSendFinalInfo()
        {
            var totPatientsInAfterLife = afterlifePatients.ToList<Patient>();
            var totPatientsInCured = curedPatients.ToList<Patient>();

            FileLogEventArgs finalLog = new FileLogEventArgs(startDate, totPatientsInCured, 
                totPatientsInAfterLife, elaspedDays);
            if (SendFinalLog != null)
            {
                SendFinalLog(this, finalLog);
            }
        } // SENDS OUT EVENT WHEN ALL DEPARTMENTS ARE EMPTY TO TURN OFF THE TIMER
        #endregion

        #region DOCTOR HANDLER
        private List<ExtraDoctor> GenerateDoctors()
        {
            List<ExtraDoctor> newDoctorList = new List<ExtraDoctor>();
            for (int i = 0; i < Conn.ExtraDoctors; i++)
            {
                newDoctorList.Add(new ExtraDoctor());
            }
            return newDoctorList;
        } // GENERATES DOCTORS AT START
        public async Task AddDoctorToIVA()
        {
            await UpdateDoctorExhausedLevel();
            await RemoveDoctorFromIVA();
            if (availableDoctorsList.Count != 0)
            {
                if (ivaDep.CurrentDoctor == null)
                {
                    ivaDep.CurrentDoctor = availableDoctorsList[0];
                    availableDoctorsList.RemoveAt(0);
                }
            }
        } // ADDS NEW DOCTOR TO IVA IF POSSIBLE
        public async Task RemoveDoctorFromIVA()
        {
            await Task.Run(() => 
            {
                if (ivaDep.CurrentDoctor != null && ivaDep.CurrentDoctor.ExhaustedLevel == 20)
                {
                    usedDoctors.Add(ivaDep.CurrentDoctor);
                    ivaDep.CurrentDoctor = null;
                }
            });
        } // REMOVES IVA-DOCTOR IF EXHAUSTEDLEVEL = 20
        public async Task UpdateDoctorExhausedLevel()
        {
            await Task.Run(() =>
            {
                if (ivaDep.CurrentDoctor != null)
                {
                    ivaDep.CurrentDoctor.ExhaustedLevel += 5;
                }
            });
        } // UPDATES IVA-DOCTOR EXHAUSTEDLEVEL
        #endregion

        #region PATIENT HANDLER
        public async Task MoveFromQueueToDep()
        {
            await UpdatePatientStatus();

            if (queueDep.PatientList.Count != 0)
            {
                bool isPlace = true;
                while (isPlace == true)
                {
                    if (ivaDep.PatientList.Count < Conn.IVASize)
                    {
                        if (queueDep.PatientList.Count != 0) 
                        {
                            ivaDep.PatientList.Add(queueDep.RemovePatient());
                            if (queueDep.PatientList.Count == 0)
                            {
                                isPlace = false;
                            }
                        }
                    }
                    else if (sanatoriumDep.PatientList.Count < Conn.SanSize)
                    {
                        if (queueDep.PatientList.Count != 0)
                        {
                            sanatoriumDep.PatientList.Add(queueDep.RemovePatient());
                            if (queueDep.PatientList.Count == 0)
                            {
                                isPlace = false;
                            }
                        }
                    }
                    else
                    {
                        isPlace = false;
                    }
                }
            }
        } // MOVES PATIENTS FROM QUEUE TO THE DIFF DEPARTMENTS
        public async Task UpdatePatientStatus()
        {
            await ChangePatientSickness();
            await MovePatients();
        } // METHOD THAT AWAITS CHANGEPATIENTSICKNESS & MOVEPATIENTS
        public async Task ChangePatientSickness()
        {
            Task task1; Task task2; Task task3;
            List<Task> taskList = new List<Task>();
            if (queueDep.PatientList.Count != 0)
            {
                task1 = Task.Run(() => queueDep.UpdateSickness());
                taskList.Add(task1);
            }
            if (ivaDep.PatientList.Count != 0)
            {
                task2 = Task.Run(() => ivaDep.UpdateSickness());
                taskList.Add(task2);
            }
            if (sanatoriumDep.PatientList.Count != 0)
            {
                task3 = Task.Run(() => sanatoriumDep.UpdateSickness());
                taskList.Add(task3);
            }
            await Task.WhenAll(taskList);
        } // UPDATE SICKNESS IN ALL DEPARTMENTS
        public async Task MovePatients()
        {
            Task task1; Task task2; Task task3;
            List<Task> taskList = new List<Task>();
            if (queueDep.PatientList.Count != 0)
            {
                task1 = Task.Run(() => MoveIvaAndSanPatients(QueueDep, queueDep.PatientList));
                taskList.Add(task1);
            }
            if (ivaDep.PatientList.Count != 0)
            {
                task2 = Task.Run(() => MoveIvaAndSanPatients(IVADep, ivaDep.PatientList));
                taskList.Add(task2);
            }
            if (sanatoriumDep.PatientList.Count != 0)
            {
                task3 = Task.Run(() => MoveIvaAndSanPatients(SanatoriumDep, sanatoriumDep.PatientList));
                taskList.Add(task3);
            }
            await Task.WhenAll(taskList);
        } // MOVES PATIENTS FROM IVA, SAN AND QUEUE TO EITHER AFTERLIFE OR CURED
        public async Task MoveIvaAndSanPatients(IDepartment iDep, List<Patient> patients)
        {
            await Task.Run(() =>
            {
                List<Patient> deadList = new List<Patient>();
                deadList = patients.Where(patient => patient.IsDead == true).ToList();
                patients.RemoveAll(patient => patient.IsDead == true); 
                foreach (var deadPatient in deadList)
                {
                    afterlifePatients.Add(deadPatient);
                }

                List<Patient> curedList = new List<Patient>();
                curedList = patients.Where(patient => patient.IsCured == true).ToList();
                patients.RemoveAll(patient => patient.IsCured == true);
                foreach (var curedPatient in curedList)
                {
                    curedPatients.Add(curedPatient);
                }

                if (iDep is PatientQueue)
                {
                    queueDep.PatientList = patients;
                }
            });
        }
        #endregion
    }
}