$(document).ready(function () {
    $('body').on('click', '[id^="openSua-"]', function () {
        var formData = new FormData();
        formData.append('id', $(this).attr('name'));

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TermAndMajor/OpenEditFaculty",
            data: formData,
            dataType: 'html',
            type: 'POST',
            processData: false,
            contentType: false,
        }).done(function (ketqua) {
            if (ketqua.indexOf("Chi tiết lỗi") !== -1 || ketqua.indexOf("Khoa không tồn tại") !== -1) {
                Swal.fire({
                    title: "Đã xảy ra lỗi!",
                    text: ketqua,
                    icon: "error"
                }).then(() => {
                    window.location.reload();
                });
            }
            else {
                $('body').find('[id="capnhatkhoa-partial"]').replaceWith(ketqua);
                $('body').find('[id="capnhatTitle"]').text("Cập nhật khoa " + $('body').find('[id="edittenkhoa"]').val());
                $('body').find('[id="capnhat"]').modal('toggle');
            }
        });
    });

    $('body').on('click', '[id="btnEditSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnEditClose"]').prop('disabled', true);

        var tenkhoa = $('body').find('[id="edittenkhoa"]').val().trim();

        var validtenkhoa = $('body').find('[id="valid-edittenkhoa"]');

        validtenkhoa.text('');

        var check = true;

        if (tenkhoa.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnEditClose"]').prop('disabled', false);

            validtenkhoa.text("Vui lòng nhập tên khoa.");
            $('body').find('[id="edittenkhoa"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('id', $('body').find('[id="idk"]').val());
            formData.append('tenkhoa', tenkhoa);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "TermAndMajor/editFaculty",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                $('body').find('[id="capnhat"]').modal('toggle');
                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnEditClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: "Đã lưu thông tin khoa " + tenkhoa,
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnEditClose"]').prop('disabled', false);

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
});