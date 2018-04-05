ViewCertificate = {

    PageLoad: function ()
    {
        document.getElementById("defaultOpen").click();

        UiGlobal.ShowCurrentTab();

        ViewCertificate.SubjectAlternativeNameTable = $('#subjectAlternativeNameTable');
        ViewCertificate.AclTable = $('#certificateAclTable');
        ViewCertificate.InitialzeAclTable();
        ViewCertificate.CertificateAceModal = $('#addCertificateAceModal');
        Services.GetCertificateDetails(ViewCertificate.GetCertificateId(), ViewCertificate.GetCertificateSuccessCallback, ViewCertificate.GetCertificateErrorCallback);
        ViewCertificate.InitializeDownloadUx();
        ViewCertificate.InitializeShowPassword();
        ViewCertificate.InitializeResetPassword();
        ViewCertificate.InitializeSelects();

        ViewCertificate.NodeSelect = $("#nodeSelect");
        ViewCertificate.InitializeNodeSelect();

        ViewCertificate.InitializeNodeSelect();
    },

    ShowDeployCertificateModal: function () {
        UiGlobal.ShowModal('deployCertificateModal');
    },

    DeployToNode: function () {

    },

    GetCertificateId: function ()
    {
        return $('#certificate-id-hidden').val();
    },

    InitializeSelects: function ()
    {
        ViewCertificate.AceTypeChangeAceSelect = $('#aceType');
        ViewCertificate.IdentityTypeChangeAceSelect = $('#aceIdentityType');
        ViewCertificate.AceIdentitySelect = $('#identity-select');

        CmOptions.AceTypes.forEach(function (item) {
            ViewCertificate.AceTypeChangeAceSelect.append(
                $('<option>', { value: item, text: item })
            );
        });

        CmOptions.IdentityTypes.forEach(function (item) {
            ViewCertificate.IdentityTypeChangeAceSelect.append(
                $('<option>', { value: item, text: item })
            );
        });


        ViewCertificate.AceIdentitySelect.select2({
            width: 'resolve',
            dropdownAutoWidth: true,
            placeholder: 'search for a certificate manager security principal',
            ajax: {
                url: ("/security/principals"),
                dataType: 'json',
                type: 'get',
                delay: 30,
                data: function (params) {
                    return {
                        query: params.term, // search term
                        page: params.page
                    };
                },
                processResults: function (data, params) {
                    params.page = params.page || 1;

                    return {
                        results: data.payload,
                    };
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            minimumInputLength: 2,
            templateResult: ViewCertificate.FormatSelectResults, // omitted for brevity, see the source of this page
            templateSelection: ViewCertificate.FormatSelection // omitted for brevity, see the source of this page
        });
        
    },

    FormatSelectResults: function (repo) {

        var markup = '<div class="user-select-container">'
        markup = markup + '<div class="user-select-title">' + repo.name + '</div>';
        markup = markup + '<div class="user-select-details">Type: <span class="user-select-details-value">' + repo.identityType + '</span></div>';
        markup = markup + '</div>';
        return markup;
    },

    FormatSelection: function (repo)
    {
        return repo.name;
    },

    AceIdentitySelect: null,

    AceTypeChangeAceSelect: null,

    IdentityTypeChangeAceSelect: null,

    CertificateData: null,

    SubjectAlternativeNameTable: null,

    AclTable: null,

    CertificateAceModal: null,

    NodeSelect: null,

    GetCertificateSuccessCallback: function (data)
    {
        CertificateData = data;

        $('#displayName').text(data.displayName);
        $('#thumbprint').text(data.thumbprint);
        $('#hashAlgorithm').text(data.hashAlgorithm);
        $('#cipherAlgorithm').text(data.cipherAlgorithm);
        $('#hasPrivateKey').text(data.hasPrivateKey);
        $('#expires').text(UiGlobal.GetDateString(data.expires));
        $('#windowsApi').text(UiGlobal.GetWindowsApiDisplay(data.windowsApi));
        $('#keySize').text(data.keySize);
        $('#storageFormat').text(data.certificateStorageFormat);

        ViewCertificate.InitializeSubjectAlternativeNamesTable(ViewCertificate.SubjectAlternativeNameTable, data.subject.subjectAlternativeName);

        //ViewCertificate.InitialzeAclTable(ViewCertificate.AclTable, data.acl);
    },

    GetCertificateErrorCallback: function ()
    {
        UiGlobal.ShowError();
    },

    ShowAddCertificateAceModal: function ()
    {
        ViewCertificate.CertificateAceModal.modal("show");
    },

    InitializeDownloadUx: function ()
    {
        $('#showCertificateDownloadOptionsButton').click(function () {

            $('#certificateDownloadOptionsModal').modal('show');

        });


        $('#cer-radio').change(function () {
            $("#download-certificate-button").prop('disabled', false);
            $("#download-cer-option-der").prop('disabled', false);
            $("#download-cer-option-b64").prop('disabled', false);
            $("#download-cer-option-b64").prop('checked', true);
            $("#cer-background").css("background-color", "#b0c4de");
            $("#pfx-background").css("background-color", "#f5f5f5");
        });

        $('#pfx-radio').change(function () {
            $("#download-certificate-button").prop('disabled', false);
            $("#download-pfx-includechain").prop('disabled', false);
            $("#download-cer-option-b64").prop('disabled', true);
            $("#download-cer-option-der").prop('disabled', true);
            $("#pfx-background").css("background-color", "#b0c4de");
            $("#cer-background").css("background-color", "#f5f5f5");
        });


        $('#download-certificate-button').click(function (e) {
            var type;
            var encoding;
            if ($("#cer-radio").prop("checked")) {
                if ($("#download-cer-option-b64").prop('checked')) {
                    type = "certb64";
                }
                else {
                    type = "certbinary";
                }
                type = "certbinary";
            }
            else if ($("#pfx-radio").prop("checked")) {
                type = "pfx";
            }
            else {
                type = "";
            }

            if ($('#include-chain-checkbox').prop('checked')) {
                type = type + "chain"
            }

            var downloadUri = "/certificate/" + $('#certificate-id').text();

            switch (type) {
                case "certbinarychain":
                    downloadUri = downloadUri + "/download/cer/binary/includechain";
                    break;
                case "certbinary":
                    downloadUri = downloadUri + "/download/cer/binary/nochain";
                    break;
                case "certb64chain":
                    downloadUri = downloadUri + "/download/cer/base64/includechain";
                    break;
                case "certb64":
                    downloadUri = downloadUri + "/download/cer/base64/nochain";
                    break;
                case "pfxchain":
                    downloadUri = downloadUri + "/download/pfx/includechain";
                    break;
                case "pfx":
                    downloadUri = downloadUri + "/download/pfx/nochain";
                    break;
                default:
                    downloadUri = downloadUri + "/download/cer/binary/nochain";
            }

            e.preventDefault();
            window.location.href = downloadUri;

        });
    },

    InitializeSubjectAlternativeNamesTable: function (table, sanList) {
        table.jsGrid({
            width: "100%",
            paging: true,
            autoload: true,
            pageLoading: true,

            controller: {
                loadData: function () {
                    return { data: sanList }
                }
            },

            fields: [
                {
                    title: "Subject Alternative Name",
                    itemTemplate: function (value, item) {
                        return item;
                    }
                }
            ]
        });
    },

    ChangeList: function ()
    {

    },


    InitialzeAclTable: function (table)
    {
        ViewCertificate.AclTable.jsGrid({
            width: "100%",
            paging: true,
            autoload: true,
            //pageLoading: true,
            deleteConfirm: "Are you sure you want to remove this ACE from the Access Control List?",
            //"certificate/{certId:guid}/acl/{aceId:guid}"
            controller: {

                loadData: function (filter) {
                    var d = $.Deferred();
                    $.ajax({
                        type: "GET",
                        url: "/certificate/" + ViewCertificate.GetCertificateId(),
                        dataType: "json"
                    }).done(function (response) {
                        d.resolve(response.payload.acl);
                    });
                    return d.promise();
                },
                //loadData: function () {
                //    return { data: acl }
                //},

                insertItem: function (item) {
                    var d = $.Deferred();
                    $.ajax({
                        type: "PUT",
                        url: "/certificate/" + item.id + "/acl",
                        dataType: "json",
                        contentType: 'application/json; charset=UTF-8',
                        data: JSON.stringify({
                            identityType: item.identityType,
                            aceType: item.aceType,
                            identity: item.identity
                        })
                    }).done(function (response) {
                        d.resolve(response.payload);
                    });
                    return d.promise();

                },

                deleteItem: function (item) {
                    $.ajax({
                        type: "DELETE",
                        url: "/certificate/" + ViewCertificate.GetCertificateId() + "/acl/" + item.id
                    }).fail(function (xhr, ajaxOptions, thrownError) {
                        ViewCertificate.HandleError(xhr.responseJSON.message, AuthenticablePrincipal.Grid);
                    });
                }
            },

            fields: [
                { title: "Identity", name: "identityDisplayName", type: "text" },
                { title: "IdentityType", name: "identityType", type: "text" },
                { title: "AceType", name: "aceType", type: "text" },
                {
                    width: 25,
                    editButton: false,
                    type: "control",
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                ViewCertificate.ShowAddCertificateAceModal("Add", {});
                            });
                    }
                }
            ]
        });
    },

    InitializeShowPassword: function ()
    {
        $('#showPasswordButton').click(function () {
            $('#password').empty();
            Services.GetCertificatePassword(ViewCertificate.GetCertificateId(), ViewCertificate.GetCertificatePasswordSuccessCallback, ViewCertificate.GetCertificatePasswordErrorCallback);
        });
    },

    InitializeResetPassword: function ()
    {
        $('#resetPasswordButton').click(function () {
            UiGlobal.ResetAlertState();
            Services.ResetCertificatePassword(ViewCertificate.GetCertificateId(), ViewCertificate.ResetCertificatePasswordSuccessCallback, ViewCertificate.ResetCertificatePasswordErrorCallback);
        });
    },

    ResetCertificatePasswordSuccessCallback: function ()
    {
        UiGlobal.ShowSuccess("Certificate password was reset successfully");
    },

    ResetCertificatePasswordErrorCallback: function ()
    {
        UiGlobal.ShowError("Could not process certificate password reset request");
    },

    GetCertificatePasswordSuccessCallback: function (data)
    {
        $('#password').text(data.decryptedPassword);
    },

    GetCertificatePasswordErrorCallback: function (x, t, m) {
        $('#password').text("Could not retrieved password, access denied.");
        $('#password').css("color", "red");
    },

    AddCertificateAce: function () {
        var data = {
            id: ViewCertificate.GetCertificateId(),
            identity: ViewCertificate.AceIdentitySelect.val(),
            identityType: $('#aceIdentityType').val(),
            aceType: $('#aceType').val()
        };

        ViewCertificate.AclTable.jsGrid("insertItem", data);
    },

    InitializeNodeSelect: function () {
        

        ViewCertificate.NodeSelect.select2({
            placeholder: 'search for a certificate manager user',
            ajax: {

                url: ("/nodes"),
                dataType: 'json',
                type: 'get',
                delay: 30,
                data: function (params) {
                    return {
                        query: params.term, // search term
                        page: params.page
                    };
                },
                processResults: function (data, params) {
                    params.page = params.page || 1;

                    return {
                        results: data.payload,
                    };
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            minimumInputLength: 2,
            templateResult: ViewCertificate.NodeSelectFormatRepo, // omitted for brevity, see the source of this page
            templateSelection: ViewCertificate.NodeSelectFormatRepoSelection // omitted for brevity, see the source of this page
        });
    },

    NodeSelectFormatRepo: function (repo) {
        return repo.hostname;
    },

    NodeSelectFormatRepoSelection: function (repo) {
        return repo.hostname;
    },

    DeployToNode: function () {

        var uri = '/node/' + ViewCertificate.NodeSelect.val() + '/deploy/' + ViewCertificate.GetCertificateId();

        Services.Post(uri, null, ViewCertificate.DeployCertSuccessCallback, null);

    },

    DeployCertSuccessCallback: function (payload) {
        UiGlobal.ShowSuccess('Certificate successfully deployed to node');
    }
}