
$(document).ready(function () {
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

        var role = $('body').find('[id="roletask"]').val();

        var formData = new FormData();
        formData.append('id', $('body').find('[id="idtask"]').val());

        if (role == "gv") {
            formData.append('trangthai', $('body').find('[id="tinhtrang"]').val());
            formData.append('role', role);
        }
        else {
            formData.append('trangthai', $('body').find('[id="trangthai"]').val());
            formData.append('role', role);
        }
        formData.append('ghichu', $('body').find('[id="ghichu"]').val());

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
                btn.html("Lưu thông tin");
                btn.prop('disabled', false);
                $('body').find('[id="btnClose"]').prop('disabled', false);

                Toast.fire({
                    icon: "success",
                    title: "Đã cập nhật kết quả công việc"
                });

                FilterDatas();
            }
        });
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
});