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
                "searchable": false,
                "orderable": false
            }
        ],

        "columns": [
            {
                "data": "Username",
                "name": "Username",
                "autoWidth": true
            },
            {
                "data": "FullName",
                "name": "Name",
                "autoWidth": true
            },
            {
                "data": "RoleName",
                "name": "Role",
                "autoWidth": true
            },
            {
                "data": "IsActive",
                "name": "Active",
                "autoWidth": true
            },
            {
                "render": function (data, type, full, meta) {
                    return '<a href="/Admin/Manage/Account/View/' + full.Username + '"><i class="fa fa-edit"></i>View Detail & Edit</a>';
                }
            },
        ]
    })
});

//tạo script để load order