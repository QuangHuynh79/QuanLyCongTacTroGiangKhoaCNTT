$(document).ready(function () {
    function Base64ToBytes(base64) {
        var s = window.atob(base64);
        var bytes = new Uint8Array(s.length);
        for (var i = 0; i < s.length; i++) {
            bytes[i] = s.charCodeAt(i);
        }
        return bytes;
    };

    $('body').on('click', '[id="downloadformimport"]', function () {
        var fileName = "CNTT UIS-ThoiKhoaBieu_TieuChuan_Mau.xlsx";

        $.ajax({
            type: "POST",
            url: $('#requestPath').val() + "Timetable/DownloadFile",
            data: '{fileName: "' + fileName + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "text",
            success: function (r) {
                var bytes = Base64ToBytes(r);

                var blob = new Blob([bytes], { type: "application/octetstream" });

                var isIE = false || !!document.documentMode;
                if (isIE) {
                    window.navigator.msSaveBlob(blob, fileName);
                } else {
                    var url = window.URL || window.webkitURL;
                    link = url.createObjectURL(blob);
                    var a = $("<a />");
                    a.attr("download", fileName);
                    a.attr("href", link);
                    $("body").append(a);
                    a[0].click();
                    $("body").remove(a);
                }
            }
        });
    });
});