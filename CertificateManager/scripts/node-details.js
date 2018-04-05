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

    InitManagedCertificatesTable: function () {
        NodeDetails.ManagedCertificatesTable.bootstrapTable({
            data: NodeDetails.Node.managedCertificates
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
    }
}