
$(document).ready(function () {
    $('body').on('change', '[id="hinhanhmc"]', function () {
        var img = $(this);

        $('body').find('[id="valid-hinhanhmc"]').text('');

        var anhcuCount = 0;
        $('body').find('[id="anhcu"]').each(function () {
            anhcuCount++;
        });

        if (img[0].files.length + anhcuCount > 1) {
            $('body').find('[id="valid-hinhanhmc"]').text('Vui lòng gửi tối đa 1 hình ảnh < 10MB.');
            img.val(null);
        }
        else {
            if (img[0].files.length > 0) {
                var fileName = img[0].files[0].name;
                var sizes = (img[0].files[0].size / 1024).toFixed(1);

                if (fileName.substring(fileName.lastIndexOf('.'), fileName.length - 1).toLowerCase() == ".jpg"
                    || fileName.substring(fileName.lastIndexOf('.'), fileName.length - 1).toLowerCase() == ".jpeg"
                    || fileName.substring(fileName.lastIndexOf('.'), fileName.length - 1).toLowerCase() == ".png") {

                    $('body').find('[id="valid-hinhanhmc"]').text("Vui lòng chọn hình ảnh có định dạng JPG, JPEG, PNG.");
                    img.val(null);
                }
                else if (sizes > (10 * 1024)) {
                    $('body').find('[id="valid-hinhanhmc"]').text("Vui lòng gửi tối đa 1 hình ảnh < 10MB.");
                    img.val(null);
                }
            }
        }
    });

    $('body').on('click', '[id^="btnTaskDetail-"]', function () {
        var id = $(this).attr('name');
        var tieude = $(this).attr("tieudeForm");
        $('body').find('[id="chitietmodalTitle"]').text(tieude);

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/TaskDetail",
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
                $('body').find('[id="contentTaskDetail"]').replaceWith(ketqua);
                if ($('body').find('[id="hideBtnSubmit"]').val() == "true") {
                    $('body').find('[id="btnSubmit"]').prop('hidden', true);
                }
                else {
                    $('body').find('[id="btnSubmit"]').prop('hidden', false);
                }
                $('body').find('[id="chitietmodal"]').modal('toggle');
            }
        });
    });

    $('body').on('click', '[id="btnSubmit"]', function () {
        var btn = $(this);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"> </span> Vui lòng chờ...');
        btn.prop('disabled', true);
        $('body').find('[id="btnClose"]').prop('disabled', true);

        var validhamc = $('body').find('[id="valid-hinhanhmc"]');
        validhamc.text('');

        var urlImg = $('body').find('[id="file-delete-list"]').val().trim();
        var deleteImg = false;
        if (urlImg.length > 0) {
            deleteImg = true;
        }
        var check = true;
        var hamc = $('body').find('[id="hinhanhmc"]');
        var role = $('body').find('[id="roletask"]').val();

        var formData = new FormData();
        formData.append('id', $('body').find('[id="idtask"]').val());

        var anhcuCount = 0;
        $('body').find('[id^="anhcu"]').each(function () {
            anhcuCount++;
        });
        if (hamc[0].files.length + anhcuCount > 1) {
            check = false;

            btn.html('Lưu thông tin');
            btn.prop('disabled', false);
            $('body').find('[id="btnClose"]').prop('disabled', false);

            validhamc.text("Vui lòng gửi tối đa 1 hình ảnh < 10MB.");
            $('body').find('[id="hamc"]').focus();
            hamc.val(null);
        }
        else {
            if (hamc[0].files.length > 0) {
                var sizes = (hamc[0].files[0].size / 1024).toFixed(1);
                if (sizes > (10 * 1024)) {
                    check = false;

                    btn.html('Lưu thông tin');
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    validhamc.text("Vui lòng gửi tối đa 1 hình ảnh < 10MB.");
                    hamc.val(null);
                }
            }
        }

        if (role == "gv") {
            formData.append('trangthai', $('body').find('[id="tinhtrang"]').val());
            formData.append('role', role);
        }
        else {
            formData.append('trangthai', $('body').find('[id="trangthai"]').val());
            formData.append('role', role);
        }
        formData.append('ghichu', $('body').find('[id="ghichu"]').val());
        formData.append('hamc', hamc[0].files[0]);
        formData.append('deleteImg', deleteImg);


        if (check == true) {
            $.ajax({
                error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
                url: $('#requestPath').val() + "TeachingAssistant/SubmitEditTaskDetail",
                data: formData,
                dataType: 'html',
                type: 'POST',
                processData: false,
                contentType: false,
            }).done(function (ketqua) {
                $('body').find('[id="chitietmodal"]').modal('toggle');

                if (ketqua.indexOf("Chi tiết lỗi") !== -1) {
                    Toast.fire({
                        icon: "error",
                        title: ketqua
                    }).then(() => {
                        window.location.reload();
                    });
                }
                else {
                    var noti = "Đã cập nhật kết quả công việc.";
                    if (role !== "gv") {
                        noti = "Đã cập nhật trạng thái công việc.";
                    }

                    btn.html("Lưu thông tin");
                    btn.prop('disabled', false);
                    $('body').find('[id="btnClose"]').prop('disabled', false);

                    Toast.fire({
                        icon: "success",
                        title: noti
                    });

                    FilterDatas();
                }
            });
        }
    });
    function FilterDatas() {
        var lophocphan = $('body').find('[id="lophocphan"] :selected').val();

        var formData = new FormData();
        formData.append('lophocphan', lophocphan);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TeachingAssistant/FilterTaskList",
            dataType: 'html',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false
        }).done(function (ketqua) {
            $('body').find('[id="content-filterLHP"]').replaceWith(ketqua);
        });
    }

    $('body').on('click', '[id^="delete-anhcu"]', function () {
        var img = $(this);

        var urlImg = $('body').find('[id="anhcu"]').val();
        $('body').find('[id="file-delete-list"]').val(urlImg);

        $('body').find('[id="parent-anhcu"]').replaceWith(null);

        img.tooltip("hide");
    });

});