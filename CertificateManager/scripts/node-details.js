var NodeDetails = {

    Id: null,

    CredentialId: null,

    Credential: null,

    Hostname: null,

    PageLoad: function () {
        openTab(event, 'nodeDetails');
        NodeDetails.Id = $('#id');
        NodeDetails.Credential = $('#credential');
        NodeDetails.CredentialId = $('#credentialId');
        NodeDetails.Hostname = $('#hostname');
        NodeDetails.GetNode(NodeDetails.Id.text());
    },

    GetNode: function (id) {
        Services.GetNode(id, NodeDetails.GetNodeSuccessCallback, null);
    },

    GetNodeSuccessCallback: function (data) {
        NodeDetails.CredentialId.text(data.credentialId);
        NodeDetails.Hostname.text(data.hostname);
        NodeDetails.Credential.text(data.credentialDisplayName);
    }
}