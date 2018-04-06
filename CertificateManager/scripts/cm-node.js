var Nodes = {

    PageLoad: function () {
        Nodes.Grid = $('#nodeTable');
        Nodes.Modal = $('#addNodeModal');
        Nodes.CommitButton = $('#addButton');
        Nodes.CredentialSelect = $('#credential');
        Nodes.InitializeGrid();
        Nodes.InitializeIdpSelect();
    },

    InitializeIdpSelect: function () {
        Services.GetActiveDirectoryIdentityProviders(Nodes.InitializeIdpSelectSuccessCallback, null);
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

            Nodes.CredentialSelect.append($('<option>', appendData));        
        });
    },

    CredentialSelect: null,

    Grid: null,

    Modal: null,

    ShowAddModal: function () {
        Nodes.ResetModalState();
        Nodes.Modal.modal("show");
        Nodes.SetCommitOnClick("Add");
    },

    CommitButton: null,

    ResetModalState: function () {
        $('#hostname').val('');
        $('#credential').val('');
    },

    Add: function () {
        Nodes.Grid.jsGrid("insertItem", Nodes.GetData());
    },

    Edit: function () {

    },


    GetData: function () {
        return {
            hostName: $('#hostname').val(),
            credentialId: $('#credential').val()
        };
    },


    SetCommitOnClick: function(eventType) {
        switch (eventType) {
            case "Add":
                Nodes.CommitButton.attr("onclick", "Nodes.Add();");
                break;
            case "Edit":
                Nodes.CommitButton.attr("onclick", "Nodes.Edit();");
                break;
            default:
                Nodes.CommitButton.attr("onclick", "Nodes.Add();");
        }
    },

    ViewNode: function (item) {
        document.location = "/view/node/" + item.id;
    },

    InitializeGrid: function ()
    {
        Nodes.Grid.jsGrid({
            height: "auto",
            width: "100%",

            
            editing: false,
            sorting: true,
            paging: true,
            autoload: true,

            pageSize: 15,
            pageButtonCount: 5,

            rowClick: function (args) {
                Nodes.ViewNode(args.item);
            },

            deleteConfirm: "Do you really want to delete this node?",

            controller: Nodes.Controller,

            fields: [

                { name: "hostname", type: "text", title: "Hostname" },

                { name: "credential", type: "text", title: "Credential" },

                { name: "lastCommunication", type: "text", title: "LastCommunication" },
                
                { name: "communicationSuccess", type: "text", title: "CommunicationSuccess" },

                {
                    name: "details",
                    title: "Action", 
                    width: 50,
                    itemTemplate: function (value, item) {
                        var $link = $("<a>").attr("href", '/view/node/' + item.id).text("View");
                        return $("<div>").append($link);
                    }
                },
                {
                    type: "control",
                    
                    headerTemplate: function () {
                        return $("<button>").attr("type", "button").text("Add")
                            .on("click", function () {
                                Nodes.ShowAddModal();
                            });
                    }
                }
            ],

            onItemInserting: function (args) { Nodes.ResetErrorState(); },
            onItemUpdating: function (args) { Nodes.ResetErrorState(); },
            onItemDeleting: function (args) { Nodes.ResetErrorState(); }

        });
    },

    Controller: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/nodes",
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
                url: "/node",
                data: item
            }).done(function (response) {
                d.resolve(response.payload);
            }).fail(function (xhr, ajaxOptions, thrownError) {
                Nodes.HandleError(xhr.responseJSON.message, Nodes.Table);
            });
            return d.promise();
        },

        updateItem: function (item) {

            $.ajax({
                type: "PUT",
                url: "/node",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                Nodes.HandleError(xhr.responseJSON.message, Nodes.Table);
            });
        },

        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/node",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                Nodes.HandleError(xhr.responseJSON.message, Nodes.Table);
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