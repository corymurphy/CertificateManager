var GeneralConfig = {

    SecurityAuditingRadio: null,

    OperationsLoggingRadio: null,

    ClearAuditConfigCurrentState: function () {
        $('input:radio[name="securityAuditingState"]').removeAttr('checked');
        $('input:radio[name="operationsLoggingState"]').removeAttr('checked');
    },

    GetAuditConfigCurrentState: function ()
    {
        return {
            securityAuditingState: $('input:radio[name="securityAuditingState"]:checked').val(),
            operationsLoggingState: $('input:radio[name="operationsLoggingState"]:checked').val()
        }
    },

    GetSettingsCurrentState: function ()
    {
        return {
            cachePeriod: $('#cachePeriod').val()
        };
    },

    GetSettings: function ()
    {
        GeneralConfig.ClearAuditConfigCurrentState();

        Services.GetSettings(GeneralConfig.GetSettingsSuccessCallback, GeneralConfig.ErrorCallback)
    },

    SaveSettings: function ()
    {
        UiGlobal.ResetAlertState();
        var data = GeneralConfig.GetSettingsCurrentState();
        Services.SaveSettings(data, GeneralConfig.SaveSettingsSuccessCallback, GeneralConfig.ErrorCallback);
    },

    SaveAuditConfig: function ()
    {
        UiGlobal.ResetAlertState();
        var data = GeneralConfig.GetAuditConfigCurrentState();
        Services.SetAuditConfig(data, GeneralConfig.SaveAuditConfigSuccessCallback, GeneralConfig.ErrorCallback);
    },

    SaveSettingsSuccessCallback: function () {
        UiGlobal.ShowSuccess("Settings were saved successfully");
    },

    ErrorCallback: function (x) {
        UiGlobal.ShowError(x.message);
    },

    GetSettingsSuccessCallback: function (data) {
        $("input[name=operationsLoggingState][value=" + data.operationsLoggingState + "]").attr('checked', 'checked');
        $("input[name=securityAuditingState][value=" + data.securityAuditingState + "]").attr('checked', 'checked');
        $('#cachePeriod').val(data.cachePeriod);
    },

    SaveAuditConfigSuccessCallback: function ()
    {
        UiGlobal.ShowSuccess("Auditing configuration was saved successfully");
    },

    PageLoad: function ()
    {
        //GeneralConfig.SecurityAuditingRadio = $('input[name=securityAuditingRadio]:checked');
        //GeneralConfig.OperationsLoggingRadio = $('input[name=operationsLoggingRadio]:checked');

        GeneralConfig.GetSettings();
        UiGlobal.ShowCurrentTab();

    }
}

