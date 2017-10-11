function WriteError(msg) {
    $('#error-alert').text(msg);
    $('#error-alert').show();
}

function DisplayCertificateDetails(data) {
    $("#displayName").text(data.displayName);
    $('#hashAlgorithm').text(data.hashAlgorithm);
    $('#thumbprint').text(data.thumbprint);
    $('#certificate-id').text(data.id);
    $('#cipherAlgorithm').text(data.cipherAlgorithm);
    $('#hasPrivateKey').text(data.hasPrivateKey);
    $('#expires').text(data.validTo);
    $('#keySize').text(data.keySize);
    $('#storageFormat').text(data.certificateStorageFormat);
    $('#windowsApi').text(data.windowsApi);
}

function GetPrivateCertificateRequestData() {
    var request = {
        SubjectCommonName: $('#commonName').val(),
        SubjectDepartment: $('#department').val(),
        SubjectOrganization: $('#organization').val(),
        SubjectCity: $('#city').val(),
        SubjectState: $('#state').val(),
        SubjectCountry: $('#country').val(),
        SubjectAlternativeNamesRaw: $('#sancsv').val(),
        CipherAlgorithm: $('#cipherAlgorithm').val(),
        Provider: $('#windowsApi').val(),
        HashAlgorithm: $('#hashAlgorithm').val(),
        KeySize: $('#keySize').val(),
        KeyUsage: $('#keyUsage').val()
    }

    return request;
}

function ValidateNewPrivateCertificate(request) {

    //var spinner = StartSpinner();
    var keysize = request.KeySize;
    var provider = request.Provider;
    var cipheralg = request.CipherAlgorithm;
    var validRsaKeySizes = ["2048", "4096", "8192", "16384"]
    var keyusage = request.KeyUsage;
    //var role = $('#role').val();


    //0 is RSA
    if (cipheralg == "0") {
        if (validRsaKeySizes.indexOf(keysize) != 0) {
            var msg = "Rsa keysize is invalid. Rsa only supports 2048, 4096, 8192, or 16384. Choose 2048 if you are unsure.";
            WriteError(msg)
            return false;
        }
    }

    //1 is EC
    if (cipheralg == "1") {
        if (provider != "1") {
            var msg = "Elliptic Curve only supports the provider Cng (CryptoApi Next Generation)";
            WriteError(msg);
            return false;
        }

        if (keysize != 256) {
            var msg = "Elliptic Curve only supports a keysize of 256bits";
            WriteError(msg);
            return false;
        }
    }


    if ($("#SubjectCommonName").val() != "") {
        var result = Services.ValidateDnsName($("#SubjectCommonName").val())
        if (result["Status"] != "valid") {
            WriteError("Invalid subject common name provided")
            return false;
        }
    }
    else {
        WriteError("You must specify a subject common name");
        return false;
    }

    if ($("#SubjectAlternativeNamesRaw").val() != "") {
        var result = Services.ValidateDnsSan($("#SubjectAlternativeNamesRaw").val())
        if (result["Status"] != "valid") {
            WriteError("Invalid subject alternative name. Either remove the subject alternative name, or specify the san with correct comma separated format. ")
            return false;
        }
    }


    if (keyusage == null) {
        WriteError("Key Usage must be specified. If you're not an Admin, you won't be able to select anything except 'Server'")
        return false;
    }

    if (!keyusage.constructor == Array) {
        WriteError("Key Usage must be specified. If you're not an Admin, you won't be able to select anything except 'Server'")
        return false;
    }

    if (keyusage.length < 1 || keyusage.length > 3) {
        WriteError("Key Usage must be specified. If you're not an Admin, you won't be able to select anything except 'Server'")
        return false;
    }

    //if (role != "Admin" && keyusage != 1) {
    //    $(".modal-body").text("Key Usage must be specified. If you're not an Admin, you won't be able to select anything except 'Server'");
    //    $('#msgModal').modal('show');
    //    return false;
    //}

    return true;
    //$('#formSubmitButtonHidden').click();


}



var PrivateCertificateAuthority = {

    CreateCertificateSuccessCallback: function (data) {

        window.sessionStorage.setItem("certificateId", data.id);
        $('#create-private-certificate-btn').prop('disabled', false);
        window.location.replace("/views/certificate/" + data.id);
    },

    CreateCertificateErrorCallback: function (x, t, m) {

        WriteError(x.responseJSON.message);

        $('#create-private-certificate-btn').prop('disabled', false);

    },

    GetCertificateErrorCallback: function (x, t, m) {

        //WriteError(x.responseJSON.message);

        //$('#create-private-certificate-btn').prop('disabled', false);

    },

    GetCertificateSuccessCallback: function (data) {
        DisplayCertificateDetails(data);
    },

    GetCertificateDetails: function () {
        //var id = window.location.pathname.replace('/views/certificate/', '');
        var id = window.sessionStorage.getItem("certificateId");
        Services.GetCertificateDetails(id, this.GetCertificateSuccessCallback, this.GetCertificateErrorCallback);
    },

    RegisterCreateCertificateButtonEvent: function () {
        $('#create-private-certificate-btn').click(function () {

            $('#error-alert').hide();
            $('#success-alert').hide();
            $('#create-private-certificate-btn').prop('disabled', true);

            var request = GetPrivateCertificateRequestData();

            //if (!ValidateNewPrivateCertificate(request))
            //{
            //    $('#create-private-certificate-btn').prop('disabled', false);
            //    return false;
            //}

            Services.CreateCertificate(request, PrivateCertificateAuthority.CreateCertificateSuccessCallback, PrivateCertificateAuthority.CreateCertificateErrorCallback);

        });

    },

    InitializeUi: function () {
        CmOptions.hashAlgorithmOptions.forEach(function (item) {

            var element = $('#hashAlgorithm');

            element.append($('<option>', {
                value: item.Name,
                text: item.Name
            }));

            if (item.Primary) {
                element.val(item.Name);
            }
        });

        CmOptions.cipherOptions.forEach(function (item) {
            $('#cipherAlgorithm').append($('<option>', {
                value: item.Name,
                text: item.Display
            }));
        });

        CmOptions.keyUsageOptions.forEach(function (item) {
            if (item.Primitive) {
                $('#keyUsage').append($('<option>', {
                    value: item.Name,
                    text: item.Display
                }));
            }
        });

        $('#keyUsage').select2({ width: '100%' });

        CmOptions.windowsApiOptions.forEach(function (item) {
            $('#windowsApi').append($('<option>', {
                value: item.Name,
                text: item.Display
            }));
        });
    },

    PageLoad: function () {
        PrivateCertificateAuthority.InitializeUi();
        PrivateCertificateAuthority.RegisterCreateCertificateButtonEvent();
    }
};