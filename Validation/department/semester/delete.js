$(document).ready(function () {
    $('body').on('click', '[id^="openXoa-"]', function () {
        var fullname = $(this).attr('fullname');
        var formData = new FormData();
        formData.append('id', $(this).attr('name'));
        Swal.fire({
            text: 'Bạn có chắc muốn xóa học kỳ "' + fullname + '" không?',
            icon: "question",
            showCancelButton: true,
            cancelButtonColor: "#d33",
            confirmButtonText: "Xóa ngay!",
            cancelButtonText: "Hủy"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
                    url: $('#requestPath').val() + "Term/DeleteSemester",
                    data: formData,
                    dataType: 'html',
                    type: 'POST',
                    processData: false,
                    contentType: false,
                }).done(function (ketqua) {
                    Toast.fire({
                        icon: "success",
                        title: "Đã xóa thông tin học kỳ " + fullname
                    }).then(() => {
                        window.location.reload();
                    });
                    
                });
            }
        });
    });
});