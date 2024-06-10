$(document).ready(function () {
    $('.banner').slick({
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 800,
        arrows: false,
        dots: true
    });

    $('.outstanding-content').slick({
        infinite: true,
        slidesToShow: 5,
        slidesToScroll: 5,
        speed: 800,
        arrows: true,
        dots: true
    });

    $('.partner-slider').slick({
        infinite: true,
        slidesToShow: 5,
        slidesToScroll: 5,
        speed: 800,
        arrows: true,
        dots: true
    });

    $(".outstanding").tabs();
})