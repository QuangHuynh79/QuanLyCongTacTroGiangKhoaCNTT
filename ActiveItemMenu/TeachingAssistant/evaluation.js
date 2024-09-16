$(document).ready(function () {
    //Close
    $('body').find('.parent-nav-link').each(function () { $(this).attr('aria-expanded', 'false'); $(this).addClass('collapsed'); })
    $('body').find('.parent-navs').each(function () { $(this).removeClass('show'); });
    $('body').find('.child-nav-link').each(function () { $(this).removeClass('active'); });

    //Opent
    $('body').find('#parent-trogiang').attr('aria-expanded', 'true');
    $('body').find('#parent-trogiang').removeClass('collapsed');
    $('body').find('#nav-trogiang').addClass('show');
    $('body').find('#child-trogiang-evaluation').addClass('active'); 
});