$(document).ready(function () {
    //Close
    $('body').find('.parent-nav-link').each(function () { $(this).attr('aria-expanded', 'false'); $(this).addClass('collapsed'); })
    $('body').find('.parent-navs').each(function () { $(this).removeClass('show'); });
    $('body').find('.child-nav-link').each(function () { $(this).removeClass('active'); });

    //Opent
    $('body').find('#parent-studentaffairs-studentaffairssemesterandmajor').attr('aria-expanded', 'true');
    $('body').find('#parent-studentaffairs-studentaffairssemesterandmajor').removeClass('collapsed');
    $('body').find('#nav-studentaffairs-studentaffairssemesterandmajor').addClass('show');
    $('body').find('#child-studentaffairs-studentaffairssemesterandmajor-hocky').addClass('active');
});