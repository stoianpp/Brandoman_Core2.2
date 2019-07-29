$(document).ready(function () {
    $(".nav a").on("click", function () {
        $(".nav").find(".active").removeClass("active");
        $(this).parent().addClass("active");
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
                if (typeof attr !== typeof undefined && attr !== false) {
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