$(document).ready(function () {
    $('body').on('click', '[id="btnExportTkb"]', function () {
        var hocky = $('body').find('[id="hocky"] :selected').val();
        var namehocky = $('body').find('[id="hocky"] :selected').text();
        var nganh = $('body').find('[id="nganh"] :selected').val();
        var namenganh = $('body').find('[id="nganh"] :selected').text();
        var mon = "";
        var gv = "";

        $('body').find('[id="locmonhoc"] :selected').each(function () {
            mon += $(this).val() + "#";
        });
        $('body').find('[id="locgiangvien"] :selected').each(function () {
            gv += $(this).val() + "#";
        });

        var formData = new FormData();
        formData.append('hocky', hocky);
        formData.append('nganh', nganh);
        formData.append('mon', mon.substring(0, mon.length - 1));
        formData.append('gv', gv.substring(0, gv.length - 1));

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "admin/dangnhap/logout"; } },
            url: $('#requestPath').val() + "TimeTable/ExportTimeTable",
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
            saveAs(datas, "Thoi Khoa Bieu - " + namehocky + " - " + namenganh + ".xls");
            $('body').find('[id="table-export-data"]').html('');
        });
    });
});