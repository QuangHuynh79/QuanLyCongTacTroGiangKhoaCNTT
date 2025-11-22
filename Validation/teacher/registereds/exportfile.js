$(document).ready(function () {
    $('body').on('click', '[id="btnExport"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);

        var hocky = $('body').find('[id="hocky"] :selected').val();
        var nganh = $('body').find('[id="nganh"] :selected').val();
        var trangthai = $('body').find('[id="trangthaipc"] :selected').val();

        var formData = new FormData();
        formData.append('hocky', hocky);
        formData.append('nganh', nganh);
        formData.append('trangthai', trangthai);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TeachingAssistant/ExportRegistereds",
            dataType: 'html',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false
        }).done(function (ketqua) {
            $('body').find('[id="table-export-data"]').html(ketqua);

            var namehocky = $('body').find('[id="hocky"] :selected').attr('name');
            var namenganh = $('body').find('[id="nganh"] :selected').attr('name');

            var datas = new Blob([document.getElementById('table-export-data').innerHTML], {
                type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset:utf-8"
            });
            saveAs(datas, "Danh Sach Sinh Vien Da Dang Ky Tro Giang - HK" + namehocky + " - " + namenganh + ".xls");
            $('body').find('[id="table-export-data"]').html('');
            btn.html('<i class="bi bi-file-earmark-arrow-down me-1"></i> Xuất');
        });
    });
});