$(document).ready(function () {
    $('body').on('click', '[id="btnExportDssv"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang Export...');


        var idLhp = $('body').find('[id="idLHPDiemDanh"]').val();
        var maLhp = $('body').find('[id="idLHPDiemDanh"]').attr('name');
        var formData = new FormData();
        formData.append('idLhp', idLhp);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "ClassManagement/ExportData",
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
            saveAs(datas, "Danh Sach Sinh Vien - " + maLhp + ".xls");
            $('body').find('[id="table-export-data"]').html('');

            btn.html('Export');
        });
    });
});