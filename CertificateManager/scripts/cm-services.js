
var certSearchResult = null;

var Services = {
 
    CreateCertificate: function (request, successCallback, errorCallback) {
        $.ajax({
            url: "/ca/private/certificate/request/includeprivatekey",
            type: 'post',
            data: {
                SubjectCommonName: request.SubjectCommonName,
                SubjectDepartment: request.SubjectDepartment,
                SubjectOrganization: request.SubjectOrganization,
                SubjectCity: request.SubjectCity,
                SubjectState: request.SubjectState,
                SubjectCountry: request.SubjectCountry,
                SubjectAlternativeNamesRaw: request.SubjectAlternativeNamesRaw,
                CipherAlgorithm: request.CipherAlgorithm,
                Provider: request.Provider,
                HashAlgorithm: request.HashAlgorithm,
                KeySize: request.KeySize,
                KeyUsage: request.KeyUsage
            },
            cache: false,
            async: true,
            dataType: "json",
            success: function (data) {
                successCallback(data);
            },
            error: function (x, t, m) {
                errorCallback(x, t, m);
            }
        });
    },

    GetCertificateDetails: function (id, successCallback, errorCallback) {
        $.ajax({
            url: "/certificate/" + id,
            type: 'get',
            cache: false,
            async: true,
            dataType: "json",
            success: function (data) {
                successCallback(data);
            },
            error: function (x, t, m) {
                errorCallback(x, t, m);
            }
        });
    },

    SearchCertificates: function (query, successCallback, errorCallback)
    {
        $.ajax({
            url: "/certificates/search",
            type: 'get',
            cache: false,
            async: true,
            dataType: "json",
            success: function (data) {
                certSearchResult = data;
                //successCallback(data);
            },
            error: function (x, t, m) {
                //errorCallback(x, t, m);
            }
        });
    },

    GetAdcsTemplates: function (successCallback, errorCallback) {
        $.ajax({
            url: "/pki-config/templates",
            type: 'get',
            cache: false,
            async: true,
            dataType: "json",
            success: function (data) {
                successCallback(data);
            },
            error: function (x, t, m) {
            }
        });
    },

    GetEnumMapping: async function () {
        const response = await axios.get("/view/enum-mapping");
        localStorage.setItem("uiEnumMap", JSON.stringify(response.data));

    },

    GetSecurityRoleDetails: function (id, successCallback, errorCallback)
    {
        $.ajax({
            url: "/security/role/" + id,
            type: 'get',
            cache: false,
            async: true,
            dataType: "json",
            success: function (data) {
                successCallback(data);
            },
            error: function (x, t, m) {
                errorCallback(x, t, m);
            }
        });
    },

    ImportUsersFromExternalIdentitySource: function (data, successCallback, errorCallback)
    {
        $.ajax({
            url: "/security/authenticable-principal/import",
            type: 'post',
            cache: false,
            async: true,
            dataType: "json",
            data: data,
            success: function (data) {
                successCallback(data);
            },
            error: function (x, t, m) {
                errorCallback(x, t, m);
            }
        });
    },

    GetAppConfig: function (successCallback, errorCallback)
    {
        $.ajax({
            url: "/config",
            type: 'get',
            cache: false,
            async: true,
            dataType: "json",
            success: function (data) {
                successCallback(data);
            },
            error: function (x, t, m) {
                errorCallback(x, t, m);
            }
        });
    },

    SetAppConfig: function (data, successCallback, errorCallback)
    {
        $.ajax({
            url: "/config",
            type: 'put',
            cache: false,
            async: true,
            data: data,
            dataType: "json",
            success: function (data) {
                successCallback();
            },
            error: function (x, t, m) {
                errorCallback(x, t, m);
            }
        });
    },

    ResetUserPassword: function (data, successCallback, errorCallback) {
        $.ajax({
            url: "/security/authenticable-principal/password",
            type: 'put',
            cache: false,
            async: true,
            data: data,
            dataType: "json",
            success: function (response) {
                successCallback("Successfully reset password");
            },
            error: function (x, t, m) {
                errorCallback(x, t, m);
            }
        });
    },

    SetLocalAppConfig: function (data, successCallback, errorCallback) {
        $.ajax({
            url: "/config/local",
            type: 'put',
            cache: false,
            async: true,
            data: data,
            dataType: "json",
            success: function (data) {
                successCallback();
            },
            error: function (x, t, m) {
                errorCallback(x, t, m);
            }
        });
    },
    SetRoleScopes: function (roleId, scopes, successCallback, errorCallback) {

        UiGlobal.ResetAlertState();

        $.ajax({
            url: "/security/role/" + roleId + "/scopes",
            type: 'put',
            cache: false,
            async: true,
            data: JSON.stringify( { scopes: scopes } ),
            dataType: "json",
            contentType: "application/json",
            success: function (data) {
                successCallback();
            },
            error: function (x, t, m) {
                errorCallback(x.responseJSON.message);
                //errorCallback(x, t, m);
            }
        });
    }

}
