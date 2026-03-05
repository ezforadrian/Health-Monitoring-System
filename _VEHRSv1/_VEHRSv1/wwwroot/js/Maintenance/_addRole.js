
function resetFields() {
   
    $("#fieldRoleName").val("");
    $("#fieldNormalizeName").val("");
}

$("#btnAddRole").on("click", function () {
    AddRoles();
});

$("#btnCancel").on("click", function () {
    resetFields();
});

var roleForDeletion = "";
$("#rolesTableBody").on("click", ".deleteRoles", function () {
    // Retrieve role information directly from the button's data attributes
    var roleName = $(this).data("name");
    var normalizedName = $(this).data("normalized");

    // Set the role information for deletion
    roleForDeletion = roleName;

    $('#modalConfirmDelete').modal('show');
    $("#fieldRoleNamemodal").val(roleName);
    // Optionally: handle the normalized name if needed
    // $("#fieldNormalizedNameModal").val(normalizedName);
});

$("#btnConfirmDelete").on("click", function () {

    deleteRole(roleForDeletion);
    roleForDeletion = "";
});

//GETROLE
var Popup, gTable;
$(document).ready(function () {

    gTable = $("#rolesTable").DataTable(
        {
            "paging": true,
            "select": true,

            // "dom": 'rtp',
            "processing": true,
            // "serverSide":true, // Enable server-side processing
            "pagingType": 'full_numbers',

            "ajax": {

                "url": "/Maintenance/GetRoleList",
                "type": "GET",
                "serverSide": true,
                "datatype": "json",
                "data": function (d) {
                    d.start = d.start || 0;
                    d.length = d.length || 100000000;

                    // Include search parameter only if it is not null or empty
                    if (d.search && d.search.value) {
                        d.search = d.search.value;
                    } else {
                        delete d.search; // Remove the search parameter if it's null or empty
                    }
                },
                "dataSrc": "data", // Assuming your data is wrapped in a "data" property
            },
            "columns": [
                { "data": "id" },
                { "data": "name" },
                { "data": "normalizedName" },

                {
                    "data": null, // Use null since we're customizing the output
                    "orderable": false,
                    "render": function (data) {
                        return `<input type='button' value='Delete' class='btn btn-danger deleteRoles' style='width:100px' data-name='${data.name}' data-normalized='${data.normalizedName}'>`;
                    }
                },

            ],

            "lengthChange": false, // Remove the show entries drop-down
            "pageLength": 10, // Set the initial page length
            "ordering": true, // Disable column sorting
            "language": {
                "paginate": {
                    "next": "Next",
                    "previous": "Previous"
                },
                "search": "Search:",
                "searchPlaceholder": "Search"
            }

        });

    $('.dataTables_filter').css({
        'float': 'right', // Align to the right
        'margin-right': '10px', // Add some right margin
        'margin-bottom': '10px'
    });

    $('.dataTables_filter input').css({
        'margin-left': '20px', // Add some right margin
        'width': '300px' // Set the width of the search input box
    });
    $('.dataTables_filter label input').attr('id', 'search');

});




function AddRoles() {
    var roleName = $("#fieldRoleName").val();
    var normalizedName = $("#fieldNormalizeName").val();
    if ($("#fieldRoleName").val() != "") {
        var newData = {
            Name: roleName,
            NormalizedName: normalizedName
        }
$.ajax({
    url: "/Maintenance/addRoles",
    type: "POST",
    contentType: "application/json",
    headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
    data: JSON.stringify(newData),
    success: function (data) {
        var ProcessStatus = data.processStatus;
        var ProcessMessage = data.processMessage;

        if (ProcessStatus == "1") {
            $("#txtSystemMessage").html(ProcessMessage);
            $('#modalSystemMessage').modal('show');
            resetFields();
            /*getRolesList();*/
            gTable.ajax.reload();
        } else if (ProcessStatus == "2") {
            $("#txtSystemMessage").html(ProcessMessage);
            $('#modalSystemMessage').modal('show');
        }
       
        else {
            $("#txtSystemMessage").html(ProcessMessage);
            $('#modalSystemMessage').modal('show');
        }

    }
});
    }
    else {
        $("#txtSystemMessage").html("Role name is required.");
    $('#modalSystemMessage').modal('show');
}
}



function deleteRole(roleName) {
    var newData = {
        Name: roleName // Ensure property name matches what your server expects
    };

    $.ajax({
        url: "/Maintenance/DeleteRoles",
        type: "POST",
        contentType: "application/json",
        headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        data: JSON.stringify(newData),
        success: function (data) {
            $("#txtSystemMessage").html(data);
            $('#modalSystemMessage').modal('show');

            resetFields();
            //getRolesList(); 
            gTable.ajax.reload();
        }
    });
}