var GeneralConfig = {




    SecurityAuditingRadio: null,

    OperationsLoggingRadio: null,

    ClearAuditConfigCurrentState: function () {
        $('input:radio[name="SecurityAuditingState"]').removeAttr('checked');
        $('input:radio[name="OperationsLoggingState"]').removeAttr('checked');
    },

    GetAuditConfigCurrentState: function ()
    {
        return {
            securityAuditingRadio: $('input:radio[name=SecurityAuditingState]:checked').val(),
            operationsLoggingRadio: $('input:radio[name=OperationsLoggingState]:checked').val()
        }
    },

    GetAuditConfig: function ()
    {
        GeneralConfig.ClearAuditConfigCurrentState();

        Services.GetAuditConfig(GeneralConfig.GetAuditConfigSuccessCallback, GeneralConfig.GetAuditConfigErrorCallback)
        //var value = 5;
        //$("input[name=mygroup][value=" + value + "]").attr('checked', 'checked');
    },

    SaveAuditConfig: function ()
    {
        UiGlobal.ResetAlertState();
        var data = GeneralConfig.GetAuditConfigCurrentState();
        Services.SetAuditConfig(data, GeneralConfig.SaveAuditConfigSuccessCallback, GeneralConfig.SaveAuditConfigErrorCallback);
    },

    GetAuditConfigSuccessCallback: function (data) {
        $("input[name=operationsLoggingState][value=" + data.operationsLoggingState + "]").attr('checked', 'checked');
        $("input[name=securityAuditingState][value=" + data.securityAuditingState + "]").attr('checked', 'checked');
    },

    GetAuditConfigErrorCallback: function (x) {
        UiGlobal.ShowError(x.message);
    },

    SaveAuditConfigSuccessCallback: function ()
    {
        UiGlobal.ShowSuccess("Auditing configuration was saved successfully");
    },

    SaveAuditConfigErrorCallback: function (x) {
        UiGlobal.ShowError(x.message);
    },

    PageLoad: function ()
    {
        //GeneralConfig.SecurityAuditingRadio = $('input[name=securityAuditingRadio]:checked');
        //GeneralConfig.OperationsLoggingRadio = $('input[name=operationsLoggingRadio]:checked');

        GeneralConfig.GetAuditConfig();
        UiGlobal.ShowCurrentTab();

    }
}

