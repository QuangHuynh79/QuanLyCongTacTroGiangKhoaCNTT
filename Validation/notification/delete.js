$(document).ready(function () {
    $('body').on('click', '[id^="deleteNoti-"]', function () {
        var id = $(this).attr('name');

        var formData = new FormData();
        formData.append('id', id);
        Swal.fire({
            title: 'Xóa bỏ?',
            text: 'Bạn có chắc muốn xóa thông báo không?',
            icon: "question",
            showCancelButton: true,
            cancelButtonColor: "#d33",
            confirmButtonText: "Xóa ngay!",
            cancelButtonText: "Hủy bỏ"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                    url: $('#requestPath').val() + "Notifications/Delete",
                    data: formData,
                    dataType: 'html',
                    type: 'POST',
                    processData: false,
                    contentType: false,
                }).done(function (ketqua) {
                    $('body').find('[id="cpn-thongbao-' + id + '"]').replaceWith('');

                    Toast.fire({
                        icon: "success",
                        title: "Đã xóa thông báo."
                    });
                });
            }
        });
    });
});