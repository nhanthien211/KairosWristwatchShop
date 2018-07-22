$(document).ready(function () {

});

/*[add product ]
    ===========================================================*/
function addToCartAjax(id, name) {
    $.ajax({
        type: "POST",
        url: '../Shopping/AddToCart',
        data: {
            "id": id
        },
        success: function (response) {
            if (response.success) {
                swal(name, "is added to cart !", "success");
            } else {
                if (response.quantity == -1) {
                    swal("", "Can not update your cart! Somthing when wrong! Plaese try again later", "");
                } else {
                    swal(name, "can not be added to cart anymore! Available in Stock: " + response.quantity, "");
                }                
            }
        },
        error: function (response) {
            swal(name, "is NOT added to cart !", "fail");
        }
    });
};

function addToCartAjaxForShop(id, name) {
    $.ajax({
        type: "POST",
        url: '../../Shopping/AddToCart',
        data: {
            "id": id
        },
        success: function (response) {
            if (response.success) {
                swal(name, "is added to cart !", "success");
            } else {
                if (response.quantity == -1) {
                    swal("", "Can not update your cart! Somthing when wrong! Plaese try again later", "");
                } else {
                    swal(name, "can not be added to cart anymore! Available in Stock: " + response.quantity, "");
                }
            }
        },
        error: function (response) {
            swal(name, "is NOT added to cart !", "fail");
        }
    });
};

/*[ remove product ]
    ===========================================================*/
function removeItemAjax(id) {
    $.ajax({
        type: "POST",
        url: '../Shopping/RemoveItem',
        data: {
            "id": id
        },
        success: function (response) {
            if (response.success) {
                if (response.empty) {
                    window.location.href = window.location.href;
                } else {
                    $('#row-' + id).remove();
                    $('#cart-total').html('$ ' + response.responseText);
                    $('#message').css("color", "green");
                    $('#message').html('Remove item successfully!');
                }                
            } else {
                $('#message').css("color", "red");
                $('#message').html('Remove item fail! Please try again later!');
            }
        },
        error: function (response) {
        }
    });
}

/*[ update product ]
    ===========================================================*/
function updateItemAjax(id, quantityNeed) {
    $.ajax({
        type: "POST",
        url: '../Shopping/UpdateItem',
        data: {
            "id": id,
            "quantityNeed": quantityNeed
        },
        success: function (response) {
            if (response.success) {
                $('#cart-total').html('$ ' + response.responseText);
                $('#message-' + id).css("color", "green");
                $('#message-' + id).html('Update item quantity successfully!');
                $("#message-" + id).show();
                $("#message-" + id).delay(2000).fadeOut('slow');
                $('#text-' + id).val(quantityNeed);
                $('#sub-total-' + id).html('$ ' + response.subTotal);
            } else {
                if (response.quantity == -1) {
                    $('#message-' + id).css("color", "red");
                    $('#message-' + id).html('Update item quantity fail! Try again later');
                    $("#message-" + id).show();
                    $("#message-" + id).delay(2000).fadeOut('slow');
                } else {
                    updateItemAjaxForInputEmpty(id, response.quantity);
                }
            }
        },
        error: function (response) {
        }
    });
}

function updateItemAjaxForInputOutRange(id, quantityNeed) {
    $.ajax({
        type: "POST",
        url: '../Shopping/UpdateItem',
        data: {
            "id": id,
            "quantityNeed": quantityNeed
        },
        success: function (response) {
            if (response.success) {
                $('#cart-total').html('$ ' + response.responseText);
                $('#message-' + id).css("color", "red");
                $('#message-' + id).html('Invalid input! Set the quantity to default');
                $("#message-" + id).show();
                $("#message-" + id).delay(2000).fadeOut('slow');
                $('#text-' + id).val(quantityNeed);
                $('#sub-total-' + id).html('$ ' + response.subTotal);
            } else {
                if (response.quantity == -1) {
                    $('#message-' + id).css("color", "red");
                    $('#message-' + id).html('Update item quantity fail! Try again later');
                    $("#message-" + id).show();
                    $("#message-" + id).delay(2000).fadeOut('slow');
                } else {
                    $('#message-' + id).css("color", "red");
                    $('#message-' + id).html('Available in stock: ' + response.quantity);
                    $("#message-" + id).show();
                    $("#message-" + id).delay(2000).fadeOut('slow');
                }
            }
        },
        error: function (response) {
        }
    });
}

