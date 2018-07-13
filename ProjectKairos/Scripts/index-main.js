(function ($) {
    "use strict";

    /*[ Load page ]
    ===========================================================*/
    $(".animsition").animsition({
        inClass: 'fade-in',
        outClass: 'fade-out',
        inDuration: 1500,
        outDuration: 800,
        linkElement: '.animsition-link',
        loading: true,
        loadingParentElement: 'html',
        loadingClass: 'animsition-loading-1',
        loadingInner: '<div data-loader="ball-scale"></div>',
        timeout: false,
        timeoutCountdown: 5000,
        onLoadEvent: true,
        browser: ['animation-duration', '-webkit-animation-duration'],
        overlay: false,
        overlayClass: 'animsition-overlay-slide',
        overlayParentElement: 'html',
        transition: function (url) {
            window.location.href = url;
        }
    });

    /*[ Back to top ]
    ===========================================================*/
    var windowH = $(window).height() / 2;

    $(window).on('scroll', function () {
        if ($(this).scrollTop() > windowH) {
            $("#myBtn").css('display', 'flex');
        } else {
            $("#myBtn").css('display', 'none');
        }
    });

    $('#myBtn').on("click", function () {
        $('html, body').animate({
            scrollTop: 0
        }, 300);
    });


    /*[ Show header dropdown ]
    ===========================================================*/
    $('.js-show-header-dropdown').on('click', function () {
        $(this).parent().find('.header-dropdown')
    });

    var menu = $('.js-show-header-dropdown');
    var sub_menu_is_showed = -1;

    for (var i = 0; i < menu.length; i++) {
        $(menu[i]).on('click', function () {

            if (jQuery.inArray(this, menu) == sub_menu_is_showed) {
                $(this).parent().find('.header-dropdown').toggleClass('show-header-dropdown');
                sub_menu_is_showed = -1;
            } else {
                for (var i = 0; i < menu.length; i++) {
                    $(menu[i]).parent().find('.header-dropdown').removeClass("show-header-dropdown");
                }

                $(this).parent().find('.header-dropdown').toggleClass('show-header-dropdown');
                sub_menu_is_showed = jQuery.inArray(this, menu);
            }
        });
    }

    $(".js-show-header-dropdown, .header-dropdown").click(function (event) {
        event.stopPropagation();
    });

    $(window).on("click", function () {
        for (var i = 0; i < menu.length; i++) {
            $(menu[i]).parent().find('.header-dropdown').removeClass("show-header-dropdown");
        }
        sub_menu_is_showed = -1;
    });


    /*[ Fixed Header ]
    ===========================================================*/
    var posWrapHeader = $('.topbar').height();
    var header = $('.container-menu-header');

    $(window).on('scroll', function () {

        if ($(this).scrollTop() >= posWrapHeader) {
            $('.header1').addClass('fixed-header');
            $(header).css('top', -posWrapHeader);

        } else {
            var x = -$(this).scrollTop();
            $(header).css('top', x);
            $('.header1').removeClass('fixed-header');
        }

        if ($(this).scrollTop() >= 200 && $(window).width() > 992) {
            $('.fixed-header2').addClass('show-fixed-header2');
            $('.header2').css('visibility', 'hidden');
            $('.header2').find('.header-dropdown').removeClass("show-header-dropdown");

        } else {
            $('.fixed-header2').removeClass('show-fixed-header2');
            $('.header2').css('visibility', 'visible');
            $('.fixed-header2').find('.header-dropdown').removeClass("show-header-dropdown");
        }

    });

    /*[ Show menu mobile ]
    ===========================================================*/
    $('.btn-show-menu-mobile').on('click', function () {
        $(this).toggleClass('is-active');
        $('.wrap-side-menu').slideToggle();
    });

    var arrowMainMenu = $('.arrow-main-menu');

    for (var i = 0; i < arrowMainMenu.length; i++) {
        $(arrowMainMenu[i]).on('click', function () {
            $(this).parent().find('.sub-menu').slideToggle();
            $(this).toggleClass('turn-arrow');
        })
    }

    $(window).resize(function () {
        if ($(window).width() >= 992) {
            if ($('.wrap-side-menu').css('display') == 'block') {
                $('.wrap-side-menu').css('display', 'none');
                $('.btn-show-menu-mobile').toggleClass('is-active');
            }
            if ($('.sub-menu').css('display') == 'block') {
                $('.sub-menu').css('display', 'none');
                $('.arrow-main-menu').removeClass('turn-arrow');
            }
        }
    });


    /*[ remove top noti ]
    ===========================================================*/
    $('.btn-romove-top-noti').on('click', function () {
        $(this).parent().remove();
    })    


    /*[ Show content Product detail ]
    ===========================================================*/
    $('.active-dropdown-content .js-toggle-dropdown-content').toggleClass('show-dropdown-content');
    $('.active-dropdown-content .dropdown-content').slideToggle('fast');

    $('.js-toggle-dropdown-content').on('click', function () {
        $(this).toggleClass('show-dropdown-content');
        $(this).parent().find('.dropdown-content').slideToggle('fast');
    });


})(jQuery);

