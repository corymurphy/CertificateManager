using CertificateManager.Entities.Interfaces;
using System;

namespace CertificateManager.Entities.Models
{
    public class CertificateHistoryModel : I2dChartData
    {
        public CertificateHistoryModel(DateTime day, int count)
        {
            this.Day = day.ToString("MMM dd");
            this.IssuanceCount = count;
        }

        public string Day { get; set; }
        public int IssuanceCount { get; set; }

        public string XAxis => Day;

        public string YAxis => IssuanceCount.ToString();
    }
}
