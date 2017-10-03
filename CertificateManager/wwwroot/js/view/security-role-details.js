var SecurityRoleDetails = {

    ShowAddRoleMemberModal: function (dialogType, client) {

        $('#addSecurityRoleMemberButton').click(function () {

            SecurityRoleDetails.AddSecurityRoleMember(client, dialogType === "Add");
        });

        $("#addSecurityRoleMemberModal").modal("show");
    },

    AddSecurityRoleMember: function (client, isNew) {
        $.extend(client, {
            memberId: $("#memberId").val(),
            roleId: $('#roleId').text()
        });

        $("#roleMembersTable").jsGrid(isNew ? "insertItem" : "updateItem", client);

        $("#addSecurityRoleMemberModal").modal("hide");
    },

    RenderViewData: function (data)
    {
        $('#roleName').text(data.payload.name);
    },

    InitializeGrid: function () {
        $("#roleMembersTable").jsGrid({
            height: "auto",
            width: "100%",
            sorting: true,
            paging: true,
            autoload: true,
            pageSize: 15,
            pageButtonCount: 5,

            deleteConfirm: "Do you really want to delete this external identity source?",

            controller: SecurityRoleDetails.MembersController,

            fields: [
                { name: "userPrincipalName", type: "text", title: "userPrincipalName", editing: false },
                { name: "enabled", type: "checkbox", title: "Enabled", sorting: false, editing: false },
                {
                    type: "control",
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                SecurityRoleDetails.ShowAddRoleMemberModal("Add", {});
                            });
                    }
                }
            ],

            onItemInserting: function (args) { SecurityRoleDetails.ResetErrorState(); },
            onItemUpdating: function (args) { SecurityRoleDetails.ResetErrorState(); },
            onItemDeleting: function (args) { SecurityRoleDetails.ResetErrorState(); }
        });
    },

    UserAddRoleMemberSelect: null,

    InitializeUserAddRoleMemberSelect: function ()
    {
        SecurityRoleDetails.UserAddRoleMemberSelect = $("#user-select");

        SecurityRoleDetails.UserAddRoleMemberSelect.select2({
            placeholder: 'search for a certificate manager user',
            ajax: {

                url: ("/security/authenticable-principals/search"),
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
            templateResult: SecurityRoleDetails.UserSelectFormatRepo, // omitted for brevity, see the source of this page
            templateSelection: SecurityRoleDetails.UserSelectFormatRepoSelection // omitted for brevity, see the source of this page
        });
    },

    UserSelectFormatRepo: function (repo)
    {
        return repo.name;
    },

    UserSelectFormatRepoSelection: function (repo)
    {
        return repo.name;
    },

    MembersController: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/security/role/" + $('#roleId').text() + "/members",
                dataType: "json"
            }).done(function (response) {
                d.resolve(response.payload);
            });
            return d.promise();
        },
        insertItem: function (item) {
            //MembersController.onItemInserting();
            var d = $.Deferred();
            $.ajax({
                type: "POST",
                url: "/security/role/" + $('#roleId').text() + "/member/" + $('#memberId').val()
            }).done(function (response) {
                d.resolve(response.payload);
            }).fail(function (xhr, ajaxOptions, thrownError) {
                SecurityRoleDetails.HandleError(xhr.responseJSON.message, $("#roleMembersTable"));
            });
            return d.promise();
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/security/role/" + $('#roleId').text() + "/member/" + item.id
            }).fail(function (xhr, ajaxOptions, thrownError) {
                SecurityRoleDetails.HandleError(xhr.responseJSON.message, $("#roleMembersTable"));
            });
        }
    },

    ResetErrorState: function () {
        UiGlobal.HideError();
    },

    HandleError: function (textStatus, grid) {
        grid.jsGrid("render");
        UiGlobal.ShowError(textStatus);
    },

    PageLoad: function ()
    {
        UiGlobal.ShowCurrentTab();
        Services.GetSecurityRoleDetails($('#roleId').text(), SecurityRoleDetails.RenderViewData, null);
        SecurityRoleDetails.InitializeGrid();
        SecurityRoleDetails.InitializeUserAddRoleMemberSelect();
    }
}