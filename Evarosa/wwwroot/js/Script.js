window.addEventListener("load", function () {
    //Scroll event
    $(window).on("scroll",function () {
        const scrolled = $(window).scrollTop();
        if (scrolled > 200) $('.go-top').fadeIn('slow');
        if (scrolled < 200) $('.go-top').fadeOut('slow');
    });

    $(window).one("scroll", function () {
        FB.XFBML.parse();
    })

    //Click event
    $(".go-top").click(function () {
        $("html, body").animate({ scrollTop: "0" }, 200);
    });
    $('.banner').slick({
        infinite: true,
        autoplay: true,
        autoplaySpeed: 3000,
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: false,
        dots: true
    });

    $('.outstanding-content').slick({
        infinite: true,
        slidesToShow: 6,
        slidesToScroll: 6,
        arrows: true,
        dots: true,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 4,
                    slidesToScroll: 4,
                    arrows: false
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    arrows: false
                }
            },
            {
                breakpoint: 540,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                    arrows: false
                }
            }
        ]
    });

    $('.partner-slider').slick({
        infinite: true,
        slidesToShow: 5,
        slidesToScroll: 5,
        arrows: true,
        dots: true,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    arrows: false
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2,
                    arrows: false
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
        const slider = $(".product-slider-main").on("init", function (slick) {
            $(".product-slider-main").fadeIn(1000);
            $(".slide-option:not(.slick-cloned)").each(function () {
                const index = $(this).data("slick-index");
                $(".plan > input.product-option-" + $(this).data("sku")).data('slick-index', index);
            });
        }).slick({
            infinite: true,
            slidesToShow: 1,
            slidesToScroll: 1,
            arrows: true,
            lazyLoad: "ondemand",
            autoplaySpeed: 3000,
            centerMode: false,
            asNavFor: ".product-slider-thmb"
        });

        const thumbnailsSlider = $(".product-slider-thmb").on("init", function (slick) {
            $(".product-slider-thmb").fadeIn(1000);
        }).slick({
            infinite: true,
            slidesToShow: 4,
            slidesToScroll: 1,
            lazyLoad: "ondemand",
            asNavFor: ".product-slider-main",
            dots: false,
            focusOnSelect: true
        });

        $(".product-slider-thmb .slick-slide").removeClass("slick-active");
        $(".product-slider-thmb .slick-slide").eq(0).addClass("slick-active");

        $(".product-slider-main").on("beforeChange", function (event, slick, currentSlide, nextSlide) {
            const mySlideNumber = nextSlide;
            $(".product-slider-thmb .slick-slide").removeClass("slick-active");
            $(".product-slider-thmb .slick-slide").eq(mySlideNumber).addClass("slick-active");
        });
    }

    $('input[name="skuId"]').change(function () {
        var hasImg = $(this).data("img");
        if (hasImg === "True") {
            const index = $('input[name="skuId"]:checked').data("slick-index");
            $('.product-slider-main').slick('slickGoTo', index);
        }
        return false;
    });

    $("#addToCart").submit(function (event) {
        event.preventDefault();

        const form = $(this);

        $.post("/ShoppingCart/AddToCart", form.serialize(), function (data) {
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
            skuId: $(this).val()
        }, function (data) {
            $(".product-sku").text(data.sku);
            $(".product-price .price").text(data.price);
            $(".product-price .price-old del").text(data.priceOld);
        });
    });

    // Function to populate districts based on selected city
    $("#cityDropdown").change(function () {
        const cityId = $(this).val();
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
        const districtId = $(this).val();
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
function BuyNow() {
    const form = $("#addToCart");
    $.post("/ShoppingCart/AddToCart", form.serialize(), function (data) {
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
        console.log(data);

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
function ListProductJS() {
    $(".by-sort").change(function () {
        const cate = $(".filter-by-category").val();
        const sort = $(".by-sort").val();

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
            ListProductJS();

            const currentURL = window.location.href;

            const urlObject = new URL(currentURL);

            const searchParams = urlObject.searchParams;

            searchParams.set("typeSort", sort);

            const newURL = urlObject.href;

            window.history.pushState({ path: newURL }, "", newURL);
        });
    });
}
function AllProductJS() {
    $(".by-sort").change(function () {
        const sort = $(".by-sort").val();

        $.ajax({
            url: "/Home/AllProductView",
            type: "GET",
            data: {
                typeSort: sort,
            }
        }).done(function (data) {
            $("#AllProductView").empty();
            $("#AllProductView").html(data);
            AllProductJS();
            const currentURL = window.location.href;

            const urlObject = new URL(currentURL);

            const searchParams = urlObject.searchParams;

            searchParams.set("typeSort", sort);

            const newURL = urlObject.href;

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
