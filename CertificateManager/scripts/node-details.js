var NodeDetails = {

    Id: null,

    CredentialId: null,

    Hostname: null,

    PageLoad: function () {
        NodeDetails.Id = $('#id');
        NodeDetails.CredentialId = $('#credential');
        NodeDetails.Hostname = $('#hostname');
        NodeDetails.GetNode(NodeDetails.Id);
    },

    GetNode: function (id) {
        Services.GetNode(id, NodeDetails.GetNodeSuccessCallback, null);
    },

    GetNodeSuccessCallback: function (data) {
        NodeDetails.CredentialId = data.credentialId;
        NodeDetails.Hostname = data.hostname;

    }
}