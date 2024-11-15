$(document).ready(function () {
    $('body').on('click', '[id^="applyLHP-"]', function () {
        var btn = $(this);
        var id = btn.attr('name');
        var idForm = btn.attr('IdFormDangKy');
        var typeLHP = btn.attr('typeLHP');

        var formData = new FormData();
        formData.append('id', id);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signout"; } },
            url: $('#requestPath').val() + "TeachingAssistant/OpenApply",
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
                $('body').find('[id="applyTitle"]').text(btn.attr('titleForm'));
                $('body').find('[id="contentApply"]').replaceWith(ketqua);

                if ($('body').find('[id="daduocduyet"]').val() == "1") {
                    if (typeLHP == "dadk") {
                        $('body').find('[id="btnHuydangky"]').prop('hidden', false);
                        $('body').find('[id="btnSubmit"]').prop('hidden', false);
                    }
                    else {
                        $('body').find('[id="btnHuydangky"]').prop('hidden', true);
                        $('body').find('[id="btnSubmit"]').prop('hidden', false);
                    }
                }
                else {
                    $('body').find('[id="btnHuydangky"]').prop('hidden', true);
                    $('body').find('[id="btnSubmit"]').prop('hidden', true);
                }

                $('body').find('[id="idFORMDKY"]').val(idForm);
                $('body').find('[id="apply"]').modal('toggle');
            }
        });

    });

    $('body').on('click', '[id^="applyTASK-"]', function () {
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

});