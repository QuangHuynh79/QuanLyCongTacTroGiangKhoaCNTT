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

            // Xuất file Excel
            var table = $('body').find('[id="exportTables"]')[0];
            if (type == "ta02") {
                var wb = XLSX.utils.table_to_book(table, { sheet: "Phụ lục TA-02" });
                XLSX.writeFile(wb, "TA02 - HK" + tenhocky + " - " + tennganh + ".xlsx");
            }
            else {
                var wb = XLSX.utils.book_new();

                var ws1 = XLSX.utils.table_to_sheet(table);
                XLSX.utils.book_append_sheet(wb, ws1, "Phụ lục TA-03");

                var table2 = $('body').find('[id="exportTables2"]')[0];
                var ws2 = XLSX.utils.table_to_sheet(table2);
                XLSX.utils.book_append_sheet(wb, ws2, "Thong tin chuyen khoan");

                XLSX.writeFile(wb, "TA03 - HK" + tenhocky + " - " + tennganh + ".xlsx");
            }
            $('body').find('[id="table-export-data"]').html('');
            btn.html('Export');
        });
    });
});