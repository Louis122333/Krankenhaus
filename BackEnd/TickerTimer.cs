using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BackEnd
{
    public class TickerTimer
    {
        public event EventHandler<TimerEventArgs> SendTick;
        private int counter;
        private int tickerSpeed;
        Timer testTimer;
        
        public TickerTimer(int tickSpeed)
        {
            tickerSpeed = tickSpeed;
            counter = 0;
        }
        public void StartTimer()
        {
            testTimer = new Timer(new TimerCallback(this.OnSendTicks), null, 1000, tickerSpeed);
        }
        public virtual void OnSendTicks(object state)
        {
            counter++;
            TimerEventArgs e = new TimerEventArgs();
            e.DayTicker = counter;
            if (SendTick != null)
            {
                SendTick(this, e);
            }
        }
        public virtual void OnSendFinalInfo(object sender, FileLogEventArgs e)
        {
            testTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
    public class TimerEventArgs
    {
        public int DayTicker { get; set; }
    }
}
