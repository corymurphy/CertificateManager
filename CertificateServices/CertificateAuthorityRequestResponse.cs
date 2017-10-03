using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class CertificateAuthorityRequestResponse
    {
        public CertificateAuthorityRequestResponse(CertificateRequestStatus certificateRequestStatus, string certificateAuthority, string encodedCertificate)
        {
            _certificateRequestStatus = certificateRequestStatus;
            _certificateAuthority = certificateAuthority;
            _issuedCertificate = encodedCertificate;
        }
        public CertificateAuthorityRequestResponse(int reqestId, CertificateRequestStatus certificateRequestStatus, string certificateAuthority, string issuedCertificate)
        {
            _reqestId = reqestId;
            _certificateRequestStatus = certificateRequestStatus;
            _certificateAuthority = certificateAuthority;
            _issuedCertificate = issuedCertificate;
        }
        public CertificateAuthorityRequestResponse(int reqestId, CertificateRequestStatus certificateRequestStatus, string certificateAuthority)
        {
            _reqestId = reqestId;
            _certificateRequestStatus = certificateRequestStatus;
            _certificateAuthority = certificateAuthority;
        }
        public CertificateAuthorityRequestResponse(CertificateRequestStatus certificateRequestStatus, string certificateAuthority)
        {
            _reqestId = 0;
            _certificateRequestStatus = certificateRequestStatus;
            _certificateAuthority = certificateAuthority;
        }

        private int _reqestId;
        private CertificateRequestStatus _certificateRequestStatus;
        private string _certificateAuthority;
        private string _issuedCertificate;
        public int RequestId { get { return _reqestId; } }
        public CertificateRequestStatus CertificateRequestStatus { get { return _certificateRequestStatus; } }
        public string CertificateAuthority { get { return _certificateAuthority; } }
        public string IssuedCertificate { get { return _issuedCertificate; } }
    }
}