function updateItemAjaxForInputEmpty(id, quantityNeed) {
    $.ajax({
        type: "POST",
        url: '../Shopping/UpdateItem',
        data: {
            "id": id,
            "quantityNeed": quantityNeed
        },
        success: function (response) {
            if (response.success) {
                $('#cart-total').html('$ ' + response.responseText);
                $('#message-' + id).css("color", "red");
                $('#message-' + id).html('Available in stock: ' + quantityNeed);
                $("#message-" + id).show();
                $("#message-" + id).delay(2000).fadeOut('slow');
                $('#text-' + id).val(quantityNeed);
                $('#sub-total-' + id).html('$ ' + response.subTotal);
            } else {
                if (response.quantity == -1) {
                    $('#message-' + id).css("color", "red");
                    $('#message-' + id).html('Update item quantity fail! Try again later');
                    $("#message-" + id).show();
                    $("#message-" + id).delay(2000).fadeOut('slow');
                } else if (response.quantity == 0) {
                    $('#message-' + id).css("color", "red");
                    $('#message-' + id).html('Sorry! This product is out of stock! Please remove it from your cart!');
                    $("#message-" + id).show();
                }
                else {
                    $('#message-' + id).css("color", "red");
                    $('#message-' + id).html('Available in stock: ' + response.quantity);
                    $("#message-" + id).show();
                    $("#message-" + id).delay(2000).fadeOut('slow');

                }
            }
        },
        error: function (response) {
        }
    });
}

function updateItemAjaxForInputOutNormal(id, quantityNeed) {
    $.ajax({
        type: "POST",
        url: '../Shopping/UpdateItem',
        data: {
            "id": id,
            "quantityNeed": quantityNeed
        },
        success: function (response) {
            if (response.success) {
                $('#cart-total').html('$ ' + response.responseText);
                $('#message-' + id).css("color", "green");
                $('#message-' + id).html('Update item quantity successfully');
                $("#message-" + id).show();
                $("#message-" + id).delay(2000).fadeOut('slow');
                $('#text-' + id).val(quantityNeed);
                $('#sub-total-' + id).html('$ ' + response.subTotal);
            } else {
                if (response.quantity == -1) {
                    $('#message-' + id).css("color", "red");
                    $('#message-' + id).html('Update item quantity fail! Try again later');
                    $("#message-" + id).show();
                    $("#message-" + id).delay(2000).fadeOut('slow');
                } else {
                    var tmp = String(response.quantity)
                    updateItemAjaxForInputEmpty(id, tmp);
                }
            }
        },
        error: function (response) {
        }
    });
}

/*[ +/- num product ]
    ===========================================================*/
$('.btn-num-product-up').on('click', function (e) {
    var numProduct = Number($(this).prev().val());
    var quantityNeed = String(numProduct + 1);

    var id = $(this).closest('div').find('#cur-id').val();

    if (numProduct < 10) {
        //updateItemAjax(id, quantityNeed);
        updateItemAjax(id, quantityNeed);
    }
    e.preventDefault();
});

$('.btn-num-product-down').on('click', function (e) {
    var id = $(this).closest('div').find('#cur-id').val();
    var numProduct = Number($('#text-' + id).val());
    var quantityNeed = String(numProduct - 1);    

    if (numProduct > 1) {
        //updateItemAjax(id, quantityNeed);
        updateItemAjax(id, quantityNeed);
    }
    e.preventDefault();
});

$('.num-product').on('blur', function (e) {
    var id = $(this).closest('div').find('#cur-id').val();
    var numProduct = Number($(this).val());
    var quantityNeed = String(numProduct);

    if (numProduct > 10) {
        updateItemAjaxForInputOutRange(id, "10");
    } else if (numProduct < 1) {
        updateItemAjaxForInputOutRange(id, "1");
    } else {
        updateItemAjaxForInputOutNormal(id, quantityNeed);
    }    
});

/*[ Proceed to checkout ]
    ===========================================================*/
var checkNoError = false;

//WIP ----------------------------------------
$('#proceed-to-checkouta').on('click', function (e) {
    if (checkNoError) {
        checkNoError = false;
        return; //Proceed action when cart is valid
    }

    e.preventDefault(); //Prevent action when cart is invalid

});