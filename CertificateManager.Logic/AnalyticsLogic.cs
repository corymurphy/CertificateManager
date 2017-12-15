using CertificateManager.Entities.Interfaces;
using CertificateManager.Entities.Models;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CertificateManager.Logic
{
    public class AnalyticsLogic
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly ICertificateRepository certificateRepository;
        private readonly IAuditRepository auditRepository;

        public AnalyticsLogic(IConfigurationRepository configurationRepository, ICertificateRepository certificateRepository, IAuditRepository auditRepository)
        {
            this.configurationRepository = configurationRepository;
            this.certificateRepository = certificateRepository;
            this.auditRepository = auditRepository;
        }

        public List<I2dChartData> GetCertificateHistory()
        {
            List<I2dChartData> history = new List<I2dChartData>();
            int fromDay = -7;

            DateTime issuedFrom = DateTime.Now.AddDays(fromDay);

            Expression<Func<CertificateIssuanceModel, bool>> query = x => x.IssuedOn > issuedFrom;

            IEnumerable<CertificateIssuanceModel> certs = certificateRepository.Get<CertificateIssuanceModel>(query);

            while( fromDay <= 0 )
            {
                DateTime day = DateTime.Now.AddDays(fromDay);

                DateTime dayMin = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
                DateTime dayMax = new DateTime(day.Year, day.Month, day.Day, 23, 59, 59);

                IEnumerable<CertificateIssuanceModel> dayQuery = certs.Where(x => x.IssuedOn >= dayMin && x.IssuedOn <= dayMax);

                if(dayQuery.Any())
                {
                    history.Add(new CertificateHistoryModel(day, dayQuery.Count()));
                }
                else
                {
                    history.Add(new CertificateHistoryModel(day, 0));
                }

                fromDay++;
            }

            return history;

        }

        public List<I2dChartData> GetLogHistory()
        {
            List<I2dChartData> history = new List<I2dChartData>();

            int fromDay = -7;

            DateTime start = DateTime.Now.AddDays(fromDay);

            IEnumerable<LogDateModel> logs = auditRepository.Get<LogDateModel>(x => x.Time > start);

            while(fromDay <= 0)
            {
                DateTime day = DateTime.Now.AddDays(fromDay);

                DateTime dayMin = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
                DateTime dayMax = new DateTime(day.Year, day.Month, day.Day, 23, 59, 59);

                IEnumerable<LogDateModel> dayQuery = logs.Where(x => x.Time >= dayMin && x.Time <= dayMax);

                if (dayQuery.Any())
                {
                    history.Add(new LogHistoryModel(day, dayQuery.Count()));
                }
                else
                {
                    history.Add(new LogHistoryModel(day, 0));
                }

                fromDay++;
            }

            return history;
        }

    }
}
