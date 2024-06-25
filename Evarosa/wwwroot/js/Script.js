$(document).ready(function () {
    //Scroll event
    $(window).scroll(function () {
        var scrolled = $(window).scrollTop();
        if (scrolled > 200) $('.go-top').fadeIn('slow');
        if (scrolled < 200) $('.go-top').fadeOut('slow');
    });

    //Click event
    $('.go-top').click(function () {
        $("html, body").animate({ scrollTop: "0" }, 500);
    });
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
        dots: true,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    arrows: false,
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2,
                    arrows: false,
                }
            },
            {
                breakpoint: 540,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false,
                }
            }
        ]
    });

    $('.partner-slider').slick({
        infinite: true,
        slidesToShow: 5,
        slidesToScroll: 5,
        speed: 800,
        arrows: true,
        dots: true,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    arrows: false,
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2,
                    arrows: false,
                }
            }
        ]
    });

    $(".outstanding-tabs > li").click(function () {
        $('.outstanding-content').slick('refresh');
    });

    $(".outstanding").tabs();

    $("#AlertBox").removeClass('hide');
    $("#AlertBox").delay(10000).slideUp(800);


    $(".input-number").niceNumber({
        autoSize: true,
        autoSizeBuffer: 1
    });

    if ($(".product-slider-main").length) {
        var $slider = $(".product-slider-main").on("init", function (slick) {
            $(".product-slider-main").fadeIn(1000);
        }).slick({
            slidesToShow: 1,
            slidesToScroll: 1,
            arrows: true,
            lazyLoad: "ondemand",
            autoplaySpeed: 3000,
            asNavFor: ".product-slider-thmb"
        });

        var thumbnailsSlider = $(".product-slider-thmb").on("init", function (slick) {
            $(".product-slider-thmb").fadeIn(1000);
        }).slick({
            slidesToShow: 4,
            slidesToScroll: 1,
            lazyLoad: "ondemand",
            asNavFor: ".product-slider-main",
            dots: false,
            centerMode: false,
            focusOnSelect: true
        });

        $(".product-slider-thmb .slick-slide").removeClass("slick-active");
        $(".product-slider-thmb .slick-slide").eq(0).addClass("slick-active");

        $(".product-slider-main").on("beforeChange", function (event, slick, currentSlide, nextSlide) {
            var mySlideNumber = nextSlide;
            $(".product-slider-thmb .slick-slide").removeClass("slick-active");
            $(".product-slider-thmb .slick-slide").eq(mySlideNumber).addClass("slick-active");
        });
    }

    $("#addToCart").submit(function (event) {
        event.preventDefault();

        var form = $(this);

        $.post("/ShoppingCart/AddToCart", {
            id: form.find('input[name="id"]').val(),
            skuId: form.find('input[name="sku"]').val(),
            quantity: form.find('input[name="qty"]').val(),
        }, function (data) {
            if (data.status) {
                Swal.fire({
                    title: "Thành công!",
                    text: data.msg,
                    icon: "success",
                    showCancelButton: true,
                    confirmButtonColor: "#3085d6",
                    cancelButtonColor: "#d33",
                    confirmButtonText: "Xem giỏ hàng"
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location = "/gio-hang";
                    }
                });
            } else {
                Swal.fire({
                    title: "Thất bại!",
                    text: data.msg,
                    icon: "error"
                });
            }

            LoadCartMini();
            $(".shoppingcart > .count").text(data.count);
        });
    });

    $(".input-cart").niceNumber({
        autoSize: true,
        autoSizeBuffer: 1,
        onDecrement: function (input, number, object) {
            updateFromCart($(input));
            return false;
        },
        onIncrement: function (input, number, object) {
            updateFromCart($(input));
            return false;
        }
    });

     $("#form-contact").on("submit", function (e) {
        e.preventDefault();

        if ($(this).valid()) {
            $.ajax({
                type: "POST",
                url: "/Home/ContactStore",
                data: $(this).serialize(),
                success: function (data) {
                    if (data.status) {
                        Swal.fire({
                            title: "Thành công!",
                            text: data.msg,
                            icon: "success"
                        });
                        $("#form-contact").trigger("reset");
                    }
                },
                error: function (error) {
                    Swal.fire({
                        title: "Thất bại!",
                        text: "Đã có lỗi xảy ra trong quá trình xử lý!",
                        icon: "error"
                    });
                }
            });
        }
     });

    $(".product-option").change(function () {
        $.get("/Home/GetSku", {
            skuId: $(this).val(),
        }, function (data) {
            $(".product-sku").text(data.sku);
            $(".product-price .price").text(data.price);
            $(".product-price .price-old del").text(data.priceOld);
        });
    })
})


function BuyNow() {
    var form = $("#addToCart");

    $.post("/ShoppingCart/AddToCart", {
        id: form.find('input[name="id"]').val(),
        skuId: form.find('input[name="sku"]').val(),
        quantity: form.find('input[name="qty"]').val(),
    }, function (data) {
        if (data.status) {
            Swal.fire({
                title: "Thành công!",
                text: data.msg,
                icon: "success",
                showCancelButton: false,
                confirmButtonColor: "#3085d6",
                confirmButtonText: "Ok"
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location = "/thanh-toan";
                }
            });
        } else {
            Swal.fire({
                title: "Thất bại!",
                text: data.msg,
                icon: "error"
            });
        }

        LoadCartMini();
        $(".shoppingcart > .count").text(data.count);
    });
}

