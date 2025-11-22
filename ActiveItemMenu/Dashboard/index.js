$(document).ready(function () {
    //Close
    $('body').find('.parent-nav-link').each(function () { $(this).attr('aria-expanded', 'false'); $(this).addClass('collapsed'); })
    $('body').find('.parent-navs').each(function () { $(this).removeClass('show'); });
    $('body').find('.child-nav-link').each(function () { $(this).removeClass('active'); });

    //Opent
    $('body').find('#parent-studentaffairs-studentaffairsdashboard').attr('aria-expanded', 'true');
    $('body').find('#parent-studentaffairs-studentaffairsdashboard').removeClass('collapsed');
    $('body').find('#nav-studentaffairs-studentaffairsdashboard').addClass('show');
    $('body').find('#child-studentaffairs-studentaffairsdashboard-index').addClass('active');
});