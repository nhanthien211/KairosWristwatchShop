$(document).ready(function () {
    $("#dataTable").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side          
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 10,
        "pagingType": "full_numbers",
        order: [[3, "desc"]],
        "ajax": {
            "url": "../LoadWatch",
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
                "searchable": false,
                "orderable": false
            },
            {
                "targets": [2],
                "searchable": false,
                "orderable": false
            },
            {
                "targets": [3],
                "searchable": false                
            }
        ],

        "columns": [
            {
                "data": "WatchCode",
                "name": "Watch Code",
                "autoWidth": true
            },
            {
                "data": "Quantity",
                "name": "Quantity",
                "autoWidth": true
            },
            {
                "data": "Price",
                "name": "Price ($)",
                "autoWidth": true
            },
            {
                "data": "Status",
                "name": "For Sale",
                "autoWidth": true
            },
            {
                "render": function (data, type, full, meta) {
                    return '<a href="/Admin/Manage/Watch/View/' + full.WatchID + '"><i class="fa fa-edit"></i>View Detail & Edit</a>';
                }
            },
        ]
    })
});