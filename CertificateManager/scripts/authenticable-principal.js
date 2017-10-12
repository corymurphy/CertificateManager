
var debugvar = "";

var AuthenticablePrincipal = {

    ShowAuthenticablePrincipalActionModal: function (dialogType, client) {
        UiGlobal.ResetAlertState();
        switch (dialogType)
        {
            case "Add":
                $('#commitAuthenticablePrincipalButton').click(function () {
                    AuthenticablePrincipal.AddAuthPrincipal(client);
                });
                break;
            case "Edit":
                AuthenticablePrincipal.LoadEditAuthPrincipalModalData(client);
                $('#commitAuthenticablePrincipalButton').click(function () {
                    AuthenticablePrincipal.EditAuthPrincipal(client);
                });
                break;
            default:
                $('#commitAuthenticablePrincipalButton').click(function () {
                    AuthenticablePrincipal.AddAuthPrincipal(client);
                });
        }
        $("#authenticablePrincipalActionModal").modal("show");
    },

    LoadEditAuthPrincipalModalData: function (client)
    {
        $('#authPrincipalUpn').val(client.userPrincipalName);
        $('#authPrincipalEnabled').val(client.enabled);
        $('#localLogonEnabled').val(client.localLogonEnabled);
        AuthenticablePrincipal.InitializeAlternativeUpnActionModalSelect2(client);
    },

    EditAuthPrincipal: function (client)
    {
        $.extend(client, {
            userPrincipalName: $("#authPrincipalUpn").val(),
            enabled: $("#authPrincipalEnabled").is(":checked"),
            localLogonEnabled: $("#localLogonEnabled").is(":checked"),
            alternativeUserPrincipalNames: UiGlobal.GetSelectedOptions(AuthenticablePrincipal.AlternativeUpnActionModalSelect2())        
        });

        AuthenticablePrincipal.Grid.jsGrid("updateItem", client);

        $("#authenticablePrincipalActionModal").modal("hide");
    },

    AddAuthPrincipal: function (client) {
        $.extend(client, {
            userPrincipalName: $("#authPrincipalUpn").val(),
            enabled: $("#authPrincipalEnabled").is(":checked"),
            localLogonEnabled: $("#localLogonEnabled").is(":checked")
        });

        AuthenticablePrincipal.Grid.jsGrid("insertItem", client);

        $("#authenticablePrincipalActionModal").modal("hide");
    },

    InitializeGrid: function ()
    {
        AuthenticablePrincipal.Grid = $("#authenticablePrincipalTable");

        AuthenticablePrincipal.Grid.jsGrid({
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

            controller: AuthenticablePrincipal.Controller,

            //rowClick: function (args) {
            //    AuthenticablePrincipal.ShowAuthenticablePrincipalActionModal("Edit", args.item);
            //},

            fields: [
                { name: "userPrincipalName", type: "text", title: "UPN" },
                {
                    name: "alternativeUserPrincipalNames",
                    title: "Alternative UPN",
                    itemTemplate: function (value, item) {

                        var altUpnSelect = $("<select style='width:100%' class='alternativeupn-select2' multiple='multiple'>");


                        if (item.alternativeUserPrincipalNames != null)
                        {
                            item.alternativeUserPrincipalNames.forEach(function (option) {
                                altUpnSelect.append($('<option>', {
                                    value: option,
                                    text: option
                                }).attr('selected', true));
                            });
                        }
                        

                        altUpnSelect = altUpnSelect.attr('disabled', true);
                        return altUpnSelect;

                    }
                },
                { name: "localLogonEnabled", type: "checkbox", title: "LocalLogonEnabled", width: 40 },
                { name: "enabled", type: "checkbox", title: "Enabled", sorting: false, width: 25 },
                {
                    title: "Action",
                    width: 15,
                    itemTemplate: function (value, item) {

                        var btn = $("<i>");
                        btn.addClass("fa fa-key");
                        btn.attr('cm-id', item.id);
                        btn.attr('cm-upn', item.userPrincipalName);
                        
                        return btn.on("click", function (event) {
                            var id = event.target.attributes["cm-id"].value
                            var upn = event.target.attributes["cm-upn"].value
                            AuthenticablePrincipal.ResetPasswordButtonClick(id, upn);
                            return false;
                        });

                        //var $text = $("<p>").text('derp');
                        //var $link = $("<a>").attr("href", item.MyItemUrl).text("Go To Item");
                        //return $("<div>").append($text).append($link);

                    }

                },
                {
                    width: 25,
                    type: "control",
                    editButton: false,
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                AuthenticablePrincipal.ShowAuthenticablePrincipalActionModal("Add", {});
                            });
                    }
                }
            ],
            onRefreshed: function (args) { AuthenticablePrincipal.InitializeAlternativeUpnSelect2(); },
            onItemUpdated: function (args) { AuthenticablePrincipal.InitializeAlternativeUpnSelect2(); },
            onItemEditing: function (args) { AuthenticablePrincipal.InitializeAlternativeUpnSelect2(); },
            onItemInserting: function (args) { UiGlobal.ResetAlertState(); },
            onItemUpdating: function (args) { UiGlobal.ResetAlertState(); AuthenticablePrincipal.InitializeAlternativeUpnSelect2(); },
            onItemDeleting: function (args) { UiGlobal.ResetAlertState(); },
            onDataLoading: function (args) { AuthenticablePrincipal.InitializeAlternativeUpnSelect2(); },
            onDataLoaded: function (args) { AuthenticablePrincipal.InitializeAlternativeUpnSelect2(); }
        });

        AuthenticablePrincipal.InitializeAlternativeUpnSelect2();
    },

    Controller: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/security/authenticable-principals",
                dataType: "json"
            }).done(function (response) {
                d.resolve(response.payload);
            });
            return d.promise();
        },
        insertItem: function (item) {
            $.ajax({
                type: "POST",
                url: "/security/authenticable-principal",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                AuthenticablePrincipal.HandleError(xhr.responseJSON.message, AuthenticablePrincipal.Grid);
            });
        },

        updateItem: function (item) {

            $.ajax({
                type: "PUT",
                url: "/security/authenticable-principal",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                AuthenticablePrincipal.HandleError(xhr.responseJSON.message, AuthenticablePrincipal.Grid);
            });
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/security/authenticable-principal",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                AuthenticablePrincipal.HandleError(xhr.responseJSON.message, AuthenticablePrincipal.Grid);
            });
        },

        onItemInserting: function (args) { AuthenticablePrincipal.ResetErrorState(); },
        onItemUpdating: function (args) { AuthenticablePrincipal.ResetErrorState(); },
        onItemDeleting: function (args) { AuthenticablePrincipal.ResetErrorState(); }
    },

    ResetErrorState: function () {
        UiGlobal.HideError();
    },

    HandleError: function (msg, grid) {
        grid.jsGrid("render");
        UiGlobal.ShowError(msg);
    },

    InitializeSelect: function () {
        CmOptions.ExternalIdentitySourceType.forEach(function (item) {
            $('#eisType').append($('<option>', {
                value: item.Name,
                text: item.Name
            }));
        });
    },

    ImportUserSelections: [],

    InitializeUserSearchSelect: function () {

        $('.idp-select').select2({ data: CmOptions.ExternalIdentitySources });

        AuthenticablePrincipal.ImportUserSelect = $(".user-search-select");

        AuthenticablePrincipal.ImportUserSelect.select2({
            placeholder: 'search for an ad object',
            ajax: {

                url: ("/identity-source/external/query/users"),
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
                    // parse the results into the format expected by Select2
                    // since we are using custom formatting functions we do not need to
                    // alter the remote JSON data, except to indicate that infinite
                    // scrolling can be used
                    params.page = params.page || 1;

                    return {
                        results: data.payload,
                        //pagination: {
                        //    more: (params.page * 30) < data.total_count
                        //}
                    };
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
            minimumInputLength: 2,
            templateResult: AuthenticablePrincipal.formatRepo, // omitted for brevity, see the source of this page
            templateSelection: AuthenticablePrincipal.formatRepoSelection // omitted for brevity, see the source of this page
        });

        AuthenticablePrincipal.ImportUserSelect.on("select2:selecting", function (e) {
            AuthenticablePrincipal.ImportUserSelections.push(e.params.args.data);
        });
    },

    formatRepo: function (repo) {

        var markup = '<div class="user-select-container">'
        markup = markup + '<div class="user-select-title">' + repo.samAccountName + ' ; ' + repo.name + ' ; ' + repo.displayName + '</div>';
        markup = markup + '<div class="user-select-details">UserPrincipalName: <span class="user-select-details-value">' + repo.userPrincipalName + '</span></div>';
        markup = markup + '<div class="user-select-details">Domain: <span class="user-select-details-value">' + repo.domain + '</span></div>';
        markup = markup + '</div>';
        //var markup = repo.text;
        return markup;
    },

    formatRepoSelection: function (repo) {
        return repo.samAccountName;
        //return repo.name || repo.text;
        //return repo.text;
    },

    FormatExistingUserMergeRepository: function (repo)
    {
        return repo.userPrincipalName;
    },

    FormatExistingUserMergeSelection: function (repo)
    {
        return repo.userPrincipalName;
    },

    ImportSelectedUsers: function ()
    {
        UiGlobal.ResetAlertState();

        
        if (AuthenticablePrincipal.MergeOnImport)
        {
            var data = {
                users: AuthenticablePrincipal.ImportUserSelections,
                mergeWith: AuthenticablePrincipal.ExistingUserMergeSelect2().val(),
                merge: AuthenticablePrincipal.MergeOnImport
            };
        }
        else
        {
            var data = {
                users: AuthenticablePrincipal.ImportUserSelections,
                merge: AuthenticablePrincipal.MergeOnImport
            };
        }
        


        Services.ImportUsersFromExternalIdentitySource(data, AuthenticablePrincipal.ImportSelectedUsersSuccessCallback, AuthenticablePrincipal.ImportSelectedUsersErrorCallback);


    },

    ImportSelectedUsersSuccessCallback: function ()
    {
        AuthenticablePrincipal.ImportUserSelections = [];
        AuthenticablePrincipal.ResetUserSelect();
        UiGlobal.RefreshGrid(AuthenticablePrincipal.Grid);

        UiGlobal.ShowSuccess("Successfully imported the selected users");

    },

    ImportSelectedUsersErrorCallback: function (e) {
        AuthenticablePrincipal.ImportUserSelections = [];
        AuthenticablePrincipal.ResetUserSelect();
        UiGlobal.ShowError("Error while importing users: " + e.responseJSON.message);
    },

    Grid: null,

    ImportUserSelect: null,

    ResetUserSelect: function ()
    {
        AuthenticablePrincipal.ImportUserSelect.select2('val', 'All');
    },

    AlternativeUpnSelect2: function () {
        return $('.alternativeupn-select2');
    },

    InitializeAlternativeUpnActionModalSelect2: function (client)
    {
        if (client.alternativeUserPrincipalNames != null) {
            client.alternativeUserPrincipalNames.forEach(function (option) {

                if (!AuthenticablePrincipal.AlternativeUpnActionModalSelect2().val().includes(option))
                {
                    AuthenticablePrincipal.AlternativeUpnActionModalSelect2().append($('<option>', {
                        value: option,
                        text: option
                    }).attr('selected', true));
                }
                
            });
        }

        AuthenticablePrincipal.AlternativeUpnActionModalSelect2().select2({ width: '100%', tags: true });
    },

    AlternativeUpnActionModalSelect2ClearOptions: function ()
    {
        var select = AuthenticablePrincipal.AlternativeUpnActionModalSelect2()[0];
        var length = select.options.length;
        for (i = 0; i < length; i++) {
            select.options[i] = null;
        }
    },

    AlternativeUpnActionModalSelect2: function ()
    {
        return $('.alternativeupn-actionmodal-select2');
    },

    InitializeAlternativeUpnSelect2: function ()
    {
        AuthenticablePrincipal.AlternativeUpnSelect2().select2({ width: '100%' });
    },

    RegisterModalCloseEvent: function ()
    {
        $('#authenticablePrincipalActionModal').on('hidden.bs.modal', function () {
            $('#authPrincipalUpn').val('');
            $('#authPrincipalEnabled').val('');
            $('#localLogonEnabled').val('');
            AuthenticablePrincipal.AlternativeUpnActionModalSelect2ClearOptions();
        });
    },

    RegisterImportUserModalCloseEvent: function ()
    {
        $('#importUsersModal').on('hidden.bs.modal', function () {
            $('#authPrincipalUpn').val('');
            $('#authPrincipalEnabled').val('');
            $('#localLogonEnabled').val('');
            AuthenticablePrincipal.AlternativeUpnActionModalSelect2ClearOptions();
        });
    },

    PageLoad: function ()
    {
        AuthenticablePrincipal.InitializeGrid();
        AuthenticablePrincipal.InitializeUserSearchSelect();
        AuthenticablePrincipal.RegisterModalCloseEvent();
        AuthenticablePrincipal.RegisterMergeCheckboxEvent();
        AuthenticablePrincipal.InitializeExistingUserMergeSelect2();
    },

    MergeOnImport: false,

    ShowMergeExistingUserSelect: function ()
    {
        $('.existingUserSelect').show();
    },

    HideMergeExistingUserSelect: function ()
    {
        $('.existingUserSelect').hide();
    },

    RegisterMergeCheckboxEvent: function () {
        $('#mergeCheckbox').change(function () {
            if ($(this).is(":checked")) {
                AuthenticablePrincipal.MergeOnImport = true;
                AuthenticablePrincipal.ShowMergeExistingUserSelect();
            }
            else
            {
                AuthenticablePrincipal.MergeOnImport = false;
                AuthenticablePrincipal.HideMergeExistingUserSelect();
            }
        });
    },

    ExistingUserMergeSelect2: function ()
    {
        return $('#existingUserSelect');
    },

    InitializeExistingUserMergeSelect2: function ()
    {
        AuthenticablePrincipal.ExistingUserMergeSelect2().select2({
            width: '100%',
            placeholder: 'Search for a Certificate Manager user',
            ajax: {
                url: ("/security/authenticable-principals"),
                dataType: 'json',
                type: 'get',
                delay: 30,
                //data: function (params) {
                //    return {
                //        query: params.term, // search term
                //        page: params.page
                //    };
                //},
                processResults: function (data, params) {
                    params.page = params.page || 1;

                    return {
                        results: data,
                    };
                },
                cache: true
            },
            escapeMarkup: function (markup) { return markup; },
            //minimumInputLength: 2,
            templateResult: AuthenticablePrincipal.FormatExistingUserMergeRepository,
            templateSelection: AuthenticablePrincipal.FormatExistingUserMergeSelection,

        });
    },

    ResetPasswordButtonClick: function (id, upn)
    {
        //console.log('reset password button click');
        //console.log(id);
        //console.log(upn);


        $('#reset-pwd-id').val(id)
        $('#reset-pwd-upn').val(upn);

        $("#resetUserPasswordModal").modal("show");

        
        return false;
    },

    ResetUserPassword: function ()
    {
        var id = $('#reset-pwd-id').val();
        var newPassword = $('#reset-pwd-password').val();

        var data = {
            id: id,
            newPassword: newPassword
        };

        $('#reset-pwd-password').val('');

        Services.ResetUserPassword(data, UiGlobal.ShowSuccess, UiGlobal.ShowError);
    }



}