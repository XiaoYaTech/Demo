$(window).load(function () {

    // Page Preloader
    $('#status').fadeOut();
    $('#preloader').delay(350).fadeOut(function () {
        $('body').delay(350).css({ 'overflow': 'visible' });
    });
});
$(document).ready(function () {
    
    // Toggle Left Menu
    $('.leftpanel .nav-parent > a').on('click', function (event) {
        event.preventDefault();
        var parent = $(this).parent();
        var sub = parent.find('> ul');
        // Dropdown works only when leftpanel is not collapsed
        if (!$('body').hasClass('leftpanel-collapsed')) {
            if (sub.is(':visible')) {
                sub.slideUp(200, function () {
                    parent.removeClass('nav-active');
                });
            } else {
                closeVisibleSubMenu();
                parent.addClass('nav-active');
                sub.slideDown(200);
            }
        }
        return false;
    });

    function closeVisibleSubMenu() {
        $('.leftpanel .nav-parent').each(function () {
            var t = jQuery(this);
            if (t.hasClass('nav-active')) {
                t.find('> ul').slideUp(200, function () {
                    t.removeClass('nav-active');
                });
            }
        });
    }
    //Menu Toggle leftpanel collapsed
    $('.menutoggle').click(function () {

        var body = $('body');

        if (!body.hasClass('leftpanel-collapsed')) {
            body.addClass('leftpanel-collapsed');
            $('.nav-mcd ul').attr('style', '');
        } else {
            body.removeClass('leftpanel-collapsed');
        }
    });

    //Add class a mouse pointer hover over leftpanel nav
    $('.nav-mcd > li').hover(function () {
        $(this).addClass('nav-hover');
    }, function () {
        $(this).removeClass('nav-hover');
    });

    //background slider
    var imglen = $('.sliderbg li').length;
    var imgIndex = 0;
    var change = function () {
        $('.sliderbg li').eq(imgIndex++).addClass('active').siblings('li').removeClass('active');
        if (imgIndex == imglen) { imgIndex = 0; }
    },
    loop = setInterval(change, 4000);

    //stopPropagation onClick tabCheckbx & task
    $('.tab-filter').click(function (e) {
        e.stopPropagation();
    });
    $('#todoBtn').click(function () {
        $('#todoList').mixItUp({
            animation: {
                duration: 400,
                effects: 'fade translateZ(-360px) stagger(34ms)',
                easing: 'ease'
            },
            selectors: {
                target: '.mix',
                filter: '.filter'
            }
        });
    });
});