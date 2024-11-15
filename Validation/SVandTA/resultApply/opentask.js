$(document).ready(function () {
    $('body').on('change', '[id="hocky"]', function () {
        FilterParentData();
    });
    $('body').on('change', '[id="nganh"]', function () {
        FilterParentData();
    });

    $('body').on('click', '[id^="Opentasklist-"]', function () {
        var btn = $(this);
        var id = btn.attr('name');

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/OpenTaskListDetail",
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
                $('body').find('[id="taskListTitle"]').text(btn.attr('titleTaskList'));
                $('body').find('[id="contentTaskList"]').replaceWith(ketqua);
                $('body').find('[id="taskList"]').modal('toggle');
            }
        });

    });
    function FilterParentData() {

        $('body').find('[id="load-content-filter"]').html('<h3 class="text-center mt-4 mb-3"><span class="spinner-border spinner-border-sm me-2" style="width: 18px; height: 18px" role="status" aria-hidden="true"></span>Đang tải...</h3>');

        var hocky = $('body').find('[id="hocky"] :selected').val();
        var nganh = $('body').find('[id="nganh"] :selected').val();

        var formData = new FormData();
        formData.append('hocky', hocky);
        formData.append('nganh', nganh);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TeachingAssistant/FilterResultApply",
            dataType: 'html',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false
        }).done(function (ketqua) {
            $('body').find('[id="load-content-filter"]').replaceWith(ketqua);
        });
    }

});