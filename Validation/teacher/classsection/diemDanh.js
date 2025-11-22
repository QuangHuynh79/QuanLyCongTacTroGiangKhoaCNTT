$(document).ready(function () {

    $('body').on('click', '[id="btnSubmitDiemDanh"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');

        var idLichHoc = $('body').find('[id="tuanhoc"] :selected').val();

        var lstIdSv = "";
        var lstDuLop = "";
        var lstGhiChu = "";
        var check = true;
        $('body').find('[id^="dulop-"]').each(function () {
            var select = $(this);
            var id = select.attr('name');
            var dulop = $('body').find('[id="dulop-' + id + '"] :selected').val();
            var ghichu = $('body').find('[id="ghichu-' + id + '"]').val().trim();

            if (ghichu.indexOf("#") != -1) {
                ghichu = ghichu.replace(/#/g, '');
            }

            lstIdSv += id + "#";
            lstDuLop += dulop + "#";
            lstGhiChu += ghichu + "#";
        });

        if (lstIdSv.length > 0) {
            lstIdSv = lstIdSv.substring(0, lstIdSv.length - 1);
        }
        else {
            check = false;
            Toast.fire({
                icon: "error",
                title: 'Đã có lỗi xảy ra vui lòng thử lại sau ít phút!'
            }).then(() => {
                window.location.reload();
            });
        }

        if (lstDuLop.length > 0) {
            lstDuLop = lstDuLop.substring(0, lstDuLop.length - 1);
        }
        else {
            check = false;
            Toast.fire({
                icon: "error",
                title: 'Đã có lỗi xảy ra vui lòng thử lại sau ít phút!'
            }).then(() => {
                window.location.reload();
            });
        }

        if (lstGhiChu.length > 0) {
            lstGhiChu = lstGhiChu.substring(0, lstGhiChu.length - 1);
        }

        if (check == true) {

            var formData = new FormData();
            formData.append('idLichHoc', idLichHoc);
            formData.append('idsv', lstIdSv);
            formData.append('trangthai', lstDuLop);
            formData.append('ghichu', lstGhiChu);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
                url: $('#requestPath').val() + "ClassManagement/RollCall",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                    btn.html('Lưu thông tin');

                    Toast.fire({
                        icon: "error",
                        title: ketqua
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else {
                    btn.html('Lưu thông tin');
                    ReloadContent();

                    Toast.fire({
                        icon: "success",
                        title: 'Đã lưu thông tin điểm danh lớp học'
                    });
                }
            });
        }
    });

    function ReloadContent() {
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
    }
});