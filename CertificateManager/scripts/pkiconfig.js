var debugData = "";

var PkiConfig = {

    ResolveIdpName: function (value, item)
    {
        var idpDisplayName = "";

        CmOptions.ExternalIdentitySources.forEach(function (idp) {


            if (idp.id == item.identityProviderId) {
                idpDisplayName = idp.name
            }


        });

        if (idpDisplayName === "") {
            idpDisplayName = "none";
        }

        return idpDisplayName;
    },

    InitializeSelect: function ()
    {
        CmOptions.hashAlgorithmOptions.forEach(function (item) {

            var caHash = $('#caHash');

            var element = $('#adcsTemplateHash');

            element.append($('<option>', {
                value: item.Name,
                text: item.Name
            }));


            caHash.append($('<option>', {
                value: item.Name,
                text: item.Name
            }));

            if (item.Primary === true)
            {
                caHash.val(item.Name);
                element.val(item.Name);
            }   
        });

        CmOptions.cipherOptions.forEach(function (item) {
            $('#adcsTemplateCipher').append($('<option>', {
                value: item.Name,
                text: item.Name,
            }));
        });
        
        CmOptions.keyUsageOptions.forEach(function (item) {
            $('#adcsTemplateKeyUsage').append($('<option>', {
                value: item.Name,
                text: item.Name
            }));
        });

        CmOptions.windowsApiOptions.forEach(function (item) {
            $('#adcsTemplateWindowsApi').append($('<option>', {
                value: item.Name,
                text: item.Name
            }));
        });


        //CmOptions.authenticationTypeOptions.forEach(function (item) {
        //    $('#caAuthenticationType').append($('<option>', {
        //        value: item.Name,
        //        text: item.Name
        //    }));
        //});

        //CmOptions.ExternalIdentitySources.forEach(function (item) {
        //    if (item.enabled)
        //    {
        //        $('#caAuthenticationRealm').append($('<option>', {
        //            value: item.id,
        //            text: item.name
        //        }));
        //    }
        //});
        
    },

    ResetErrorState: function () {
        UiGlobal.HideError();
    },

    HandleError: function (textStatus, grid) {
        grid.jsGrid("render");
        UiGlobal.ShowError(textStatus.responseJSON.message);
    },

    AddTemplate: function (client, isNew) {

        var allowedToIssueList = "";
        $('#adcsTemplateAllowedToIssue').val().forEach(function (item) {

            if (allowedToIssueList === "")
            {
                allowedToIssueList = item;
            }
            else
            {
                allowedToIssueList = allowedToIssueList + ';' + item;
            }

        });


        $.extend(client, {
            name: $("#adcsTemplateName").val(),
            //hash: $("#adcsTemplateHash").val(),
            cipher: $("#adcsTemplateCipher").val(),
            keyUsage: $("#adcsTemplateKeyUsage").val(),
            windowsApi: $("#adcsTemplateWindowsApi").val(),
            rolesAllowedToIssue: allowedToIssueList
        });

        $("#adcsTemplatesTable").jsGrid(isNew ? "insertItem" : "updateItem", client);

        $("#addAdcsTemplateModal").modal("hide");
    },

    ShowAddTemplateModal: function (dialogType, client) {

        $('#addAdcsTemplateButton').click(function () {

            PkiConfig.AddTemplate(client, dialogType === "Add");
        });

        $("#addAdcsTemplateModal").modal("show");
    },

    ShowEditTemplateModal: function (dialogType, client)
    {
        var roleIds = new Array();
        
        for (var key in client.rolesAllowedToIssueSelectView) {
            roleIds.push(client.rolesAllowedToIssueSelectView[key].id);
        }

        $("#adcsTemplateName").val(client.name);
        //$("#adcsTemplateHash").val(client.hash);
        $("#adcsTemplateCipher").val(client.cipher);
        $("#adcsTemplateKeyUsage").val(client.keyUsage);
        $("#adcsTemplateWindowsApi").val(client.windowsApi);


        var roleSelect = document.getElementById('adcsTemplateAllowedToIssue');

        for (i = 0; i < roleSelect.children.length; i++) {

            var option = roleSelect.children[i];

            if (roleIds.includes(option.value))
            {
                option.selected = true;
            }
            else
            {
                option.selected = false;
            }
        }

        PkiConfig.InitializeSelect2();

        $('#addAdcsTemplateButton').click(function () {

            PkiConfig.AddTemplate(client, dialogType === "Add");
        });

        $("#addAdcsTemplateModal").modal("show");
    },

    EditTemplate: function (client)
    {

    },

    ShowAddPrivateCaModal: function (client) {

        PkiConfig.InitializePrivateCaIdentityProviderSelect2();

        $('#commitPrivateCaButton').click(function () {

            PkiConfig.AddPrivateCa(client);
        });

        $("#privateCaActionModal").modal("show");
    },

    ShowEditPrivateCaModal: function (client)
    {
        debugData = client;
        $('#privateCaId').val(client.id);
        $('#caServerName').val(client.serverName);
        $('#caCommonName').val(client.commonName);
        $('#caHash').val(client.hashAlgorithm);
        
        
        PkiConfig.InitializePrivateCaIdentityProviderSelect2();

        $('#commitPrivateCaButton').click(function () {

            PkiConfig.ChangePrivateCa(client);
        });

        $("#privateCaActionModal").modal("show");
    },

    AddPrivateCa: function (client, isNew) {
        $.extend(client, {
            serverName: $("#caServerName").val(),
            commonName: $("#caCommonName").val(),
            hashAlgorithm: $("#caHash").val(),
            authenticationRealm: $("#caAuthenticationRealm").val(),
            authenticationType: $("#caAuthenticationType").val(),
            username: $("#caUsername").val(),
            password: $("#caPassword").val()
        });

        $("#privateCaTable").jsGrid("insertItem", client);

        $("#privateCaActionModal").modal("hide");
    },

    ChangePrivateCa: function (client) {
        $.extend(client, {
            id: $('#privateCaId').val(),
            serverName: $("#caServerName").val(),
            commonName: $("#caCommonName").val(),
            hashAlgorithm: $("#caHash").val(),
            identityProviderId: $('#caIdentityProvider').val()
        });

        $("#privateCaTable").jsGrid("updateItem", client);

        $("#privateCaActionModal").modal("hide");
    },

    PrivateCertificateAuthoritiesController: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/pki-config/certificate-authorities/private",
                dataType: "json"
            }).done(function (response) {
                d.resolve(response.payload);
            });
            return d.promise();
        },
        insertItem: function (item) {
            $.ajax({
                type: "POST",
                url: "/pki-config/certificate-authority/private",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                PkiConfig.HandleError(xhr.responseJSON.message, $("#privateCaTable"));
            });
        },

        updateItem: function (item) {
            var d = $.Deferred();
            $.ajax({
                type: "PUT",
                url: "/pki-config/certificate-authority/private",
                data: item
            }).done(function (response) {
                d.resolve(response.payload);
            }).fail(function (xhr, ajaxOptions, thrownError) {
                PkiConfig.HandleError(xhr.responseJSON.message, $("#privateCaTable"));
                });
            return d.promise();
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/pki-config/certificate-authority/private",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                PkiConfig.HandleError(xhr.responseJSON.message, $("#privateCaTable"));
            });
        }  
    },

    AdcsTemplateController: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/pki-config/templates",
                //data: filter,
                dataType: "json"
            }).done(function (response) {
                d.resolve(response.payload);
            });
            return d.promise();
        },

        insertItem: function (item) {
            var d = $.Deferred();
            $.ajax({
                type: "POST",
                url: "/pki-config/template",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                PkiConfig.HandleError(xhr.responseJSON.message, $("#adcsTemplatesTable"));
                });
            return d.promise();
        },

        updateItem: function (item) {
            var d = $.Deferred();
            $.ajax({
                type: "PUT",
                url: "/pki-config/template",
                data: item
            }).done(function (response) {
                d.resolve(response.payload);
            }).fail(function (xhr, ajaxOptions, thrownError) {
                PkiConfig.HandleError(xhr.responseJSON.message, $("#adcsTemplatesTable"));
                });
            return d.promise();
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/pki-config/template",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                PkiConfig.HandleError(xhr.responseJSON.message, $("#adcsTemplatesTable"));
                });

        }
    },

    InitializePkiConfigGrids: function () {

        $("#adcsTemplatesTable").jsGrid({
            height: "auto",
            width: "100%",

            //filtering: true,
            editing: false,
            sorting: true,
            paging: true,
            autoload: true,

            pageSize: 15,
            pageButtonCount: 5,

            deleteConfirm: "Do you really want to delete this template?",

            controller: PkiConfig.AdcsTemplateController,

            rowClick: function (args) {
                PkiConfig.ShowEditTemplateModal("Edit", args.item);
            },
            
            fields: [
                { title: "Template Name", name: "name", type: "text", validate: { validator: "rangeLength", param: [1, 100] }, width: 80 },
                //{ title: "Server Name", name: "hash", type: "select", items: CmOptions.hashAlgorithmOptions, valueType: "string", valueField: "Name", textField: "Name", width: 30 },
                { title: "Cipher", name: "cipher", type: "select", items: CmOptions.cipherOptions, valueField: "Name", textField: "Name", width: 30 },
                { title: "Key Usage", name: "keyUsage", type: "select", items: CmOptions.keyUsageOptions, valueField: "Name", textField: "Name" },
                { title: "WindowsApi", name: "windowsApi", type: "select", items: CmOptions.windowsApiOptions, valueField: "Name", textField: "Name", width: 40 },
                {
                    title: "Roles Allowed To Issue This Template",
                    name: "rolesAllowedToIssue",
                    itemTemplate: function (value, item) {

                        var roleSelect = $("<select style='width:100%' class='security-roles-adcs-select2' multiple='multiple'>");

                        item.rolesAllowedToIssueSelectView.forEach(function (option) {
                            roleSelect.append($('<option>', {
                                value: option.id,
                                text: option.name
                            }).attr('selected', true));
                        });

                        roleSelect = roleSelect.attr('disabled', true);
                        return roleSelect;
                        //return $("<div>").append(roleSelect);
                    }
                },
                {
                    width:25,
                    editButton: false,
                    type: "control",
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                PkiConfig.ShowAddTemplateModal("Add", {});
                            });
                    }
                }
            ],
            onRefreshed: function (args) { PkiConfig.InitializeSelect2(); },
            onItemUpdated: function (args) { PkiConfig.InitializeSelect2(); },
            onItemEditing: function (args) { PkiConfig.InitializeSelect2(); },
            onItemInserting: function (args) { PkiConfig.ResetErrorState(); },
            onItemUpdating: function (args) { PkiConfig.ResetErrorState(); PkiConfig.InitializeSelect2(); },
            onItemDeleting: function (args) { PkiConfig.ResetErrorState(); },
            onDataLoading: function (args) { PkiConfig.InitializeSelect2();}, 
            onDataLoaded: function (args) { PkiConfig.InitializeSelect2();}


        });

        $("#privateCaTable").jsGrid({
            height: "auto",
            width: "100%",

            //filtering: true,
            editing: false,
            sorting: true,
            paging: true,
            autoload: true,

            pageSize: 15,
            pageButtonCount: 5,

            deleteConfirm: "Do you really want to delete this certificate authority?",

            controller: PkiConfig.PrivateCertificateAuthoritiesController,

            rowClick: function (args) {
                PkiConfig.ShowEditPrivateCaModal(args.item);
            },

            fields: [
                { title: "Server Name", name: "serverName", type: "text" },
                { title: "Common Name", name: "commonName", type: "text" },
                { title: "Hash", name: "hashAlgorithm", type: "select", items: CmOptions.hashAlgorithmOptions, valueType: "string", valueField: "Name", textField: "Name" },
                { title: "Identity Provider", name: "identityProviderId", itemTemplate: function (value, item) { return PkiConfig.ResolveIdpName(value, item); }  },
                {
                    type: "control",
                    editButton: false,
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                PkiConfig.ShowAddPrivateCaModal({});
                            });
                    }
                }
            ],
            onItemInserting: function (args) { PkiConfig.ResetErrorState(); },
            onItemUpdating: function (args) { PkiConfig.ResetErrorState(); },
            onItemDeleting: function (args) { PkiConfig.ResetErrorState(); }
        });

        var editableAllowedToIssue = $('.security-roles-adcs-select2-editable');

        CmOptions.SecurityRoles.forEach(function (option) {
            editableAllowedToIssue.append($('<option>', {
                //value: item.Name,
                //text: item.Name,
                value: option.Id,
                text: option.Name
            }))
        });
    },

    InitializePrivateCaIdentityProviderSelect2: function ()
    {
        $('#caIdentityProvider').select2({ data: CmOptions.ExternalIdentitySources, width: '100%' });
    },

    InitializeSelect2: function ()
    {
        $('.security-roles-adcs-select2').select2({ width: '100%' });
    }
}

