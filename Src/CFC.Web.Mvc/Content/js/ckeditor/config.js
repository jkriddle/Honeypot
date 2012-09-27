/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function (config) {
    config.toolbar = 'CFCToolbar';

    config.toolbar_CFCToolbar =
	[
		{ name: 'document', items: ['Preview','Maximize'] },
		{ name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
		{ name: 'insert', items: ['Image', 'Table', 'HorizontalRule'] },
		{ name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
       // '/',
		{ name: 'styles', items: ['Styles', 'Format'] },
		{ name: 'basicstyles', items: ['Bold', 'Italic', 'Strike', '-', 'RemoveFormat'] },
		{ name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote'] },
	    { name: 'document', items: ['Source'] }
	];
};
