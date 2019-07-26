$(document).ready(function () {
    $('form').each(function () {
        if ($(this).data('validator'))
            $(this).data('validator').settings.ignore = ".note-editor *";
    });

    $('#close-button').click(function () {
        location.href = "/";
    });
    $('#confirmok1').click(function (e) {
        var std = $(this).attr('id');
        $('#myModal').modal({
            backdrop: 'static',
            keyboard: false
        })
        $.ajax({
            type: "POST",
            url: '@Url.Action("Product/AddEditRecord")',
            data: '{Id: ' + JSON.stringify(std) + '}',
            dataType: "json",
            contentType: "application/json; charset=utf-8",
        });
    });
});
$(document).ready(function () {
    // Initialize Editor
    $('.textarea-editor').summernote({
        disableDragAndDrop: true,
        height: 300, // set editor height
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
    $('.dropdown-toggle').dropdown()
});