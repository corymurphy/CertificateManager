var Login = {

    PageLoad: function ()
    {
        Login.InitializeAlternativeLoginOptions();
        Login.ConfigureAuthBypass();
        Login.InitializeDomainSelect();
    },

    ConfigureAuthBypass: function ()
    {

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
        CmOptions.ExternalIdentitySources.forEach(function (item) {
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
    }

}