$(document).ready(function () {
    // Initialize Editor
    $('.textarea-editor').summernote({
        disableDragAndDrop: true,
        height: 500, // set editor height
        minHeight: null, // set minimum height of editor
        maxHeight: null, // set maximum height of editor
        focus: true, // set focus to editable area after initializing summernote
        toolbar: [
            // [groupName, [list of button]]
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['font', ['strikethrough', 'superscript', 'subscript']],
            ['fontsize', ['fontsize']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['height', ['height']]
        ]
    });
    $('.dropdown-toggle').dropdown();
    $('#summernote-disabled').summernote('disable');
});