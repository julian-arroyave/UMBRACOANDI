//#region  link-contenedor

$(document).ready(function () {
    $(".link-contenedor").click(function () {
        var url = $(this).attr("href");
        location.href = url;
    });
    $(".link-contenedor-blank").click(function () {
        var url = $(this).attr("href");
        window.open(url, "_blank");
        window.focus();
    });
});

//#endregion

//#region slider-home

$(document).ready(function () {

    $(".ico-slider").click(function () {
        $(this).parent().find(".ico-slider").removeClass("active");
        $(this).addClass("active");
        var idSlider = parseInt($(this).attr("idslider"));

        $(this).parent().parent().parent().find('.slider-content').each(function (index) {
            if (parseInt($(this).attr("idslider")) == idSlider) {
                $(this).parent().parent().parent().find('.slider-content').hide();
                $(this).fadeIn(700);
            }
        });
    });




    $(".ico-slider-ppal").click(function () {
        $(".ico-slider-ppal").removeClass("active");
        $(this).addClass("active");
        var idSlider = parseInt($(this).attr("idslider"));

        $('.slider-ppal').each(function (index) {
            if (parseInt($(this).attr("idslider")) == idSlider) {
                $(".slider-ppal").hide();
                $(this).fadeIn(700);
            }
        });
    });


    setInterval("NextSlider()", 30000);

    $(".button-up").click(function () {
        NextSlider();
    })

});



function NextSlider() {
    var active = parseInt($("#buttom-slider-ppal .active").attr("idslider"));
    var numSliders = parseInt($(".ico-slider-ppal").length);
    if (active + 1 == numSliders) {
        active = 0
    }
    else { active++; }

    $(".ico-slider-ppal").removeClass("active");

    $('.ico-slider-ppal').each(function (index) {

        if (parseInt($(this).attr("idslider")) == active) {
            $(this).addClass("active");
        }

    });

    $('.slider-ppal').each(function (index) {
        if (parseInt($(this).attr("idslider")) == active) {
            $(".slider-ppal").hide();
            $(this).fadeIn(1400);
        }
    });
}


//#endregion
