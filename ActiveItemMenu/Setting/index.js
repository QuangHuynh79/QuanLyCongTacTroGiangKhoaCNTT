$(document).ready(function () {
    //Close
    $('body').find('.parent-nav-link').each(function () { $(this).attr('aria-expanded', 'false'); $(this).addClass('collapsed'); })
    $('body').find('.parent-navs').each(function () { $(this).removeClass('show'); });
    $('body').find('.child-nav-link').each(function () { $(this).removeClass('active'); });

    //Opent
    $('body').find('#parent-setting').attr('aria-expanded', 'true');
    $('body').find('#parent-setting').removeClass('collapsed');
    $('body').find('#nav-setting').addClass('show');
    $('body').find('#child-setting-index').addClass('active');
});