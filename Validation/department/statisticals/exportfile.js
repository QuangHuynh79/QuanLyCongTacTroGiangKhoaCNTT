$(document).ready(function () {
    $('body').on('click', '[id="exportfile"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang Export...');

        var type = btn.attr('name');
        var hocky = $('body').find('[id="hocky"] :selected').val();
        var nganh = $('body').find('[id="nganh"] :selected').val();

        var tenhocky = $('body').find('[id="hocky"] :selected').attr('name');
        var tennganh = $('body').find('[id="nganh"] :selected').attr('name');

        var formData = new FormData();
        formData.append('type', type);
        formData.append('hocky', hocky);
        formData.append('nganh', nganh);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "Statisticals/ExportFiles",
            dataType: 'html',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false
        }).done(function (ketqua) {
            $('body').find('[id="table-export-data"]').html(ketqua);
            var datas = new Blob([document.getElementById('table-export-data').innerHTML], {
                type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset:utf-8"
            });
            if (type == "ta02") {
                saveAs(datas, "TA02 - HK" + tenhocky + " - " + tennganh + ".xls");
            }
            else {
                saveAs(datas, "TA03 - HK" + tenhocky + " - " + tennganh + ".xls");
            }
            $('body').find('[id="table-export-data"]').html('');

            btn.html('Export');
        });
    });
});