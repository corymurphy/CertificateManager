var CmScripts = {

    PageLoad: function () {
        CmScripts.Grid = $('#table');
        CmScripts.Modal = $('#addModal');
        CmScripts.CommitButton = $('#addButton');
        //CmScripts.CredentialSelect = $('#credential');
        CmScripts.InitializeGrid();
        //CmScripts.InitializeIdpSelect();
    },

    InitializeIdpSelect: function () {
        Services.GetActiveDirectoryIdentityProviders(CmScripts.InitializeIdpSelectSuccessCallback, null);
    },

    InitializeIdpSelectSuccessCallback: function (data) {

        var primarySet = false;

        var appendData = {};

        data.forEach(function (item) {

          
            if (primarySet) {
                appendData = {
                    value: item.id,
                    text: item.name
                };
            }
            else {
                appendData = {
                    value: item.id,
                    text: item.name,
                    selected: "selected"
                };
                primarySet = true;
            }

            CmScripts.CredentialSelect.append($('<option>', appendData));        
        });
    },

    CredentialSelect: null,

    Grid: null,

    Modal: null,

    ShowAddModal: function () {
        CmScripts.ResetModalState();
        CmScripts.Modal.modal("show");
        CmScripts.SetCommitOnClick("Add");
    },

    CommitButton: null,

    ResetModalState: function () {
        $('#name').val('');
        $('#code').val('');
    },

    Add: function () {
        CmScripts.Grid.jsGrid("insertItem", CmScripts.GetData());
    },

    Edit: function () {

    },


    GetData: function () {
        return {
            name: $('#name').val(),
            code: $('#code').val()
        };
    },


    SetCommitOnClick: function(eventType) {
        switch (eventType) {
            case "Add":
                CmScripts.CommitButton.attr("onclick", "CmScripts.Add();");
                break;
            case "Edit":
                CmScripts.CommitButton.attr("onclick", "CmScripts.Edit();");
                break;
            default:
                CmScripts.CommitButton.attr("onclick", "CmScripts.Add();");
        }
    },

    InitializeGrid: function ()
    {
        CmScripts.Grid.jsGrid({
            height: "auto",
            width: "100%",

            editing: true,
            sorting: true,
            paging: true,
            autoload: true,

            pageSize: 15,
            pageButtonCount: 5,

            deleteConfirm: "Do you really want to delete this script?",

            controller: CmScripts.Controller,

            fields: [

                { name: "name", type: "text", title: "name" },

                { name: "id", type: "text", title: "id" },

                {
                    name: "details",
                    title: "Action", 
                    width: 50,
                    itemTemplate: function (value, item) {
                        var $link = $("<a>").attr("href", '/view/script/' + item.id).text("View");
                        return $("<div>").append($link);
                    }
                },
                {
                    type: "control",
                    
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                CmScripts.ShowAddModal();
                            });
                    }
                }
            ],

            onItemInserting: function (args) { CmScripts.ResetErrorState(); },
            onItemUpdating: function (args) { CmScripts.ResetErrorState(); },
            onItemDeleting: function (args) { CmScripts.ResetErrorState(); }

        });
    },

    Controller: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/scripts",
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
                url: "/script",
                data: item
            }).done(function (response) {
                d.resolve(response.payload);
            }).fail(function (xhr, ajaxOptions, thrownError) {
                CmScripts.HandleError(xhr.responseJSON.message, CmScripts.Table);
            });
            return d.promise();
        },

        updateItem: function (item) {

            $.ajax({
                type: "PUT",
                url: "/script",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                CmScripts.HandleError(xhr.responseJSON.message, CmScripts.Table);
            });
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/script",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                CmScripts.HandleError(xhr.responseJSON.message, CmScripts.Table);
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
        CmOptions.ActiveDirectoryMetadataType.forEach(function (item) {
            $('#eisType').append($('<option>', {
                value: item.Name,
                text: item.Name
            }));
        });
    }


}