function LoadCartMini() {
    $.get("/ShoppingCart/CartMini", function (data) {
        $(".shoppingcart-mini").html(data);
    });
}

function removeFromCart(recordId) {
    if (confirm("Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng!!!")) {
        $.post("/ShoppingCart/RemoveFromCart", {
            recordId
        }, function (data) {
            if (data.status) {
                LoadCartMini();
                $(`table.cart tr[data-id="${recordId}"]`).fadeOut();
            } else {
                Swal.fire({
                    title: "Thất bại!",
                    text: data.msg,
                    icon: "error"
                });
            }

            $(".shopping-cart > .count").text(data.count);
        });
    }
}

function updateFromCart(input) {
    $.post("/ShoppingCart/UpdateFromCart", {
        recordId: input.data("recordid"),
        quantity: input.val(),
    }, function (data) {
        console.log(data)

        if (data.status) {
            LoadCartMini();
            $(`table.cart tr[data-id="${input.data("recordid")}"]`).find(".item-total").text(data.itemTotal);
        } else {
            Swal.fire({
                title: "Thất bại!",
                text: data.msg,
                icon: "error"
            });
        }

        $(".shoppingcart > .count").text(data.count);
        $(".order .sum-value").text(data.total);
    });
}

$(document).ready(function () {
    // Function to populate districts based on selected city
    $("#cityDropdown").change(function () {
        var cityId = $(this).val();
        if (cityId) {
            $.ajax({
                url: "/quan-huyen",
                type: "GET",
                data: { cityId: cityId },
                success: function (response) {
                    console.log(response);
                    $("#districtDropdown").empty();
                    $("#districtDropdown").append($("<option>").text("Chọn Quận/Huyện").attr("value", ""));
                    $("#wardDropdown").empty();
                    $("#wardDropdown").append($("<option>").text("Chọn Xã/Phường").attr("value", ""));
                    $.each(response.districts, function (index, district) {
                        $("#districtDropdown").append($("<option>").text(district.name).attr("value", district.id));
                    });
                }
            });
        } else {
            $("#districtDropdown").empty();
            $("#districtDropdown").append($("<option>").text("Chọn Quận/Huyện").attr("value", ""));
            $("#wardDropdown").empty();
            $("#wardDropdown").append($("<option>").text("Chọn Xã/Phường").attr("value", ""));
        }
    });

    // Function to populate wards based on selected district
    $("#districtDropdown").change(function () {
        var districtId = $(this).val();
        if (districtId) {
            $.ajax({
                url: "/xa-phuong",
                type: "GET",
                data: { districtId: districtId },
                success: function (response) {
                    $("#wardDropdown").empty();
                    $("#wardDropdown").append($("<option>").text("Chọn Xã/Phường").attr("value", ""));
                    $.each(response.wards, function (index, ward) {
                        $("#wardDropdown").append($("<option>").text(ward.name).attr("value", ward.id));
                    });
                }
            });
        } else {
            $("#wardDropdown").empty();
            $("#wardDropdown").append($("<option>").text("Chọn Xã/Phường").attr("value", ""));
        }
    });

    $("#wardDropdown").change(function () {
        $.ajax({
            url: "/shipfee",
            type: "GET",
            data: { cityId: $("#cityDropdown").val() },
            success: function (data) {
                if (data.status) {
                    $(".sum-value.shipfee").text(data.fee);
                    $(".sum-value.total").text(data.total);
                }
            }
        });
    });
});

function ListProductJS() {
    $(".by-sort").change(function () {
        let cate = $(".filter-by-category").val();
        let sort = $(".by-sort").val();

        $.ajax({
            url: "/Home/ListProductView",
            type: "GET",
            data: {
                url: cate,
                typeSort: sort,
            }
        }).done(function (data) {
            $("#ListProductView").empty();
            $("#ListProductView").html(data);
            ListProductJS()

            var currentURL = window.location.href;

            var urlObject = new URL(currentURL);

            var searchParams = urlObject.searchParams;

            searchParams.set("typeSort", sort);

            var newURL = urlObject.href;

            window.history.pushState({ path: newURL }, "", newURL);
        });
    });
}

function AllProductJS() {
    $(".by-sort").change(function () {
        let sort = $(".by-sort").val();

        $.ajax({
            url: "/Home/AllProductView",
            type: "GET",
            data: {
                typeSort: sort,
            }
        }).done(function (data) {
            $("#AllProductView").empty();
            $("#AllProductView").html(data);
            AllProductJS()
            var currentURL = window.location.href;

            var urlObject = new URL(currentURL);

            var searchParams = urlObject.searchParams;

            searchParams.set("typeSort", sort);

            var newURL = urlObject.href;

            window.history.pushState({ path: newURL }, "", newURL);
        });
    });
}

function menuOpen() {
    $(".nav-mobi").addClass("active");
}
function menuClose() {
    $(".nav-mobi").removeClass("active");
}
function toggleSubmenu(button) {
    $(button).closest("li").children("ul").slideToggle();
}
