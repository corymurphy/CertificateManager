namespace CertificateManager.Entities.Interfaces
{
    public interface ICertificateSubjectRaw
    {
        string SubjectCommonName { get; }
        string SubjectDepartment { get; }
        string SubjectOrganization { get; }
        string SubjectCity { get; }
        string SubjectState { get; }
        string SubjectCountry { get; }
        string SubjectAlternativeNamesRaw { get; }
    }
}
