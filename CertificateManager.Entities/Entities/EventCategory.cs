namespace CertificateManager.Entities
{
    public enum EventCategory
    {
        CertificateIssuance,
        CertificateAccess,
        CertificateDownload,
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
