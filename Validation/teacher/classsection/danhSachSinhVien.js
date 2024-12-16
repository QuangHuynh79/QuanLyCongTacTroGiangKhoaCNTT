$(document).ready(function () {

    $('body').on('change', '[id="tuanhoc"]', function () {
        var idLichHoc = $('body').find('[id="tuanhoc"] :selected').val();
        var idLhp = $('body').find('[id="idLHPDiemDanh"]').val();

        var formData = new FormData();
        formData.append('idLhp', idLhp);
        formData.append('idLichHoc', idLichHoc);

        $('body').find('[id="loadcontentdiemdanhsv"]').html('<div class="modal-body overflow-x-hidden" id="loadcontentdiemdanhsv"><h3 class="text-center mb-3 mt-3"><span class="spinner-border spinner-border-sm me-2" style="width: 18px; height: 18px" role="status" aria-hidden="true"></span>Đang tải...</h3></div>');

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "ClassManagement/ClassList",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                Toast.fire({
                    icon: "error",
                    title: ketqua
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                $('body').find('[id="loadcontentdiemdanhsv"]').replaceWith(ketqua);
                $('body').find('[id="diemDanhTable"]').DataTable({
                    pageLength: 100,
                    columnDefs: [{
                        'searchable': false,
                        'targets': [0, 3, 4]
                    },
                    ]
                });
            }
        });
    });

    $('body').on('click', '[id^="viewDiemDanh-"]', function () {
        var id = $(this).attr('name');
        var tieude = $(this).attr("tieudeForm");
        $('body').find('[id="diemdanhsvTitle"]').text(tieude);

        var formData = new FormData();
        formData.append('idLhp', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "ClassManagement/ClassList",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                Toast.fire({
                    icon: "error",
                    title: ketqua
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                $('body').find('[id="loadcontentdiemdanhsv"]').replaceWith(ketqua);
                $('body').find('[id="diemDanhTable"]').DataTable({
                    pageLength: 100,
                    columnDefs: [{
                        'searchable': false,
                        'targets': [0, 3, 4]
                    },
                    ]
                });
                $('body').find('[id="diemdanhsv"]').modal('toggle');
            }
        });
    });
});