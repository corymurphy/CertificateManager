ViewCertificate = {

    PageLoad: function ()
    {
        document.getElementById("defaultOpen").click();
        Services.GetCertificateDetails(ViewCertificate.GetCertificateId(), ViewCertificate.GetCertificateSuccessCallback, ViewCertificate.GetCertificateErrorCallback);
        ViewCertificate.InitializeDownloadUx();
    },

    GetCertificateId: function ()
    {
        return $('#certificate-id-hidden').val();
    },

    CertificateData: null,

    GetCertificateSuccessCallback: function (data)
    {
        CertificateData = data;

        $('#displayName').text(data.displayName);
        $('#thumbprint').text(data.thumbprint);
        $('#hashAlgorithm').text(data.hashAlgorithm);
        $('#cipherAlgorithm').text(data.cipherAlgorithm);
        $('#hasPrivateKey').text(data.hasPrivateKey);
        $('#expires').text(UiGlobal.GetDateString(data.expires));
        $('#windowsApi').text(UiGlobal.GetWindowsApiDisplay(data.windowsApi));
        $('#keySize').text(data.keySize);
        $('#storageFormat').text(data.certificateStorageFormat);

    },

    GetCertificateErrorCallback: function ()
    {
        UiGlobal.ShowError();
    },

    InitializeDownloadUx: function ()
    {
        $('#showCertificateDownloadOptionsButton').click(function () {

            $('#certificateDownloadOptionsModal').modal('show');

        });


        $('#cer-radio').change(function () {
            $("#download-certificate-button").prop('disabled', false);
            $("#download-cer-option-der").prop('disabled', false);
            $("#download-cer-option-b64").prop('disabled', false);
            $("#download-cer-option-b64").prop('checked', true);
            $("#cer-background").css("background-color", "#b0c4de");
            $("#pfx-background").css("background-color", "#f5f5f5");
        });

        $('#pfx-radio').change(function () {
            $("#download-certificate-button").prop('disabled', false);
            $("#download-pfx-includechain").prop('disabled', false);
            $("#download-cer-option-b64").prop('disabled', true);
            $("#download-cer-option-der").prop('disabled', true);
            $("#pfx-background").css("background-color", "#b0c4de");
            $("#cer-background").css("background-color", "#f5f5f5");
        });


        $('#download-certificate-button').click(function (e) {
            var type;
            var encoding;
            if ($("#cer-radio").prop("checked")) {
                if ($("#download-cer-option-b64").prop('checked')) {
                    type = "certb64";
                }
                else {
                    type = "certbinary";
                }
                type = "certbinary";
            }
            else if ($("#pfx-radio").prop("checked")) {
                type = "pfx";
            }
            else {
                type = "";
            }

            if ($('#include-chain-checkbox').prop('checked')) {
                type = type + "chain"
            }

            var downloadUri = "/certificate/" + $('#certificate-id').text();

            switch (type) {
                case "certbinarychain":
                    downloadUri = downloadUri + "/download/cer/binary/includechain";
                    break;
                case "certbinary":
                    downloadUri = downloadUri + "/download/cer/binary/nochain";
                    break;
                case "certb64chain":
                    downloadUri = downloadUri + "/download/cer/base64/includechain";
                    break;
                case "certb64":
                    downloadUri = downloadUri + "/download/cer/base64/nochain";
                    break;
                case "pfxchain":
                    downloadUri = downloadUri + "/download/pfx/includechain";
                    break;
                case "pfx":
                    downloadUri = downloadUri + "/download/pfx/nochain";
                    break;
                default:
                    downloadUri = downloadUri + "/download/cer/binary/nochain";
            }

            e.preventDefault();
            window.location.href = downloadUri;

        });
    }
}