/**
 * @license Copyright (c) 2003-2021, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';

    config.removeDialogTabs = "image:advanced;link:advanced";
    config.autoGrow_maxHeight = 400;
    config.allowedContent = true;

    config.filebrowserBrowseUrl = '/ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = '/ckfinder/ckfinder.html?type=Images';
    config.filebrowserUploadUrl = '/ckfinder/connector?command=QuickUpload&type=Files';
    config.filebrowserImageUploadUrl = '/ckfinder/connector?command=QuickUpload&type=Images';
    
    config.extraPlugins = "btgrid,videoembed,lineheight";
};
