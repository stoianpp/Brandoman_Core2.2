$(document).ready(function () {

    $(".nav a").on("click", function () {
        $(".nav").find(".active").removeClass("active");
        $(this).parent().addClass("active");
    });

        $('.btn-bootstrap-dialog').click(function () {
            var url = $(this).data('url');
            $.get(url, function (data) {
                $('#bootstrapDialog').html(data);
                $('#ModalPopUp').find('button, input').addClass('button-width');
                $('#bootstrapDialog').modal('show');
                $('#ModalPopUp').find('#myModalLabel').html($(this).attr("title"));
            });
        });

    $('.btn-bootstrap-dialog1').click(function () {
        var id = $(this).attr('data-id');
        $('#myModal1').modal({
        backdrop: 'static',
keyboard: false
})
        .on('click', '#confirmOk', function (e) {
        $.ajax({
            type: "POST",
            url: '@Url.Action("DeleteRecord", "Product")',
            data: '{id: ' + id + '}',
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (success) {
                $('#myModal1').modal('hide');
                window.location.reload();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert('oops, something bad happened')
            }
        })
    });
});

    $('#myModal2').on('hidden.bs.modal', function () {
        window.location.reload();
    });

    $('tbody').sortable({
        //items: 'tr:not(tr:first-child)',
        cursor: 'move',
axis: 'y',
dropOnEmpty: false,
        start: function (e, ui) {
        ui.item.addClass("selected");
    },
        stop: function (e, ui) {
        ui.item.removeClass("selected");
    var reordered = false;
    var orderList = [];
            $(this).find("tr").each(function (index) {
                var item = {
        ProductId: $(this).attr("data-productId"),
Order: index + 1
};
orderList.push(item);
$(this).attr("data-order", index + 1);
var attr = $(this).attr("style");
                if (typeof attr !== typeof undefined && attr !== false){
        reordered = true;
    }
});

            if (reordered) {
        $('#myModal2').modal({
            backdrop: 'static',
            keyboard: false
        })
            .on('click', '#confirmOrderOk', function (e) {
                e.preventDefault();
                $.ajax({
                    type: "POST",
                    url: '@Url.Action("SaveProductOrder", "Product")',
                    data: JSON.stringify(orderList),
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (success) {
                        $('#myModal2').modal('hide');
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert('oops, something bad happened')
                    }
                });
            })
    };
}
});
});