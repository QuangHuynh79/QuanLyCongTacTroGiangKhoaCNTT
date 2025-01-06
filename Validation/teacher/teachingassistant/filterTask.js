$(document).ready(function () {
    $('body').on('change', '[id="hocky"]', function () {
        FilterDataHocKy();
    });
    $('body').on('change', '[id="lophocphan"]', function () {
        FilterData();
    });

    function FilterData() {

        $('body').find('[id="content-filterLHP"]').html('<h3 class="text-center mb-3"><span class="spinner-border spinner-border-sm me-2" style="width: 18px; height: 18px" role="status" aria-hidden="true"></span>Đang tải...</h3>');

        var lophocphan = $('body').find('[id="lophocphan"] :selected').val();

        var formData = new FormData();
        formData.append('lophocphan', lophocphan);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TaskManagement/FilterTaskList",
            dataType: 'html',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false
        }).done(function (ketqua) {
            $('body').find('[id="content-filterLHP"]').replaceWith(ketqua);
        });
    }

    function FilterDataHocKy() {

        $('body').find('[id="content-filterLHP"]').html('<h3 class="text-center mb-3"><span class="spinner-border spinner-border-sm me-2" style="width: 18px; height: 18px" role="status" aria-hidden="true"></span>Đang tải...</h3>');

        var hocky = $('body').find('[id="hocky"] :selected').val();

        var formData = new FormData();
        formData.append('hocky', hocky);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TaskManagement/FilterHocKyTaskList",
            dataType: 'html',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false
        }).done(function (ketqua) {
            $('body').find('[id="lophocphan"]').replaceWith(ketqua);
            FilterData();
        });
    }
});