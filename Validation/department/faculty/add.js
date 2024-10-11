$(document).ready(function () {
    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var tenkhoa = $('body').find('[id="tenkhoa"]').val().trim();

        var validtenkhoa = $('body').find('[id="valid-tenkhoa"]');

        validtenkhoa.text('');

        var check = true;
       
        if (tenkhoa.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validtennganh.text("Vui lòng nhập tên khoa.");
            $('body').find('[id="tenkhoa"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('tenkhoa', tenkhoa);

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "TermAndMajor/addFaculty",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Swal.fire({
                        title: "Thành công!",
                        text: "Đã lưu thông tin khoa mới.",
                        icon: "success"
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else if (ketqua == "Exist") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Swal.fire({
                        title: "Thất bại!",
                        text: "Khoa " + tenkhoa + " đã tồn tại trên hệ thống.",
                        icon: "warning"
                    });
                }
                else {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

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