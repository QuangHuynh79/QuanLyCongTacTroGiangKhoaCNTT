$(document).ready(function () {

    $('body').on('click', '[id="btnInfoSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnInfoClose"]').prop('disabled', true);

        var ma = $('body').find('[id="infoma"]').val().trim();
        var hoten = $('body').find('[id="infohoten"]').val().trim();
        var dienthoai = $('body').find('[id="infodienthoai"]').val().trim();
        var nganh = $('body').find('[id="infonganh"] :selected').val();

        var validma = $('body').find('[id="valid-infoma"]');
        var validhoten = $('body').find('[id="valid-infohoten"]');
        var validchucdanh = $('body').find('[id="valid-infochucdanh"]');
        var validdienthoai = $('body').find('[id="valid-infodienthoai"]');

        validma.text('');
        validhoten.text('');
        validchucdanh.text('');
        validdienthoai.text('');

        var specialChars = /[^a-zA-Z0-9 ]/g;

        var check = true;

        if (dienthoai.length > 0) {
            if (dienthoai.length != 10) {
                check = false;

                btn.html('Lưu thông tin');
                btn.prop('disabled', false);
                $('body').find('[id="btnInfoClose"]').prop('disabled', false);

                validdienthoai.text("Điện thoại phải đủ 10 số (Không dùng mã quốc gia).");
                $('body').find('[id="infodienthoai"]').focus();
            }
        }
     
        if (hoten.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnInfoClose"]').prop('disabled', false);

            validhoten.text("Vui lòng nhập họ & tên.");
            $('body').find('[id="infohoten"]').focus();
        }

        if (ma.length < 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnInfoClose"]').prop('disabled', false);

            validma.text("Vui lòng nhập mã tài khoản.");
            $('body').find('[id="infoma"]').focus();
        }
        else if (ma.indexOf(" ") !== - 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnInfoClose"]').prop('disabled', false);

            validma.text("Chỉ được nhập số-chữ không dấu và không có khoảng trắng!");
            $('body').find('[id="infoma"]').focus();
        }
        else if (ma.match(specialChars)) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnInfoClose"]').prop('disabled', false);

            validma.text("Chỉ được nhập số-chữ không dấu và không có khoảng trắng!");
            $('body').find('[id="infoma"]').focus();
        }

        if (check == true) {
            var formData = new FormData();
            formData.append('ma', ma);
            formData.append('hoten', hoten);
            formData.append('dienthoai', dienthoai);
            formData.append('nganh', nganh);
            formData.append('gioitinh', $('body').find('[id="infogioitinh"] :selected').val());
            formData.append('quoctich', $('body').find('[id="infoquoctich"]').prop('checked'));
            formData.append('ngaysinh', $('body').find('[id="infongaysinh"]').val());

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
                url: $('#requestPath').val() + "Users/UpdateInfo",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnInfoClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: "Đã cập nhật thông tin cá nhân."
                    });
                    
                }
                else if (ketqua == "Exist") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnInfoClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "error",
                        title: "Người dùng đã tồn tại hoặc Email/Mã đã được sử dụng."
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnInfoClose"]').prop('disabled', false);

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