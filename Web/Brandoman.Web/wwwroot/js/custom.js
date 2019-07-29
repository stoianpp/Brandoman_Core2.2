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
                    Id: $(this).attr("data-productId"),
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
                    confirmDialog("Do you want to save the new order?", (ans) => {
                        if (ans) {
                            e.preventDefault();
                            $.ajax({
                                type: "POST",
                                url: 'Product/SaveProductOrder',
                                data: JSON.stringify(orderList),
                                dataType: "json",
                                contentType: "application/json; charset=utf-8",
                                success: function (data) {
                                    window.location.href = data;
                                }
                            });
                        } else {
                            window.location.href = "/";
                        }
                })
            };
        }
    });
});  