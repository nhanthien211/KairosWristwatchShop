$(document).ready(function () {
    $("#dataTable").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side          
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 10,
        "pagingType": "full_numbers",
        order: [[4, "desc"]],
        "ajax": {
            "url": "../LoadOrder",
            "type": "POST",
            "datatype": "json"
        },

        "columnDefs": [
            {
                "targets": [0],
                "orderable": false
            },
            {
                "targets": [1],
                "orderable": false
            },      
            {
                "targets": [2],
                "searchable": false,
                "orderable": false
            },
            {
                "targets": [3],
                "searchable": false,
                "orderable": false
            },
            {
                "targets": [4],
                "searchable": false                
            },
            {
                "targets": [5],
                "searchable": false,
                "orderable": false
            }
        ],

        "columns": [
            {
                "data": "OrderId",
                "name": "Order",
                "autoWidth": true
            },
            {
                "data": "Customer",
                "name": "Customer",
                "autoWidth": true
            },
            {
                "data": "Date",
                "name": "Order Date",
                "autoWidth": true
            },
            {
                "data": "Total",
                "name": "Total ($)",
                "autoWidth": true
            },
            {
                "data": "Status",
                "name": "Status",
                "autoWidth": true
            },
            {
                "render": function (data, type, full, meta) {
                    return '<a href="/Admin/Manage/Order/View/' + full.OrderId + '"><i class="fa fa-edit"></i>View Detail & Edit</a>';
                }
            },
        ]
    })
});

//tạo script để load order