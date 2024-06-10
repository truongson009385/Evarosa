var leftMenuProfile = $.cookie("left_menu_profile");
if (leftMenuProfile == null) leftMenuProfile = "";
var arrMenu = leftMenuProfile.split('|');
for (var i = 0; i < arrMenu.length; i++) {
    $("#left_menu_profile li[data-id='" + arrMenu[i] + "'] a.root").addClass("expand");
    $("#left_menu_profile li[data-id='" + arrMenu[i] + "'] div").show();
}
$(".left_menu_profile li .root").click(function () {
    $(this).parent().find(".sub").slideToggle(400);
    $(this).parent().find(".root").toggleClass("expand");
    var strTemp = $(this).parent().attr("data-id") + "|";
    leftMenuProfile = leftMenuProfile.replace(strTemp, "");
    if ($(this).hasClass("expand")) {
        leftMenuProfile = leftMenuProfile + strTemp;
    }
    $.cookie("left_menu_profile", leftMenuProfile, {
        path: "/"
    });
});

$("#AlertBox").removeClass('hide');
$("#AlertBox").delay(5000).slideUp(500);

function myFunction() {
    var x = document.getElementById("responsive");
    if (x.className === "") {
        x.className += " responsive";
    } else {
        x.className = "";
    }
}

function fbFunction() {
    var x = document.getElementById("form");
    if (x.className === "") {
        x.className += " show-form";
    } else {
        x.className = "";
    }
}

function confirmation(title, content, func) {
    $.confirm({
        title,
        content,
        buttons: {
            confirm: {
                text: 'Xác nhận',
                action: function () {
                    func();
                }
            },
            cancel: {
                text: 'Thoát'
            }
        }
    });
}

const dropContainer = $("#dropcontainer");
const fileInput = $("#images");

dropContainer.on("dragover", function (e) {
    e.preventDefault();
});

dropContainer.on("dragenter", function () {
    dropContainer.addClass("drag-active");
});

dropContainer.on("dragleave", function () {
    dropContainer.removeClass("drag-active");
});

dropContainer.on("drop", function (e) {
    e.preventDefault();
    dropContainer.removeClass("drag-active");
    fileInput[0].files = e.originalEvent.dataTransfer.files;
});

$('.select2').select2();
$(".datepicker").datepicker();
//$('.summernote').customSummernote();

$("textarea.ckeditor").ckeditor();
CKEDITOR.timestamp = new Date();