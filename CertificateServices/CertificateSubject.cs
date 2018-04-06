using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CertificateServices
{
    public class CertificateSubject
    {
        private CertificateSubject()
        {
            this.CommonName = "err_invalid_cn";
        }

        public CertificateSubject(string commonName, List<string> san)
        {
            if (String.IsNullOrWhiteSpace(commonName))
                throw new ArgumentNullException(nameof(commonName), "Certificate common name cannot be null or empty");

            if (commonName.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(commonName), "Subject common name longer than the maximum of 64 characters or is null");

            CommonName = commonName;

            SetSubjectAlternativeName(san);
        }

        public CertificateSubject(string commonName, bool appendSand = true)
        {
            if (String.IsNullOrWhiteSpace(commonName))
                throw new ArgumentNullException( nameof(commonName), "Certificate common name cannot be null or empty");

            if (commonName.Length >= 64 )
                throw new ArgumentOutOfRangeException(nameof(commonName), "Subject common name longer than the maximum of 64 characters or is null");

            CommonName = commonName;

            SetSubjectAlternativeName(appendSand);
        }

        public CertificateSubject(string commonName, string department, string organization, bool appendSand = true)
        {
            if (String.IsNullOrWhiteSpace(commonName) || commonName.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(commonName), "Subject common name longer than the maximum of 64 characters or is null");
            if (String.IsNullOrWhiteSpace(department) || department.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(department), "Department longer than the maximum of 64 characters or is null");
            if (String.IsNullOrWhiteSpace(organization) || organization.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(organization), "Organization longer than the maximum of 64 characters or is null");

            CommonName = commonName;
            Department = department;
            Organization = organization;

            SetSubjectAlternativeName(appendSand);
        }

        public CertificateSubject(string commonName, string department, string organization, List<string> san)
        {
            if (String.IsNullOrWhiteSpace(commonName) || commonName.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(commonName), "Subject common name longer than the maximum of 64 characters or is null");
            if (String.IsNullOrWhiteSpace(department) || department.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(department), "Department longer than the maximum of 64 characters or is null");
            if (String.IsNullOrWhiteSpace(organization) || organization.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(organization), "Organization longer than the maximum of 64 characters or is null");

            CommonName = commonName;
            Department = department;
            Organization = organization;

            SetSubjectAlternativeName(san);

        }

        public CertificateSubject(X509Certificate2 cert)
        {
            if(cert == null)
            {
                throw new ArgumentNullException(nameof(cert));
            }

            if(string.IsNullOrWhiteSpace(cert.Subject))
            {
                throw new Exception("Subject cannot be null");
            }

            if(!IsValidDistinguishedNameLength(cert.Subject))
            {
                throw new Exception("Distinguished name of the subject is too long");
            }


            if(cert.Subject.Contains(","))
            {
                foreach(string component in cert.Subject.Split(','))
                {
                    if (IsCommonNameComponent(component))
                    {
                        this.CommonName = GetCommonName(component);
                    }
                }
                return;
            }

            else
            {
                if(!IsCommonNameComponent(cert.Subject))
                {
                    throw new Exception("Invalid CN");
                }
            }

            throw new Exception("Could not parse subject");

        }

        public CertificateSubject
            (
                string commonName,
                string department,
                string organization,
                string city,
                string state,
                string country,
                bool appendSand = true
            )
        {

            ValidateParams(commonName, department, organization, city, state, country);

            SetParams(commonName, department, organization, city, state, country);

            SetSubjectAlternativeName(appendSand);
        }

        public CertificateSubject
            (
                string commonName,
                string department,
                string organization,
                string city,
                string state,
                string country,
                List<string> san
            )
        {

            ValidateParams(commonName, department, organization, city, state, country);

            SetParams(commonName, department, organization, city, state, country);

            SetSubjectAlternativeName(san);
        }

        public string CommonName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Organization { get; set; }
        public string Department { get; set; }
        public string Country { get; set; }
        public List<string> SubjectAlternativeName { get; set; }
        public bool ContainsSubjectAlternativeName { get; set; }
        public override string ToString()
        {
            string formattedSubject = String.Format("CN={0}", CommonName);

            if (!(String.IsNullOrEmpty(Department)))
                formattedSubject = String.Join(",", formattedSubject, String.Format("OU={0}", Department));
            if (!(String.IsNullOrEmpty(Organization)))
                formattedSubject = String.Join(",", formattedSubject, String.Format("O={0}", Organization));
            if (!(String.IsNullOrEmpty(City)))
                formattedSubject = String.Join(",", formattedSubject, String.Format("L={0}", City));
            if (!(String.IsNullOrEmpty(State)))
                formattedSubject = String.Join(",", formattedSubject, String.Format("S={0}", State));
            if (!(String.IsNullOrEmpty(Country)))
                formattedSubject = String.Join(",", formattedSubject, String.Format("C={0}", Country));

            return formattedSubject;
        }

        private void ValidateParams(string commonName, string department, string organization, string city, string state, string country)
        {
            if (string.IsNullOrWhiteSpace(commonName))
                throw new ArgumentNullException(nameof(commonName));

            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentNullException(nameof(department));

            if (string.IsNullOrWhiteSpace(organization))
                throw new ArgumentNullException(nameof(organization));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentNullException(nameof(city));

            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentNullException(nameof(state));

            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentNullException(nameof(country));

            if (commonName.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(commonName), "Subject common name longer than the maximum of 64 characters or is null");

            if (department.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(department), "Subject department longer than the maximum of 64 characters or is null");

            if (organization.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(organization), "Subject organization longer than the maximum of 64 characters or is null");

            if (city.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(city), "Subject city longer than the maximum of 64 characters or is null");

            if (state.Length >= 64)
                throw new ArgumentOutOfRangeException(nameof(state), "Subject state longer than the maximum of 64 characters or is null");

            if (country.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(country), "Country code must have 2 characters");
        }

        private void SetParams(string commonName, string department, string organization, string city, string state, string country)
        {
            CommonName = commonName;
            City = city;
            State = state;
            Department = department;
            Organization = organization;
            Country = country;
        }

        public static CertificateSubject CreateFromDistinguishedName(string dn)
        {

            if (String.IsNullOrWhiteSpace(dn))
                throw new ArgumentNullException(nameof(dn));

            if (!IsValidDistinguishedNameLength(dn))
                throw new ArgumentOutOfRangeException(nameof(dn));

            CertificateSubject subject = new CertificateSubject();

            foreach (string item in dn.Split(','))
            {
                if (subject.IsCommonNameComponent(item))
                    subject.CommonName = subject.GetCommonName(item);

                if (subject.IsDepartmentComponent(item))
                    subject.Department = subject.GetDepartment(item);

                if (subject.IsOrganizationComponent(item))
                    subject.Organization = subject.GetOrganization(item);

                if (subject.IsCityComponent(item))
                    subject.City = subject.GetCity(item);

                if (subject.IsStateComponent(item))
                    subject.State = subject.GetState(item);

                if (subject.IsCountryComponent(item))
                    subject.Country = subject.GetCountry(item);
            }
            return subject;
        }

        private void SetSubjectAlternativeName(List<string> san)
        {
            if (san == null || san.Count < 1)
            {
                this.ContainsSubjectAlternativeName = false;
            }
            else
            {
                this.ContainsSubjectAlternativeName = true;
                SubjectAlternativeName = san;
            }
        }

        private void SetSubjectAlternativeName(bool appendSan)
        {
            if(appendSan == true)
            {
                this.ContainsSubjectAlternativeName = true;
                SubjectAlternativeName = new List<string> { this.CommonName };
            }
            else
            {
                this.ContainsSubjectAlternativeName = false;
            }
        }

        private bool IsCommonNameComponent(string item)
        {
            if (item.Trim().ToUpper().StartsWith("CN=", StringComparison.CurrentCultureIgnoreCase))
                return true;
            else
                return false;
        }

        private bool IsDepartmentComponent(string item)
        {
            if (item.Trim().ToUpper().StartsWith("OU=", StringComparison.CurrentCultureIgnoreCase))
                return true;
            else
                return false;
        }

        private bool IsOrganizationComponent(string item)
        {
            if (item.Trim().ToUpper().StartsWith("O=", StringComparison.CurrentCultureIgnoreCase))
                return true;
            else
                return false;
        }

        private bool IsCityComponent(string item)
        {
            if (item.Trim().ToUpper().StartsWith("L=", StringComparison.CurrentCultureIgnoreCase))
                return true;
            else
                return false;
        }

        private bool IsStateComponent(string item)
        {
            if (item.Trim().ToUpper().StartsWith("S=", StringComparison.CurrentCultureIgnoreCase))
                return true;
            else
                return false;
        }

        private bool IsCountryComponent(string item)
        {
            if (item.Trim().ToUpper().StartsWith("C=", StringComparison.CurrentCultureIgnoreCase))
                return true;
            else
                return false;
        }

        private string GetCommonName(string item)
        {
            return item.Trim().Substring(3);
        }

        private string GetDepartment(string item)
        {
            return item.Trim().Substring(3);
        }

        private string GetOrganization(string item)
        {
            return item.Trim().Substring(2);
        }

        private string GetCity(string item)
        {
            return item.Trim().Substring(2);
        }

        private string GetState(string item)
        {
            return item.Trim().Substring(2);
        }

        private string GetCountry(string item)
        {
            return item.Trim().Substring(2);
        }

        private static bool IsValidDistinguishedNameLength(string dn)
        {
            if (dn.Length > 256)
                return false;
            else
                return true;

        }

        private bool IsValidDistinguishedNameComponentValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (value.Contains("="))
                return false;

            if (value.Contains("/"))
                return false;

            if (value.Contains(@"\"))
                return false;

            return true;

        }
    }
}
