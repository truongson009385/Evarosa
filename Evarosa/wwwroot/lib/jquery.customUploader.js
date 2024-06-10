(function ($) {
    var inputUpdate = (input, uuid, remove) => {
        let currentValue = (input.val() || '').trim();

        let values = currentValue ? currentValue.split(',') : [];

        if (remove) {
            values = values.filter(val => val !== uuid);
        } else {
            if (!values.includes(uuid)) {
                values.push(uuid);
            }
        }

        input.val(values.join(','));
    };


    $.fn.customUploader = function (options) {
        this.each(function () {
            const ele = $(this);
            const defaults = {
                folder: "project",
                template: "qq-template",
                limit: 1,
                debug: false
            };

            const settings = $.extend({}, defaults, options);
            const dataAttributes = ele.data();
            const mergedOptions = $.extend({}, settings, dataAttributes);

            const {
                folder,
                uploadUrl,
                deleteUrl,
                extens,
                template,
                limit,
                init,
                input
            } = mergedOptions;

            const inputElement = $(input);

            ele.fineUploader({
                template,
                request: { endpoint: uploadUrl || `/uploads/${folder}` },
                deleteFile: { enabled: true, forceConfirm: true, endpoint: deleteUrl || `/delete/${folder}` },
                validation: { allowedExtensions: extens.split(',').map(item => item.trim()), itemLimit: limit },
                autoUpload: true,
                debug: false,
                callbacks: {
                    onComplete: (id, name, responseJSON) => {
                        ele.fineUploader('setName', id, responseJSON.files[0]);
                        inputUpdate(inputElement, responseJSON.files[0], false);
                    },
                    onDelete: (id) => inputUpdate(inputElement, ele.fineUploader('getName', id), true)
                }
            });

            //sortable
            const uploadList = ele.find(".qq-upload-list");
            uploadList.sortable({
                items: "li",
                cursor: "grab",
                update: () => {
                    const files = ele.fineUploader('getUploads');
                    const arrName = uploadList.children("li").map((_, el) => files.find(obj => obj.id == $(el).attr("qq-file-id")).name).get();

                    inputElement.val(arrName.join(","));
                }
            }).disableSelection();

            if (init) ele.fineUploader('addInitialFiles', JSON.parse(init));
        });
    };
})(jQuery);
