$(document).ready(function () {
    //Close
    $('body').find('.parent-nav-link').each(function () { $(this).attr('aria-expanded', 'false'); $(this).addClass('collapsed'); })
    $('body').find('.parent-navs').each(function () { $(this).removeClass('show'); });
    $('body').find('.child-nav-link').each(function () { $(this).removeClass('active'); });

    //Opent
    $('body').find('#parent-admin-admindashboard').attr('aria-expanded', 'true');
    $('body').find('#parent-admin-admindashboard').removeClass('collapsed');
    $('body').find('#nav-admin-admindashboard').addClass('show');
    $('body').find('#child-admin-admindashboard-index').addClass('active');
});