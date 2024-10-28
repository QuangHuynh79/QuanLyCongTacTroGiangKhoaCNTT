$(document).ready(function () {
    $('body').on('click', '[id^="btnDanhgia-"]', function () {
        var id = $(this).attr('name');
        var urls = $('#requestPath').val() + "ReviewTask/OpenReviewTask";
        var types = "edit";
        var titleForm = $(this).attr('titleForm');

        OpenDanhGia(id, urls, types, titleForm);
    }); 
    $('body').on('click', '[id^="btnXemDanhgia-"]', function () {
        var id = $(this).attr('name');
        var urls = $('#requestPath').val() + "ReviewTask/OpenReviewTask";
        var types = "view";
        var titleForm = $(this).attr('titleForm');

        OpenDanhGia(id, urls, types, titleForm);
    });

    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var lsttrangthai = "";
        var lstid = "";
        var check = true;

        var giothucte = $('body').find('[id^="thucte"]').val().trim();
        if (giothucte.length < 1) {
            $('body').find('[id^="thucte-"]').val("0");
            giothucte = 0;
        }

        $('body').find('[id^="select-danhgia-"]').each(function () {
            lstid += $(this).attr('name') + "#";
            lsttrangthai += $(this).prop("checked") + "#";
        });

        if (check == true && lstid.length > 0) {
            lstid = lstid.substring(0, lstid.length - 1);
            lsttrangthai = lsttrangthai.substring(0, lsttrangthai.length - 1);

            var formData = new FormData();
            formData.append('lstid', lstid);
            formData.append('lsttrangthai', lsttrangthai);
            formData.append('giothucte', giothucte);

            formData.append('ghichu', $('body').find('[id="ghichu"]').val());

            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                url: $('#requestPath').val() + "ReviewTask/SubmitReviewTask",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                $('body').find('[id="danhgiachung"]').modal('toggle');
                if (ketqua == "SUCCESS") {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: "Đã đánh giá công việc trợ giảng.",
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else {
                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

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

    function OpenDanhGia(id, urls, types, titleForm) {
        var formData = new FormData();
        formData.append('id', id);
        formData.append('type', types);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: urls,
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
                $('body').find('[id="content-partialDetail"]').replaceWith(ketqua);
                $('body').find('[id="danhgiachungTitle"]').text(titleForm);

                if (types == "edit") {
                    $('body').find('[id="btnSubmit"]').prop('hidden', false);
                }
                else {
                    $('body').find('[id="btnSubmit"]').prop('hidden', true);
                }

                $('body').find('[id="danhgiachung"]').modal('toggle');
            }
        });
    }

});