$(document).ready(function () {

    $('body').on('click', '[id="btnDeXuatSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnDeXuatClose"]').prop('disabled', true);

        var idLHP = $('body').find('[id="idLHPDX"]').val();
        var lydo = $('body').find('[id="lydo"]').val().trim();

        var mota = "";
        var khoiluong = "";
        var thoigian = "";
        var noilamviec = "";
        var ketqua = "";

        var check = true;

        $('body').find('[id^="valid-lydo"]').text('');
        $('body').find('[id^="valid-mota-"]').each(function () { $(this).text(''); });
        $('body').find('[id^="valid-khoiluong-"]').each(function () { $(this).text(''); });
        $('body').find('[id^="valid-thoigian-"]').each(function () { $(this).text(''); });
        $('body').find('[id^="valid-noilamviec-"]').each(function () { $(this).text(''); });
        $('body').find('[id^="valid-ketqua-"]').each(function () { $(this).text(''); });

        if (lydo.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnDeXuatClose"]').prop('disabled', false);

            $('body').find('[id^="valid-lydo"]').text('Vui lòng nhập lý do cần TA hỗ trợ.');
            $('body').find('[id="lydo"]').focus();
        }

        $('body').find('[id^="mota-"]').each(function () {
            var inp = $(this);
            var idInp = inp.attr('name');
            var val = inp.val().trim();
            if (val.length < 1) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnDeXuatClose"]').prop('disabled', false);

                $('body').find('[id^="valid-mota-' + idInp + '"]').text('Vui lòng nhập mô tả.');
            }
            else {
                mota += val + "~";
            }
        });
        $('body').find('[id^="khoiluong-"]').each(function () {
            var inp = $(this);
            var idInp = inp.attr('name');
            var val = inp.val().trim().replace(",", ".");
            if (val.length < 1) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnDeXuatClose"]').prop('disabled', false);

                $('body').find('[id^="valid-khoiluong-' + idInp + '"]').text('Vui lòng nhập.');
            }
            else {
                khoiluong += val + "~";
            }
        });
        $('body').find('[id^="thoigian-"]').each(function () {
            var inp = $(this);
            var idInp = inp.attr('name');
            var val = inp.val().trim();
            if (val.length < 1) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnDeXuatClose"]').prop('disabled', false);

                $('body').find('[id^="valid-thoigian-' + idInp + '"]').text('Vui lòng nhập hạn hoàn thành.');
            }
            else {
                thoigian += val + "~";
            }
        });
        $('body').find('[id^="noilamviec-"]').each(function () {
            var inp = $(this);
            var idInp = inp.attr('name');
            var val = inp.val().trim();
            if (val.length < 1) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnDeXuatClose"]').prop('disabled', false);

                $('body').find('[id^="valid-noilamviec-' + idInp + '"]').text('Vui lòng nhập nơi làm việc.');
            }
            else {
                noilamviec += val + "~";
            }
        });
        $('body').find('[id^="ketqua-"]').each(function () {
            var inp = $(this);
            var idInp = inp.attr('name');
            var val = inp.val().trim();
            if (val.length < 1) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnDeXuatClose"]').prop('disabled', false);

                $('body').find('[id^="valid-ketqua-' + idInp + '"]').text('Vui lòng nhập kết quả mong đợi.');
            }
            else {
                ketqua += val + "~";
            }
        });

        if (check == true) {
            var formData = new FormData();

            formData.append('idLHP', idLHP);
            formData.append('lydo', lydo);
            formData.append('trangthai', $('body').find('[id="camket"]').prop('checked'));
            formData.append('mota', mota.substring(0, mota.length - 1));
            formData.append('khoiluong', khoiluong.substring(0, khoiluong.length - 1));
            formData.append('thoigian', thoigian.substring(0, thoigian.length - 1));
            formData.append('noilamviec', noilamviec.substring(0, noilamviec.length - 1));
            formData.append('ketqua', ketqua.substring(0, ketqua.length - 1));

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
                url: $('#requestPath').val() + "ClassSection/AddSuggested",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                $('body').find('[id="dexuattrogiang"]').modal('toggle');

                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnDeXuatClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: "Đã đề xuất lớp học phần."
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else if (ketqua.indexOf("Sai định dạng") !== -1) {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnDeXuatClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "error",
                        title: "Ô khối lượng công việc dòng số " + ketqua.split('-')[1] + " nhập sai định dạng."
                    });
                }
                else if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnDeXuatClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "error",
                        title: ketqua
                    }).then(() => {
                        window.location.reload();
                    });
                }
            });
        }
    });

    $('body').on('click', '[id="btnviewtasklistmodalClose"]', function () {
        confirmTaskCloseForm();
    });
    $('body').on('click', '[id="btnviewtasklistmodalClose2"]', function () {
        confirmTaskCloseForm();
    });

    $('body').on('click', '[id="btnDeXuatClose"]', function () {
        confirmCloseForm();
    });

    $('body').on('click', '[id="btnDeXuatClose2"]', function () {
        confirmCloseForm();
    });
    function confirmCloseForm() {

        var check = true;
        var lydo = $('body').find('[id="lydo"]').val().trim();

        if (lydo.length > 0) {
            check = false;
        }

        $('body').find('[id^="mota-"]').each(function () {
            var inp = $(this);
            var val = inp.val().trim();
            if (val.length > 0) {
                check = false;
            }
        });
        $('body').find('[id^="khoiluong-"]').each(function () {
            var inp = $(this);
            var val = inp.val().trim();
            if (val.length > 0) {
                check = false;
            }
        });
        $('body').find('[id^="thoigian-"]').each(function () {
            var inp = $(this);
            var val = inp.val().trim();
            if (val.length > 0) {
                check = false;
            }
        });
        $('body').find('[id^="noilamviec-"]').each(function () {
            var inp = $(this);
            var val = inp.val().trim();
            if (val.length > 0) {
                check = false;
            }
        });
        $('body').find('[id^="ketqua-"]').each(function () {
            var inp = $(this);
            var val = inp.val().trim();
            if (val.length > 0) {
                check = false;
            }
        });

        if (check == false) {
            Swal.fire({
                text: 'Dữ liệu chưa được lưu. Xác nhận đóng form?',
                icon: "question",
                showCancelButton: true,
                cancelButtonColor: "#d33",
                confirmButtonText: "Đóng ngay!",
                cancelButtonText: "Không đóng"
            }).then((result) => {
                if (result.isConfirmed) {
                    $('#dexuattrogiang').modal('toggle');
                }
            });
        }
        else {
            $('#dexuattrogiang').modal('toggle');
        }
    }

    function confirmTaskCloseForm() {
        var checkViewOnlySeen = $('body').find('[id="viewchixem"]').val();

        if (checkViewOnlySeen == "true") {
            $('#viewtasklistmodal').modal('toggle');
        }
        else {

            var check = true;

            $('body').find('[id^="edit-mota-"]').each(function () {
                var inp = $(this);
                var val = inp.val().trim();
                if (val.length > 0) {
                    check = false;
                }
            });
            $('body').find('[id^="edit-khoiluong-"]').each(function () {
                var inp = $(this);
                var val = inp.val().trim();
                if (val.length > 0) {
                    check = false;
                }
            });
            $('body').find('[id^="edit-thoigian-"]').each(function () {
                var inp = $(this);
                var val = inp.val().trim();
                if (val.length > 0) {
                    check = false;
                }
            });
            $('body').find('[id^="edit-noilamviec-"]').each(function () {
                var inp = $(this);
                var val = inp.val().trim();
                if (val.length > 0) {
                    check = false;
                }
            });
            $('body').find('[id^="edit-ketqua-"]').each(function () {
                var inp = $(this);
                var val = inp.val().trim();
                if (val.length > 0) {
                    check = false;
                }
            });

            if (check == false) {
                Swal.fire({
                    text: 'Dữ liệu chưa được lưu. Xác nhận đóng form?',
                    icon: "question",
                    showCancelButton: true,
                    cancelButtonColor: "#d33",
                    confirmButtonText: "Đóng ngay!",
                    cancelButtonText: "Không đóng"
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#viewtasklistmodal').modal('toggle');
                    }
                    
                });
            }
            else {
                $('#viewtasklistmodal').modal('toggle');
            }
        }
    }
});