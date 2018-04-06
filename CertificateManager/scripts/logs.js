var Logs = {


    Grid: null,

    Controller: {
        loadData: function (filter) {
            var d = $.Deferred();
            $.ajax({
                type: "GET",
                url: "/logs",
                dataType: "json"
            }).done(function (response) {
                d.resolve(response.payload);
            });
            return d.promise();
        }
    },

    ShowDetailsModal: function (data)
    {
        $('#eventTime').val(data.time);
        $('#eventResult').val(data.eventResult);
        $('#eventCategory').val(data.eventCategory);
        $('#contextUserId').val(data.userId);
        $('#contextUser').val(data.userDisplay);
        $('#targetId').val(data.target);
        $('#message').text(data.message);

        UiGlobal.ShowModal('LogDetailsModal');
    },

    InitializeGrid: function ()
    {
        Logs.Grid.jsGrid({
            height: "auto",
            width: "100%",

            rowClick: function (args) {
                Logs.ShowDetailsModal(args.item);
            },

            rowClass: function (item, itemIndex) {
                return UiGlobal.GetRowClass(item, itemIndex);
            },

            editing: false,
            sorting: true,
            paging: true,
            autoload: true,

            pageSize: 30,
            pageButtonCount: 5,

            deleteConfirm: "Do you really want to delete this log?",

            controller: Logs.Controller,

            fields: [
                { name: "target", type: "text", title: "Target" },
                //{ name: "targetDescription", type: "text", title: "Description" },
                { name: "userDisplay", type: "text", title: "UserDisplay" },
                { name: "userId", type: "text", title: "UserId" },
                { name: "eventResult", type: "text", title: "EventResult" },
                { name: "eventCategory", type: "text", title: "EventCategory" },
                { name: "time", type: "text", title: "Time" }
            ]

        });
    },

    PageLoad: function ()
    {
        Logs.Grid = $('#logsTable');
        Logs.InitializeGrid();
    },

    Clear: function () {
        Services.Delete('/logs', Logs.ClearLogsCallbackSuccess, null);
    },

    ClearLogsCallbackSuccess: function (data) {
        location.reload();
    }
}

