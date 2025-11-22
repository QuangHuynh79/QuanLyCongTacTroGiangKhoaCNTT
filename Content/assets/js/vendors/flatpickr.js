var flatpickrElements;
0 < document.querySelectorAll(".flatpickr").length && (flatpickrElements = document.querySelectorAll(".flatpickr")).forEach(function (l) {
    flatpickr(l, {
        locale: 'vn',
        disableMobile: !0,
        allowInput: false,
    })
});