var SecurityRoles = {

    ShowAddSecurityRoleModal: function (dialogType, client) {

        //$("#eisName").val(client.name);
        //$("#eisDomain").val(client.hash);
        //$("#eis").val(client.cipher);
        //$("#adcsTemplateKeyUsage").val(client.keyUsage);
        //$("#adcsTemplateWindowsApi").val(client.windowsApi);

        $('#AddSecurityRoleButton').click(function () {

            SecurityRoles.AddRole(client, dialogType === "Add");
        });

        $("#addSecurityRoleModal").modal("show");
    },

    AddRole: function (client, isNew) {
        $.extend(client, {
            name: $("#secRoleName").val(),
            enabled: $("#secRoleEnabled").is(":checked")
        });

        $("#securityRolesTable").jsGrid(isNew ? "insertItem" : "updateItem", client);

        $("#addSecurityRoleModal").modal("hide");
    },

    InitializeGrid: function ()
    {
        $("#securityRolesTable").jsGrid({
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

            controller: SecurityRoles.Controller,

            fields: [
                { name: "name", type: "text", title: "name" },
                //{ name: "domain", type: "text", title: "Domain" },
                //{ name: "searchBase", type: "text", title: "SearchBase" },
                //{ name: "externalIdentitySourceType", type: "select", items: CmOptions.ExternalIdentitySourceType, valueType: "string", valueField: "Name", textField: "Name", title: "Type" },
                //{ name: "username", type: "text", title: "Username" },
                //{ name: "password", type: "text", readOnly: true, title: "password" },
                { name: "enabled", type: "checkbox", title: "Enabled", sorting: false },
                {
                    //css: "security-roles-select2",
                    name: "details",
                    title: "Action", 
                    width: 50,
                    itemTemplate: function (value, item) {
                        //var $text = $("<p>").text(item.MyField);
                        var $link = $("<a>").attr("href", '/view/security/role/' + item.id).text("View");
                        return $("<div>").append($link);
                    }
                },
                {
                    type: "control",
                    
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                SecurityRoles.ShowAddSecurityRoleModal("Add", {});
                            });
                    }
                }
            ],

            onItemInserting: function (args) { SecurityRoles.ResetErrorState(); },
            onItemUpdating: function (args) { SecurityRoles.ResetErrorState(); },
            onItemDeleting: function (args) { SecurityRoles.ResetErrorState(); }

        });
    },

    Controller: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/security/roles",
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
                url: "/security/role",
                data: item
            }).done(function (response) {
                d.resolve(response.payload);
            }).fail(function (xhr, ajaxOptions, thrownError) {
                SecurityRoles.HandleError(xhr.responseJSON.message, $("#securityRolesTable"));
            });
            return d.promise();
        },

        updateItem: function (item) {

            $.ajax({
                type: "PUT",
                url: "/security/role",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                SecurityRoles.HandleError(xhr.responseJSON.message, $("#securityRolesTable"));
            });
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/security/role",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                SecurityRoles.HandleError(xhr.responseJSON.message, $("#securityRolesTable"));
            });
        }
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
    }


}