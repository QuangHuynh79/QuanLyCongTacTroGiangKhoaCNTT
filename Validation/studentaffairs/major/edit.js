$(document).ready(function () {
    $('body').on('click', '[id^="openSua-"]', function () {
        var formData = new FormData();
        formData.append('id', $(this).attr('name'));

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TermAndMajor/OpenEditMajor",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua.indexOf("Chi tiết lỗi") !== -1 || ketqua.indexOf("Ngành không tồn tại") !== -1) {
                Swal.fire({
                    title: "Đã xảy ra lỗi!",
                    text: ketqua,
                    icon: "error"
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                $('body').find('[id="capnhatnganh-partial"]').replaceWith(ketqua);
                $('body').find('[id="capnhatTitle"]').text("Cập nhật ngành " + $('body').find('[id="editmanganh"]').val());
                $('body').find('[id="capnhat"]').modal('toggle');
            }
        });
    });

    $('body').on('click', '[id="btnEditSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnEditClose"]').prop('disabled', true);

        var manganh = $('body').find('[id="editmanganh"]').val().trim();
        var tennganh = $('body').find('[id="edittennganh"]').val().trim();
        var tenviettat = $('body').find('[id="edittenviettat"]').val().trim();
        var ctdt = $('body').find('[id="editctdt"] :selected').val().trim();

        var validmanganh = $('body').find('[id="valid-editmanganh"]');
        var validtennganh = $('body').find('[id="valid-edittennganh"]');
        var validtenviettat = $('body').find('[id="valid-edittenviettat"]');
        var validctdt = $('body').find('[id="valid-editctdt"]');

        validmanganh.text('');
        validtennganh.text('');
        validtenviettat.text('');
        validctdt.text('');

        var check = true;

        var check = true;

        if (manganh.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validmanganh.text("Vui lòng nhập mã ngành.");
            $('body').find('[id="editmanganh"]').focus();
        }

        if (tennganh.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validtennganh.text("Vui lòng nhập tên ngành.");
            $('body').find('[id="edittennganh"]').focus();
        }

        if (tenviettat.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validtenviettat.text("Vui lòng nhập tên viết tắt.");
            $('body').find('[id="edittenviettat"]').focus();
        }

        if (ctdt.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validctdt.text("Vui lòng chọn chương trình đào tạo.");
            $('body').find('[id="editctdt"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('id', $('body').find('[id="idn"]').val());
            formData.append('manganh', manganh);
            formData.append('tennganh', tennganh);
            formData.append('tenviettat', tenviettat);
            formData.append('ctdt', ctdt);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "TermAndMajor/editMajor",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnEditClose"]').prop('disabled', false);

                    Swal.fire({
                        title: "Thành công!",
                        text: "Đã lưu thông tin ngành " + manganh,
                        icon: "success"
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnEditClose"]').prop('disabled', false);

                    Swal.fire({
                        title: "Đã xảy ra lỗi!",
                        text: ketqua,
                        icon: "error"
                    }).then(() => {
                        window.location.reload();
                    });
                }
            });

        }
    });
});