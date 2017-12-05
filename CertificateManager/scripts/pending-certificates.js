var PendingCertificates = {

    PageLoad: function ()
    {
        PendingCertificates.Grid = $('#allPendingCertificates');
        PendingCertificates.InitializeGrid();
    },

    Grid: null,
    
    Controller:
    {
        loadData: function (filter)
        {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/certificate/request/pending",
                dataType: "json"
            }).done(function (response) {
                d.resolve(response.payload);
            });
            return d.promise();
        },
        deleteItem: function (item) {
            $.ajax({
                type: "DELETE",
                url: "/certificate/request/pending/" + item.id
                //data: item
            }).fail(function (xhr, ajaxOptions, thrownError) {
                ViewAllCertificates.HandleError(xhr.responseJSON.message, ViewAllCertificates.Grid);
            });
        }
    },

    InitializeGrid: function ()
    {
        PendingCertificates.Grid.jsGrid({
            height: "auto",
            width: "100%",

            //filtering: true,
            editing: false,
            sorting: true,
            paging: true,
            autoload: true,

            pageSize: 15,
            pageButtonCount: 5,

            deleteConfirm: "Do you want to deny this certificate request?",

            controller: PendingCertificates.Controller,

            fields: [
                { title: "CommonName", name: "subjectCommonName", type: "text" },
                { title: "RequestDate", name: "requestDate", type: "text"  },
                { title: "Cipher", name: "cipherAlgorithm", type: "text" },
                { title: "Hash", name: "hashAlgorithm", type: "text" },
                { title: "RequestType", name: "pendingCertificateRequestType", type: "text" },
                { title: "KeyUsage", name: "keyUsage", type: "text" },
                {
                    title: "Action",
                    width: 15,
                    itemTemplate: function (value, item) {
                        var btn = $("<i>");
                        btn.addClass("fa fa-check");
                        btn.attr('req-id', item.id);

                        btn.on('click', function (event) {
                            UiGlobal.ShowWarning("Please wait, the certificate request is processing");
                            var id = event.target.attributes["req-id"].value;
                            Services.IssuePendingCertificate(id, PendingCertificates.IssueCertificateSuccessCallback, PendingCertificates.IssueCertificateErrorCallback);
                            return false;
                        });

                        return btn;
                    }

                },
                {
                    type: "control",
                    editButton: false,
                    width: 10
                }
            ]

            //onItemInserting: function (args) { SecurityRoles.ResetErrorState(); },
            //onItemUpdating: function (args) { SecurityRoles.ResetErrorState(); },
            //onItemDeleting: function (args) { SecurityRoles.ResetErrorState(); }

        });
    },

    IssueCertificateSuccessCallback: function (data)
    {
        UiGlobal.HideWarning();
        window.location.replace("/view/certificate/" + data.id);
    },

    IssueCertificateErrorCallback: function (x, t, m)
    {
        UiGlobal.HideWarning();
        UiGlobal.ShowError("Could not issue the pending certificate.");
    }
}