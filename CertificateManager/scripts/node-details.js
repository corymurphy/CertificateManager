var NodeDetails = {

    Id: null,

    CredentialId: null,

    Credential: null,

    Hostname: null,

    Node: null,

    ManagedCertificatesTable: null,

    PageLoad: function () {

        UiGlobal.ShowCurrentTab();
        NodeDetails.Id = $('#id');
        NodeDetails.Credential = $('#credential');
        NodeDetails.CredentialId = $('#credentialId');
        NodeDetails.Hostname = $('#hostname');
        NodeDetails.ManagedCertificatesTable = $('#managedCertificatesTable');
        NodeDetails.GetNode(NodeDetails.Id.val());
    },

    ShowSelectedTable: function () {

    },

    GetNode: function (id) {
        Services.GetNode(id, NodeDetails.GetNodeSuccessCallback, null);
    },


    RenewManagedCertificateSucessCallback: function () {
        UiGlobal.ShowSuccess("Certificate renewal job has started. Check the logs for result.");
    },

    RenewManagedCertificateErrorCallback: function () {
        UiGlobal.ShowError("Certificate renewal failed. Check the logs for more details.");
    },

    RenewManagedCertificate: function (id) {
        var uri = "/node/" + NodeDetails.Node.id + "/renew/" + id;
        Services.Post(uri, null, NodeDetails.RenewManagedCertificateSucessCallback, NodeDetails.RenewManagedCertificateErrorCallback)
    },

    InitManagedCertificatesTable: function () {
        NodeDetails.ManagedCertificatesTable.bootstrapTable({
            data: NodeDetails.Node.managedCertificates,
            columns: [{ align: 'left' }, { align: 'left'},
            {
                field: 'operate',
                title: 'Action',
                align: 'left',
                valign: 'middle',
                clickToSelect: false,
                formatter: function (value, row, index) {
                    return '<button class="btn btn-primary btn-sm" onclick="NodeDetails.RenewManagedCertificate(' + " '" + row.id + "' " + ')" >Renew</button> ';
                }
            }]
        });
    },

    GetNodeSuccessCallback: function (data) {
        NodeDetails.Node = data;
        NodeDetails.CredentialId.val(data.credentialId);
        NodeDetails.Hostname.val(data.hostname);
        NodeDetails.Credential.val(data.credentialDisplayName);
        NodeDetails.InitManagedCertificatesTable();
    },

    InvokeCertificateDiscovery: function () {
        UiGlobal.ResetAlertState();
        Services.Post("/node/" + NodeDetails.Id.val() + "/discovery/iis", null, NodeDetails.InvokeCertificateDiscoverySuccessCallback, NodeDetails.InvokeCertificateDiscoveryErrorCallback);
    },

    InvokeCertificateDiscoverySuccessCallback: function () {
        UiGlobal.ShowSuccess("Certificate discovery has started for this node. Reviews logs for results.");
    },

    InvokeCertificateDiscoveryErrorCallback: function () {
        UiGlobal.ShowError("Failed to start certificate discovery");
    },

    ResetManagedCertificatesState: function () {
        var uri = "/node/" + NodeDetails.Id.val() + "/managedcertificates";
        Services.Delete(uri, NodeDetails.ResetManagedCertificatesStateSuccessCallback, NodeDetails.ResetManagedCertificatesStateErrorCallback)
    },

    ResetManagedCertificatesStateSuccessCallback: function () {
        UiGlobal.ShowSuccess("Successfully reset the state of all managed certificates for this node.");
    },

    ResetManagedCertificatesStateErrorCallback: function () {
        UiGlobal.ShowError("Failed to reset state of managed certificates");
    }
}