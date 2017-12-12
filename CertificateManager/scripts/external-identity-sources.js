var ActiveDirectoryMetadatas = {

    ShowAuthApiActionModal: function (dialogType, client) {

        if (dialogType === "Add")
        {
            $('#commitAuthApiEntity').click(function () {

                ActiveDirectoryMetadatas.AddAuthApiCertificate(client);
            });
        }
        else
        {

        }
        

        $("#authApiActionModal").modal("show");
    },

    ShowAddActiveDirectoryMetadataModal: function (dialogType, client) {

        $('#commitAuthApiEntity').click(function () {

            ActiveDirectoryMetadatas.AddEis(client, "Add");
        });

        $("#addActiveDirectoryMetadataModal").modal("show");
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

        $("#ActiveDirectoryMetadatasTable").jsGrid(isNew ? "insertItem" : "updateItem", client);

        $("#addActiveDirectoryMetadataModal").modal("hide");
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

            controller: ActiveDirectoryMetadatas.AuthApiController,

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
                                ActiveDirectoryMetadatas.ShowAuthApiActionModal("Add", {});
                            });
                    }
                }
            ]
        });
    },

    InitializeGrid: function ()
    {
        $("#ActiveDirectoryMetadatasTable").jsGrid({
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

            controller: ActiveDirectoryMetadatas.Controller,

            fields: [
                { name: "name", type: "text", title: "Name" },
                { name: "domain", type: "text", title: "Domain" },
                { name: "searchBase", type: "text", title: "SearchBase" },
                { name: "activeDirectoryMetadataType", type: "select", items: CmOptions.ActiveDirectoryMetadataType, valueType: "string", valueField: "Name", textField: "Name", title: "Type" },
                { name: "username", type: "text", title: "Username" },
                { name: "password", type: "text", readOnly: true, title: "password" },
                { name: "enabled", type: "checkbox", title: "Enabled", sorting: false },
                {
                    type: "control",
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                ActiveDirectoryMetadatas.ShowAddActiveDirectoryMetadataModal("Add", {});
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
                ActiveDirectoryMetadatas.HandleError(xhr.responseJSON.message, $("#ActiveDirectoryMetadatasTable"));
            });
        },

        updateItem: function (item) {

            $.ajax({
                type: "PUT",
                url: "/cm-config/external-identity-source",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ActiveDirectoryMetadatas.HandleError(xhr.responseJSON.message, $("#ActiveDirectoryMetadatasTable"));
            });
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/cm-config/external-identity-source",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ActiveDirectoryMetadatas.HandleError(xhr.responseJSON.message, $("#ActiveDirectoryMetadatasTable"));
            });
        },

        onItemInserting: function (args) { ActiveDirectoryMetadatas.ResetErrorState(); },
        onItemUpdating: function (args) { ActiveDirectoryMetadatas.ResetErrorState(); },
        onItemDeleting: function (args) { ActiveDirectoryMetadatas.ResetErrorState(); }
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
                ActiveDirectoryMetadatas.HandleError(xhr.responseJSON.message, $("#authApiSigningCertificatesTable"));
            });
        },

        updateItem: function (item) {

            $.ajax({
                type: "PUT",
                url: "/identity-sources/local-authapi/trustedcertificate",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ActiveDirectoryMetadatas.HandleError(xhr.responseJSON.message, $("#authApiSigningCertificatesTable"));
            });
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/identity-sources/local-authapi/trustedcertificate",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ActiveDirectoryMetadatas.HandleError(xhr.responseJSON.message, $("#authApiSigningCertificatesTable"));
            });
        },

        onItemInserting: function (args) { ActiveDirectoryMetadatas.ResetErrorState(); },
        onItemUpdating: function (args) { ActiveDirectoryMetadatas.ResetErrorState(); },
        onItemDeleting: function (args) { ActiveDirectoryMetadatas.ResetErrorState(); }
    },

    ResetErrorState: function () {
        UiGlobal.HideError();
    },

    HandleError: function (textStatus, grid) {
        grid.jsGrid("render");
        UiGlobal.ShowError(textStatus.responseJSON.message);
    },

    InitializeSelect: function () {
        CmOptions.ActiveDirectoryMetadataType.forEach(function (item) {
            $('#eisType').append($('<option>', {
                value: item.Name,
                text: item.Name
            }));
        });
    },

    CertificateSelect2: null,

    InitializeCertificateSelect2: function ()
    {
        ActiveDirectoryMetadatas.CertificateSelect2.select2({
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
            templateResult: ActiveDirectoryMetadatas.FormatCertificate,
            templateSelection: ActiveDirectoryMetadatas.FormatCertificateSelection,

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
        $('#allowLocalAuthentication').prop('checked', data.payload.localLogonEnabled);
        $('#allowEmergencyAccess').prop('checked', data.payload.emergencyAccessEnabled);
    },

    SaveAppConfigSuccessCallback: function ()
    {
        UiGlobal.ShowSuccess("Successfully saved app configuration");
    },

    PageLoad: function ()
    {
        ActiveDirectoryMetadatas.InitializeSelect();
        ActiveDirectoryMetadatas.InitializeGrid();
        ActiveDirectoryMetadatas.CertificateSelect2 = $('#thumbprint');
        ActiveDirectoryMetadatas.InitializeCertificateSelect2();
        ActiveDirectoryMetadatas.InitializeAuthApiGrid();
        Services.GetAppConfig(ActiveDirectoryMetadatas.GetAppConfigSuccessCallback, null);
        UiGlobal.ShowCurrentTab();
    },

    SetAppConfig: function()
    {
        UiGlobal.ResetAlertState();

        var data = {
            jwtValidityPeriod: $('#jwtValidityPeriod').val(),
            localIdpIdentifier: $('#localIdpIdentifier').val()
        }

        Services.SetAppConfig(data, ActiveDirectoryMetadatas.SaveAppConfigSuccessCallback, null);
    },

    SetLocalAppConfig: function ()
    {

        UiGlobal.ResetAlertState();

        var data = {
            localLogonEnabled: $('#allowLocalAuthentication').prop('checked'),
            emergencyAccessEnabled: $('#allowEmergencyAccess').prop('checked')
        }

        Services.SetLocalAppConfig(data, ActiveDirectoryMetadatas.SaveAppConfigSuccessCallback, null);
    }
}