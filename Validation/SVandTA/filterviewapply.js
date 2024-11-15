$(document).ready(function () {
    $('body').on('change', '[id="hocky"]', function () {
        FilterParentData();
    });
    $('body').on('change', '[id="nganh"]', function () {
        FilterParentData();
    });
    $('body').on('change', '[id="locgiangvien"]', function () {
        FilterData();
    });
    $('body').on('change', '[id="loctrangthai"]', function () {
        FilterData();
    });
    $('body').on('change', '[id="locmonhoc"]', function () {
        FilterData();
    });

    function FilterParentData() {

        $('body').find('[id="filterParentLoad"]').html('<h3 class="text-center mt-4 mb-3"><span class="spinner-border spinner-border-sm me-2" style="width: 18px; height: 18px" role="status" aria-hidden="true"></span>Đang tải...</h3>');

        var hocky = $('body').find('[id="hocky"] :selected').val();
        var nganh = $('body').find('[id="nganh"] :selected').val();

        var formData = new FormData();
        formData.append('hocky', hocky);
        formData.append('nganh', nganh);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TeachingAssistant/FilterParentApply",
            dataType: 'html',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false
        }).done(function (ketqua) {
            $('body').find('[id="filterParentLoad"]').replaceWith(ketqua);
        });
    }

    function FilterData() {

        var hocky = $('body').find('[id="hocky"] :selected').val();
        var nganh = $('body').find('[id="nganh"] :selected').val();
        var mon = "";
        var gv = "";
        var trangthai = $('body').find('[id="loctrangthai"] :selected').val();
        $('body').find('[id="locmonhoc"] :selected').each(function () {
            mon += $(this).val() + "#";
        });
        $('body').find('[id="locgiangvien"] :selected').each(function () {
            gv += $(this).val() + "#";
        });

        var formData = new FormData();
        formData.append('hocky', hocky);
        formData.append('nganh', nganh);
        formData.append('mon', mon.substring(0, mon.length - 1));
        formData.append('gv', gv.substring(0, gv.length - 1));
        formData.append('trangthai', trangthai);

        $.ajax({
            error: function (a, xhr, c) { if (a.status == 403 && a.responseText.indexOf("SystemLoginAgain") != -1) { window.location.href = $('body').find('[id="requestPath"]').val() + "account/signin"; } },
            url: $('#requestPath').val() + "TeachingAssistant/FilterChildApply",
            dataType: 'html',
            data: formData,
            type: 'POST',
            processData: false,
            contentType: false
        }).done(function (ketqua) {
            $('body').find('[id="filterLoad"]').replaceWith(ketqua);

        });
    }
});