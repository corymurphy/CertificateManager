
var ViewAllCertificates = {

    PageLoad: function ()
    {
        ViewAllCertificates.Grid = $('#allCertificatesTable');

        ViewAllCertificates.InitializeGrid();
    },

    Grid: null,

    Controller: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/certificates",
                dataType: "json"
            }).done(function (response) {
                d.resolve(response.payload);
            });
            return d.promise();
        },
        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/certificate",
                data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ViewAllCertificates.HandleError(xhr.responseJSON.message, ViewAllCertificates.Grid);
            });
        }
    },

    InitializeGrid: function ()
    {
        ViewAllCertificates.Grid.jsGrid({
            height: "auto",
            width: "100%",

            //filtering: true,
            editing: false,
            sorting: true,
            paging: true,
            autoload: true,

            pageSize: 15,
            pageButtonCount: 5,

            deleteConfirm: "Do you really want to delete this certificate?",

            controller: ViewAllCertificates.Controller,

            fields: [
                { title: "Display Name", name: "displayName", type: "text" },
                { title: "Hash", name: "hashAlgorithm", type: "text", width: 25 },
                { title: "Cipher", name: "cipherAlgorithm", type: "text", width: 25 },
                {
                    title: "Expires", name: "validTo", type: "text", width: 40,
                    itemTemplate: function (value, item) {
                        return UiGlobal.GetDateString(value);
                    } },
                { title: "Thumbprint", name: "thumbprint", type: "text" },
                {
                    name: "details",
                    title: "Action",
                    width: 20,
                    itemTemplate: function (value, item) {
                        //var $text = $("<p>").text(item.MyField);
                        var $link = $("<a>").attr("href", '/view/certificate/' + item.id).text("View");
                        return $("<div>").append($link);
                    }
                },
                {
                    type: "control",
                    editButton: false,
                    width: 10
                }
            ],

            onItemInserting: function (args) { SecurityRoles.ResetErrorState(); },
            onItemUpdating: function (args) { SecurityRoles.ResetErrorState(); },
            onItemDeleting: function (args) { SecurityRoles.ResetErrorState(); }

        });
    }
}