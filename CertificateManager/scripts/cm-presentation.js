function openTab(evt, tabName) {
    // Declare all variables
    var i, tabcontent, tablinks;

    location.hash = tabName;
    // Get all elements with class="tabcontent" and hide them
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }

    // Get all elements with class="tablinks" and remove the class "active"
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }

    // Show the current tab, and add an "active" class to the button that opened the tab
    document.getElementById(tabName).style.display = "block";
    $('.' + tabName).addClass("active");
};


var UIDefaults = {
    GetEnumMap: function () {
        if (localStorage.getItem("uiEnumMap") === null) {
            Services.GetEnumMapping();

            while (localStorage.getItem("uiEnumMap") === null)
            {

            }
            return JSON.parse(localStorage.getItem("uiEnumMap"));
        }
        else {
            return JSON.parse(localStorage.getItem("uiEnumMap"));
        }
    }
};


var UiGlobal = {

    GetWindowsApiDisplay: function (name)
    {
        var displayName = "unknown";

        CmOptions.windowsApiOptions.forEach(function (item) {

            if (item.Name == name)
            {
                displayName = item.Display;
            }
        });

        return displayName;
    },

    GetShortDateTime: function (value, item) {
        var date = new Date(value)
        return date.toDateString();
    },
    RefreshGrid: function (grid)
    {
        grid.jsGrid("render");
    },
    ResetAlertState: function ()
    {
        UiGlobal.HideError();
        UiGlobal.HideSuccess();
    },
    ShowSuccess: function (msg)
    {
        $('#success-alert').text(msg)
        $('#success-alert').show();
    },
    HideSuccess: function (msg)
    {
        $('#success-alert').hide();
    },
    ShowError: function (msg)
    {
        $('#error-alert').text(msg)
        $('#error-alert').show();
    },
    HideError: function ()
    {
        $('#error-alert').hide();
    },
    ShowCurrentTab: function ()
    {
        if (location.hash === "" || location.hash === null) {
            $('#defaultOpen').click();
        }
        else
        {
            var currentTab = location.hash.replace("#", "");
            $("." + currentTab).click();
        }
    },

    ShowModal: function (id) {
        $("#" + id).modal("show");
    },
    GetSelectedOptions: function (obj)
    {
        var selectedArray = [];

        var selected = obj.find(":selected");

        for (i = 0; i < selected.length; i++) {
            selectedArray.push(selected[i].value);
        }

        return selectedArray;
    },

    GetDateString(arg) {
        return (new Date(arg)).toDateString();
    },


    GetFormRowDiv: function ()
    {
        return $('<div>').addClass('form-group row');
    },

    GetFormColLabel: function (displayText)
    {
        return $('<div>').addClass('col-sm-6').text(displayText);
    },

    GetFormCheckbox: function (inputId, isChecked)
    {
        var input = $('<input>').addClass('form-check-input scope').attr('type', 'checkbox').attr('id', inputId).attr("checked", isChecked);
        var label = $('<label>').addClass('form-check-label');
        var inputDiv = $('<div>').addClass('form-check');
        var inputCol = $('<div>').addClass('col-sm-4');

        return inputCol.append(inputDiv.append(label.append(input)));
    },

    GetButton: function (onclick)
    {
        return $('<button>')
            .attr('type', 'button')
            .addClass('btn btn-primary')
            .attr('onclick', onclick)
            .text('Save');
    }
}