// Validate input for form -----------------------------------------------------------
(function () {
    'use strict';
    window.addEventListener('load', function () {
        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.getElementsByClassName('to-validate');
        // Loop over them and prevent submission
        var validation = Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    }, false);
})();

// Set max date input is today (DOB)--------------------------------------------------
var today = new Date();
var dd = today.getDate();
var mm = today.getMonth() + 1; //January is 0
var yyyy = today.getFullYear();
if (dd < 10) {
    dd = '0' + dd
}
if (mm < 10) {
    mm = '0' + mm
}
today = yyyy + '-' + mm + '-' + dd;
document.getElementById("validationCustom05").setAttribute("max", today);

//Check pasword + re-pasword
var check = function () {
    if ((document.getElementById('validationCustom07').value != "") &&
        (document.getElementById('validationCustom08').value != "")) {
        if (document.getElementById('validationCustom07').value ==
            document.getElementById('validationCustom08').value) {
            document.getElementById('message').style.color = 'green';
            document.getElementById('message').innerHTML = 'Matched!';
            return true;
        } else {
            document.getElementById('message').style.color = 'red';
            document.getElementById('message').innerHTML = 'Password and Confirm Password not matching!';
            return false;
        }
    }
};

//Check pasword + re-pasword
var checkA = function () {
    alert('a');
    if (document.getElementById('shipCity')[0].selectedIndex <= 0) {
        alert('ád');
        document.getElementById('message').style.color = 'green';
        document.getElementById('message').innerHTML = 'Matched!';
        return false;
    }
};

function checkAddress() {
    var flag = true;
    if ($("#shipCity")[0].selectedIndex <= 0) {
        document.getElementById('messageForCity').style.color = '#dc3545';
        document.getElementById('messageForCity').innerHTML = 'Please select City!';
        flag = false;
    } else {
        document.getElementById('messageForCity').style.color = 'green';
        document.getElementById('messageForCity').innerHTML = 'Valid!';
    }

    if ($("#shipDistrict")[0].selectedIndex <= 0) {
        document.getElementById('messageForDis').style.color = '#dc3545';
        document.getElementById('messageForDis').innerHTML = 'Please select District!';
        flag = false;
    } else {
        document.getElementById('messageForDis').style.color = 'green';
        document.getElementById('messageForDis').innerHTML = 'Valid!';
    }

    if ($("#shipWard")[0].selectedIndex <= 0) {
        document.getElementById('messageForWard').style.color = '#dc3545';
        document.getElementById('messageForWard').innerHTML = 'Please select Ward!';
        flag = false;
    } else {
        document.getElementById('messageForWard').style.color = 'green';
        document.getElementById('messageForWard').innerHTML = 'Valid!';
    }

    return flag;
}
