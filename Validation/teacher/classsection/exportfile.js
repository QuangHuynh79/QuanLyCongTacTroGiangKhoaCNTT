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

            // Xuất file Excel
            var table = $('body').find('[id="exportTables"]')[0];
            var wb = XLSX.utils.table_to_book(table, { sheet: "Danh sách SV" });
            var ws = wb.Sheets["Danh sách SV"];
            for (var cell in ws) {
                if (ws.hasOwnProperty(cell) && cell[0] !== '!') { // Bỏ qua các metadata của sheet 
                    var cellAddress = XLSX.utils.decode_cell(cell);
                    if (cellAddress.r > 3) {

                        if (!ws[cell]) ws[cell] = {};
                        ws[cell].t = 's'; // Định dạng kiểu chuỗi 
                        ws[cell].z = '@'; // Định dạng chuỗi
                    }
                }
            }

            XLSX.writeFile(wb, "Danh Sach Sinh Vien - " + maLhp + ".xlsx");

            $('body').find('[id="table-export-data"]').html('');
            btn.html('Export');
        });
    });
});