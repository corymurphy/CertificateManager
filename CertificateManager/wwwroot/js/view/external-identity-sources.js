var ExternalIdentitySources = {

    ShowAuthApiActionModal: function (dialogType, client) {

        if (dialogType === "Add")
        {
            $('#commitAuthApiEntity').click(function () {

                ExternalIdentitySources.AddAuthApiCertificate(client);
            });
        }
        else
        {

        }
        

        $("#authApiActionModal").modal("show");
    },

    ShowAddExternalIdentitySourceModal: function (dialogType, client) {

        $('#commitAuthApiEntity').click(function () {

            ExternalIdentitySources.AddEis(client, "Add");
        });

        $("#addExternalIdentitySourceModal").modal("show");
    },

    AddEis: function (client, isNew) {
        $.extend(client, {
            name: $("#eisName").val(),
            domain: $("#eisDomain").val(),
            username: $("#eisUsername").val(),
            password: $("#eisPassword").val(),
            enabled: $("#eisEnabled").val(),
            searchBase: $("#eisSearchBase").val(),
        });

        $("#externalIdentitySourcesTable").jsGrid(isNew ? "insertItem" : "updateItem", client);

        $("#addExternalIdentitySourceModal").modal("hide");
    },

    AddAuthApiCertificate: function (client) {
        $.extend(client, {
            id: $("#thumbprint").val(),
            primary: $("#primary").is(":checked")
        });

        $("#authApiSigningCertificatesTable").jsGrid("insertItem", client);

        $("#authApiActionModal").modal("hide");
    },

    InitializeAuthApiGrid: function ()
    {
        $('#authApiSigningCertificatesTable').jsGrid({
            height: "auto",
            width: "100%",

            //filtering: true,
            editing: false,
            sorting: true,
            paging: true,
            autoload: true,

            pageSize: 15,
            pageButtonCount: 5,

            deleteConfirm: "Do you really want to delete this external identity source?",

            controller: ExternalIdentitySources.AuthApiController,

            fields: [
                { name: "displayName", type: "text", title: "DisplayName" },
                { name: "thumbprint", type: "text", title: "Thumbprint" },
                { name: "hasPrivateKey", type: "checkbox", title: "HasPrivateKey", sorting: false },
                { name: "primary", type: "checkbox", title: "Primary", sorting: false },
                {
                    type: "control",
                    editButton: false,
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                ExternalIdentitySources.ShowAuthApiActionModal("Add", {});
                            });
                    }
                }
            ]
        });
    },

    InitializeGrid: function ()
    {
        $("#externalIdentitySourcesTable").jsGrid({
            height: "auto",
            width: "100%",

            //filtering: true,
            editing: true,
            sorting: true,
            paging: true,
            autoload: true,

            pageSize: 15,
            pageButtonCount: 5,

            deleteConfirm: "Do you really want to delete this external identity source?",

            controller: ExternalIdentitySources.Controller,

            fields: [
                { name: "name", type: "text", title: "Name" },
                { name: "domain", type: "text", title: "Domain" },
                { name: "searchBase", type: "text", title: "SearchBase" },
                { name: "externalIdentitySourceType", type: "select", items: CmOptions.ExternalIdentitySourceType, valueType: "string", valueField: "Name", textField: "Name", title: "Type" },
                { name: "username", type: "text", title: "Username" },
                { name: "password", type: "text", readOnly: true, title: "password" },
                { name: "enabled", type: "checkbox", title: "Enabled", sorting: false },
                {
                    type: "control",
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                ExternalIdentitySources.ShowAddExternalIdentitySourceModal("Add", {});
                            });
                    }
                }
            ]
        });
    },

    Controller: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/cm-config/external-identity-sources",
                dataType: "json"
            }).done(function (response) {
                d.resolve(response);
            });
            return d.promise();
        },
        insertItem: function (item) {
            $.ajax({
                type: "POST",
                url: "/cm-config/external-identity-source",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ExternalIdentitySources.HandleError(xhr.responseJSON.message, $("#externalIdentitySourcesTable"));
            });
        },

        updateItem: function (item) {

            $.ajax({
                type: "PUT",
                url: "/cm-config/external-identity-source",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ExternalIdentitySources.HandleError(xhr.responseJSON.message, $("#externalIdentitySourcesTable"));
            });
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/cm-config/external-identity-source",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ExternalIdentitySources.HandleError(xhr.responseJSON.message, $("#externalIdentitySourcesTable"));
            });
        },

        onItemInserting: function (args) { ExternalIdentitySources.ResetErrorState(); },
        onItemUpdating: function (args) { ExternalIdentitySources.ResetErrorState(); },
        onItemDeleting: function (args) { ExternalIdentitySources.ResetErrorState(); }
    },

    AuthApiController: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/identity-sources/local-authapi/trustedcertificates",
                dataType: "json"
            }).done(function (response) {
                d.resolve(response.payload);
            });
            return d.promise();
        },
        insertItem: function (item) {
            $.ajax({
                type: "POST",
                url: "/identity-sources/local-authapi/trustedcertificate",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ExternalIdentitySources.HandleError(xhr.responseJSON.message, $("#authApiSigningCertificatesTable"));
            });
        },

        updateItem: function (item) {

            $.ajax({
                type: "PUT",
                url: "/identity-sources/local-authapi/trustedcertificate",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ExternalIdentitySources.HandleError(xhr.responseJSON.message, $("#authApiSigningCertificatesTable"));
            });
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/identity-sources/local-authapi/trustedcertificate",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ExternalIdentitySources.HandleError(xhr.responseJSON.message, $("#authApiSigningCertificatesTable"));
            });
        },

        onItemInserting: function (args) { ExternalIdentitySources.ResetErrorState(); },
        onItemUpdating: function (args) { ExternalIdentitySources.ResetErrorState(); },
        onItemDeleting: function (args) { ExternalIdentitySources.ResetErrorState(); }
    },

    ResetErrorState: function () {
        UiGlobal.HideError();
    },

    HandleError: function (textStatus, grid) {
        grid.jsGrid("render");
        UiGlobal.ShowError(textStatus.responseJSON.message);
    },

    InitializeSelect: function () {
        CmOptions.ExternalIdentitySourceType.forEach(function (item) {
            $('#eisType').append($('<option>', {
                value: item.Name,
                text: item.Name
            }));
        });
    },

    CertificateSelect2: null,

    InitializeCertificateSelect2: function ()
    {
        ExternalIdentitySources.CertificateSelect2.select2({
            width: '100%',
            placeholder: 'Search for a certificate',
            ajax: {
                url: ("/certificates/search/summary"),
                dataType: 'json',
                type: 'get',
                delay: 30,
                //data: function (params) {
                //    return {
                //        query: params.term
                //    };
                //},
                processResults: function (data, params) {
                    //params.page = params.page || 1;

                    return {
                        results: data.payload,
                    };
                },
                cache: false
            },
            escapeMarkup: function (markup) { return markup; },
            templateResult: ExternalIdentitySources.FormatCertificate,
            templateSelection: ExternalIdentitySources.FormatCertificateSelection,

        });
    },

    FormatCertificate: function (repo) {

        var markup = '<div class="user-select-container">'
        markup = markup + '<div class="cert-select-title"><b>' + repo.displayName + '</b></div>';
        markup = markup + '<div class="cert-select-details">Thumbprint: <span class="cert-select-details-value">' + repo.thumbprint + '</span></div>';
        markup = markup + '<div class="cert-select-details">Expires: <span class="cert-select-details-value">' + UiGlobal.GetDateString(repo.expiry) + '</span></div>';
        //markup = markup + '<div class="user-select-details">Domain: <span class="user-select-details-value">' + repo.domain + '</span></div>';
        markup = markup + '</div>';
        //var markup = repo.text;
        return markup;
    },

    FormatCertificateSelection: function (repo) {
        return repo.thumbprint;
    },

    GetAppConfigSuccessCallback: function (data)
    {
        $('#jwtValidityPeriod').val(data.payload.jwtValidityPeriod);
        $('#localIdpIdentifier').val(data.payload.localIdpIdentifier);
    },

    SaveAppConfigSuccessCallback: function ()
    {
        UiGlobal.ShowSuccess("Successfully saved app configuration");
    },

    PageLoad: function ()
    {
        ExternalIdentitySources.InitializeSelect();
        ExternalIdentitySources.InitializeGrid();
        ExternalIdentitySources.CertificateSelect2 = $('#thumbprint');
        ExternalIdentitySources.InitializeCertificateSelect2();
        ExternalIdentitySources.InitializeAuthApiGrid();
        Services.GetAppConfig(ExternalIdentitySources.GetAppConfigSuccessCallback, null);
        UiGlobal.ShowCurrentTab();
    },

    SetAppConfig: function()
    {
        var data = {
            jwtValidityPeriod: $('#jwtValidityPeriod').val(),
            localIdpIdentifier: $('#localIdpIdentifier').val()
        }

        Services.SetAppConfig(data, ExternalIdentitySources.SaveAppConfigSuccessCallback, null);
    }
}