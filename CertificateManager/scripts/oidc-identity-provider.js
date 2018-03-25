var OidcIdentityProvider = {
    PageLoad: function () {
        OidcIdentityProvider.InitGrid();

        OidcIdentityProvider.OidcIdpModal = $('#oidcIdpModal');
        OidcIdentityProvider.CommitOidcIdpButton = $('#commitOidcIdpButton');
    },

    InitModal: function () {
        OidcIdentityProvider.CommitOidcIdpButton('#commitOidcIdpButton');
    },

    InitGrid: function () {
        OidcIdentityProvider.Grid = $("#openIdConfigTable");

        $("#openIdConfigTable").jsGrid({
            height: "auto",
            width: "100%",

            editing: true,
            sorting: true,
            paging: true,
            autoload: true,

            pageSize: 15,
            pageButtonCount: 5,

            deleteConfirm: "Do you really want to delete this identity provider?",

            controller: OidcIdentityProvider.Controller,

            fields: [
                { name: "name", type: "text", title: "Name" },
                { name: "authority", type: "text", title: "Authority" },
                { name: "clientId", type: "text", title: "ClientId" },

                {
                    type: "control",
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                OidcIdentityProvider.ShowAddOidcIdpModal();
                            });
                    }
                }
            ]
        });
    },

    GetOidcIdpData: function () {
        return {
            name: $('#oidcIdpName').val(),
            clientId: $('#oidcIdpClientId').val(),
            authority: $('#oidcIdpAuthority').val()
        };
    },

    AddOidcIdp: function () {
        OidcIdentityProvider.Grid.jsGrid("insertItem", OidcIdentityProvider.GetOidcIdpData());
    },

    ShowAddOidcIdpModal: function () {
        OidcIdentityProvider.ResetAddOidcModalState();
        OidcIdentityProvider.OidcIdpModal.modal("show");
        OidcIdentityProvider.SetCommitOnClick("Add");
    },

    SetCommitOnClick(eventType) {
        switch (eventType) {
            case "Add":
                OidcIdentityProvider.CommitOidcIdpButton.attr("onclick", "OidcIdentityProvider.AddOidcIdp();");
                break;
            case "Edit":
                OidcIdentityProvider.CommitOidcIdpButton.attr("onclick", "OidcIdentityProvider.EditOidcIdp();");
                break;
            default:
                OidcIdentityProvider.CommitOidcIdpButton.attr("onclick", "OidcIdentityProvider.AddOidcIdp();");
        }
    },

    ResetAddOidcModalState: function () {
        $('#oidcIdpName').val('');
        $('#oidcIdpAuthority').val('');
        $('#oidcIdpClientId').val('');
    },

    Controller: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/cm-config/oidc-idp",
                dataType: "json"
            }).done(function (response) {
                d.resolve(response.payload);
            });
            return d.promise();
        },
        insertItem: function (item) {
            $.ajax({
                type: "POST",
                url: "/cm-config/oidc-idp",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                OidcIdentityProvider.HandleError(xhr.responseJSON.message);
            });
        },

        updateItem: function (item) {

            $.ajax({
                type: "PUT",
                url: "/cm-config/oidc-idp",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                OidcIdentityProvider.HandleError(xhr.responseJSON.message);
            });
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/cm-config/oidc-idp",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                OidcIdentityProvider.HandleError(xhr.responseJSON.message);
            });
        },

        onItemInserting: function (args) { OidcIdentityProvider.ResetErrorState(); },
        onItemUpdating: function (args) { OidcIdentityProvider.ResetErrorState(); },
        onItemDeleting: function (args) { OidcIdentityProvider.ResetErrorState(); }
    },

    HandleError: function (msg) {
        OidcIdentityProvider.Grid.jsGrid("render");
        UiGlobal.ShowError(msg);
    },

    ResetErrorState: function () {
        UiGlobal.HideError();
    },

    OidcIdpModal: null,

    CommitOidcIdpButton: null,

    Grid: null

}