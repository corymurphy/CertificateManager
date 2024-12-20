﻿using CertificateManager.Entities.Interfaces;
using CertificateServices;
using CertificateServices.Enumerations;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

namespace CertificateManager.Logic
{
    public class DataTransformation
    {

        public List<string> ParseSubjectAlternativeName(string sanraw)
        {
            DnsValidation dnsValidator = new DnsValidation();

            if (!string.IsNullOrEmpty(sanraw))
            {
                string[] sanRawArr = sanraw.Split(new char[] { ',' });
                List<string> sanArr = new List<string>();

                foreach (string san in sanRawArr)
                {
                    if (!string.IsNullOrEmpty(san))
                        sanArr.Add(san.Trim());
                }

                if (!(sanArr == null))
                {
                    if (dnsValidator.IsSubjectAlternativeNameValid(sanArr.ToArray()))
                        return sanArr;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public bool ValidateKeyUsage(string keyUsageString)
        {
            try
            {
                this.ParseKeyUsage(keyUsageString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public KeyUsage ParseKeyUsage(string keyUsageString)
        {

            if (string.IsNullOrWhiteSpace(keyUsageString))
                throw new ArgumentNullException(nameof(keyUsageString));

            KeyUsage keyUsage = (KeyUsage)Enum.Parse(typeof(KeyUsage), keyUsageString);


            return keyUsage;
            
        }

        public string GetEkuStringFromX509Certificate2(X509Certificate2 cert)
        {

            X509EnhancedKeyUsageExtension ekuExtension = null;

            foreach (X509Extension extension in cert.Extensions)
            {
                if (extension.Oid.Value == "2.5.29.37")
                {
                    ekuExtension = extension as X509EnhancedKeyUsageExtension;
                    break;
                }

            }


            if(ekuExtension == null)
            {
                throw new Exception("Could not determine EKU");
            }

            string ekuString = string.Empty;

            foreach(var eku in ekuExtension.EnhancedKeyUsages)
            {
                if(string.IsNullOrEmpty(ekuString))
                {
                    ekuString = eku.FriendlyName.Replace(" ", string.Empty).Trim();
                }
                else
                {
                    ekuString = "," + eku.FriendlyName.Replace(" ", string.Empty).Trim();
                }
            }


            return ekuString;
        }

        public List<Guid> ParseGuidList(string str)
        {
            List<Guid> list = new List<Guid>();

            if (string.IsNullOrWhiteSpace(str) || str.Length < 36 || str.Length > int.MaxValue)
                return list;


            if (str.Length == 36)
            {
                Guid result = new Guid();
                bool isGuid = Guid.TryParse(str, out result);

                if (isGuid)
                    list.Add(result);
            }
            else
            {
                string[] strArr = str.Split(';');

                if(strArr != null && strArr.Length > 0)
                {
                    foreach(string potentialGuid in strArr)
                    {
                        Guid result = new Guid();
                        bool isGuid = Guid.TryParse(potentialGuid, out result);

                        if (isGuid)
                            list.Add(result);
                    }
                }

            }


            return list;
        }

        public CertificateSubject NewCertificateSubjectFromModel(ICertificateSubjectRaw model)
        {
            List<string> san = this.ParseSubjectAlternativeName(model.SubjectAlternativeNamesRaw);

            CertificateSubject subject = new CertificateSubject(model.SubjectCommonName, san);

            if (string.IsNullOrWhiteSpace(model.SubjectCity))
                subject.City = model.SubjectCity;

            if (string.IsNullOrWhiteSpace(model.SubjectCountry))
                subject.Country = model.SubjectCountry;

            if (string.IsNullOrWhiteSpace(model.SubjectDepartment))
                subject.Department = model.SubjectDepartment;

            if (string.IsNullOrWhiteSpace(model.SubjectOrganization))
                subject.Organization = model.SubjectOrganization;

            if (string.IsNullOrWhiteSpace(model.SubjectState))
                subject.State = model.SubjectState;

            return subject;
        }
    }
}
