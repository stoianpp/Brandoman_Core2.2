function confirmDialog(message, handler) {
    $(`<div class="modal fade" id="myModal" role="dialog"> 
    <div class="modal-dialog"> 
    <!-- Modal content--> 
    <div class="modal-content"> 
        <div class="modal-body" style="padding:10px;"> 
            <h4 class="text-center">${message}</h4> 
            <div class="text-center"> 
            <a class="btn btn-success btn-yes">Yes</a> 
            <a class="btn btn-default btn-no">No</a> 
            </div> 
        </div> 
    </div> 
</div> 
</div>`).appendTo('body');

    //Trigger the modal
    $("#myModal").modal({
        backdrop: 'static',
        keyboard: false
    });

    //Pass true to a callback function
    $(".btn-yes").click(function () {
        handler(true);
        $("#myModal").modal("hide");
    });

    //Pass false to callback function
    $(".btn-no").click(function () {
        handler(false);
        $("#myModal").modal("hide");
    });

    //Remove the modal once it is closed.
    $("#myModal").on('hidden.bs.modal', function () {
        $("#myModal").remove();
    });
}

$("#confirmbtn").click(function (e) {
    e.preventDefault();
    var formBase = $("form").has("#confirmbtn");

    confirmDialog("Save changes?", (ans) => {
        if (ans) {
            formBase.submit();
        } else {
            window.location.reload();
        }
    });
})

$(".delete-btn").click(function (e) {
    var id = $(this).attr("id").slice(0, -1);
    confirmDialog("Delete that record?", (ans) => {
        if (ans) {
            $.ajax({
                type: "POST",
                url: 'Product/DeleteRecord?id=' + id,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                crossDomain: true,
                success: function (data) {
                    window.location.href = data;
                }
            });
        } else {
            window.location.href = "/";
        }
    });
})