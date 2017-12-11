var Login = {

    PageLoad: function ()
    {
        Login.ShowErrorIfUnsuccessful();
        Login.InitializeAlternativeLoginOptions();
        Login.ConfigureAuthBypass();
        Login.InitializeDomainSelect();   
    },

    ShowErrorIfUnsuccessful: function ()
    {
        if (Login.IsAuthFailure())
        {
            UiGlobal.ShowError();
        }
    },

    ConfigureAuthBypass: function ()
    {
        //controlled by form post
    },

    InitializeAlternativeLoginOptions: function ()
    {
        $('#altLoginMethodCheckbox').change(function () {
            if ($(this).is(":checked")) {
                $('.alt-login-btn').show();
            }
            else {
                $('.alt-login-btn').hide();
            }
        });
    },

    InitializeDomainSelect: function ()
    {
        CmOptions.ActiveDirectoryMetadatas.forEach(function (item) {
            if (item.enabled) {
                var element = $('#domain');

                element.append($('<option>', {
                    value: item.id,
                    text: item.name
                }));
            }
        });

        if (CmOptions.LocalAuthenticationEnabled)
        {
            var element = $('#domain');

            element.append($('<option>', {
                value: CmOptions.LocalIdentityProviderId,
                text: 'Local'
            }));
        }
    },

    IsAuthFailure: function ()
    {
        if (document.URL.indexOf("authentication_failure") > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}