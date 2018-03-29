var ScriptDetails = {

    Uri: '/script',

    Id: null,

    Name: null,

    Code: null,

    PageLoad: function () {
        ScriptDetails.Id = $('#id');
        ScriptDetails.Name = $('#name');
        ScriptDetails.Code = $('#code');
        ScriptDetails.Get(ScriptDetails.Id.val());
    },

    Get: function (id) {
        var uri = ScriptDetails.Uri + '/' + ScriptDetails.Id.val();
        Services.Get(uri, ScriptDetails.GetSuccessCallback, ScriptDetails.ErrorCallback)
    },

    GetSuccessCallback: function (data) {
        ScriptDetails.Name.val(data.name);
        ScriptDetails.Code.val(data.code);
    },

    GetSaveData: function () {
        data = {
            id: ScriptDetails.Id.val(),
            name: ScriptDetails.Name.val(),
            code: ScriptDetails.Code.val()
        }

        return data;
    },

    SaveSuccessCallback: function () {
        UiGlobal.ResetAlertState();
        UiGlobal.ShowSuccess();
    },

    ErrorCallback: function (msg) {
        UiGlobal.ResetAlertState();
        UiGlobal.ShowError('Error while parsing the script');
    },

    Save: function () {
        UiGlobal.ResetAlertState();
        Services.Put(ScriptDetails.Uri, ScriptDetails.GetSaveData(), ScriptDetails.SaveSuccessCallback, ScriptDetails.ErrorCallback);
    }
}