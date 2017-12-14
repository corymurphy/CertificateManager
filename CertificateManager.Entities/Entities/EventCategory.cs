namespace CertificateManager.Entities
{
    public enum EventCategory
    {
        CertificateViewed, 
        CertificateIssuance,
        CertificateAccess,
        CertificateDownload,
        CertificatePasswordViewed,
        CertificatePasswordReset,
        UserAuthentication,
        UserManagementResetPassword,
        UserManagementSet,
        UserManagementDelete,
        UserManagementNew,
        UserManagementImport,
        RoleManagementNew,
        RoleManagementDelete,
        RoleManagementUpdate,
        RoleManagementAddMember,
        RoleManagementSetScopes
    }
}
