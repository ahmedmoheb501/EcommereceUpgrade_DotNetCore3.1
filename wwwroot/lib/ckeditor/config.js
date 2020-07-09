/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function( config )
{
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.width = '600px';
    //var roxyFileman = '/fileman/index.html?integration=ckeditor';
    config.extraPlugins = 'filebrowser';
    //config.filebrowserBrowseUrl = roxyFileman;
    //config.filebrowserImageBrowseUrl = roxyFileman;// + '&type=image';
    config.filebrowserBrowseUrl= '/ckfinder/ckfinder.html';
    config.filebrowserUploadUrl= '/ckfinder/core/connector/php/connector.php?command=QuickUpload&type=Files';

    config.removeDialogTabs = 'link:upload;image:upload';
};
