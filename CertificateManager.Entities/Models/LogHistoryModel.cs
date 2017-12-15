using CertificateManager.Entities.Interfaces;
using System;

namespace CertificateManager.Entities.Models
{

    public class LogHistoryModel : I2dChartData
    {
        public LogHistoryModel(DateTime day, int count)
        {
            XAxis = day.ToString("MMM dd");
            YAxis = count.ToString();
        }

        public string XAxis { get; private set; }

        public string YAxis { get; private set; }
    }
}
