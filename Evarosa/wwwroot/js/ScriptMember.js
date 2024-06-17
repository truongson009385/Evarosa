function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#imagePreview').css('background-image', 'url(' + e.target.result + ')');
            $('#imagePreview').hide();
            $('#imagePreview').fadeIn(650);
        }
        reader.readAsDataURL(input.files[0]);

        var formData = new FormData();
        formData.append('avatar', input.files[0]);

        $.ajax({
            url: '/member/upload-avatar',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    console.log('Avatar uploaded successfully!');
                } else {
                    console.log('Error uploading avatar.');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log('AJAX error: ' + textStatus + ' : ' + errorThrown);
            }
        });
    }
}

$("#imageUpload").change(function () {
    readURL(this);
});