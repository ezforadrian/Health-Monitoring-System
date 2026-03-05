//Input Trigger -- Works on Paste AND KeyPress
$("#fieldUserId").on("input", function (e) {
    var object = $(this);
    var objectVal = object.val();
    var cleanText = objectVal.replace(/((?![0-9-]).)+/g, "");
    this.value = cleanText.substring(0, 7);
});

//keyPress Display of Employee Details
$("#fieldUserId").on("keyup", function () {
        getUserInfo(this.value);
});

$("#btnCancelMode").on("click", function () {
    resetFields();
});

$("#btnEnrollUser").on("click", function () {
    enrollUser();
});

var userForDeletion = "";
var Name = "";
var JTitle = "";
$("#usersTableBody").on("click", ".deleteUser", function () {
  
    // Get the closest row to the delete button
    //var $row = $(this).closest("tr");

    // Retrieve user information from the inputs in the row
    //var UserID = $row.find("input.userID").val();
    //var LastName = $row.find("input.lastName").val(); // Changed to lastName
    //var FirstName = $row.find("input.firstName").val(); // Changed to firstName
    //var Mi = $row.find("input.MI").val();
    //var Title = $row.find("input.position").val();

    var UserID = $(this).data("userid");
    var LastName = $(this).data("lastname");; // Changed to lastName
    var FirstName = $(this).data("firstname"); // Changed to firstName
    var Mi = $(this).data("middlename");
    var Title = $(this).data("position");

    // Set the user information for deletion
    userForDeletion = UserID;
    Name = LastName + ', ' + FirstName + ' ' + Mi + '.'; // Combine names correctly
    JTitle = Title;
    $('#modalConfirmDelete').modal('show');

    $("#fieldUserIdmodal").val(UserID);
    $("#fieldNameIdmodal").val(Name);
    $("#fieldTilemodal").val(JTitle);
    //deleteUser(UserID);
    //$("#btnCancelEditMode").trigger("click");
});

$("#btnConfirmDelete").on("click", function () {
   
    deleteUser(userForDeletion);
    userForDeletion = "";
});

$("#btnUpdateUser").on("click", function () {
    
    $('#modalConfirmUpdate').modal('show');
    //updateUser();
    //resetFields();
});
$("#btnConfirmUpdate").on("click", function () {  

    updateUser();
    resetFields();
});



$("#usersTableBody").on("click", ".updateUser", function () {
    //var UserName = $(this).next("input[class='userName']").val();
    var UserNames = $(this).data("username");
    $("#btnEnrollUser").hide();
    $("#btnUpdateUser").show();
    getUserInfo_System(UserNames);
    $("#fieldUserId").prop("disabled", true);
   
});


function resetFields() {
    $("#btnEnrollUser").show();
    $("#fieldUserId").prop("disabled", false);
    $("#btnUpdateUser").hide();
    $("#fieldUserId").val("");
    $("#fieldFirstName").val("");
    $("#fieldLastName").val("");
    $("#fieldMiddleInitial").val("");
    $("#fieldDepartment").val("");
    $("#fieldJobTitle").val("");
}


//GET USER
var Popup, gTable;
$(document).ready(function () {

    gTable = $("#usersTable").DataTable(
        {
            "paging": true,
            "select": true,

            //"dom": 'rtp',
            "processing": true,
            // "serverSide":true, // Enable server-side processing
            "pagingType": 'full_numbers',

            "ajax": {

                "url": "GetUsersList",
                "type": "POST",
                "serverSide": true,
                "datatype": "json",
                headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
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
                { "data": "userName" },
                { "data": "firstName" },
                { "data": "lastName" },
                { "data": "middleName" },
                { "data": "position" },
                { "data": "department" },
                { "data": "userRole" },


                {
                    "data": null, // Use null since we're customizing the output
                    "orderable": false,
                    "render": function (data) {
                        return `<input type='button' value='Edit' class='btn btn-primary updateUser' style='width:100px' data-userName='${data.userName}'>`;
                    }
                },
                {
                    "data": null, // Use null since we're customizing the output
                    "orderable": false,
                    "render": function (data) {
                        return `<input type='button' value='Delete' class='btn btn-danger deleteUser' style='width:100px' 
                        data-userId='${data.userName}'
                        data-firstName='${data.firstName}'
                        data-lastName='${data.lastName}'
                        data-middleName='${data.middleName}'
                        data-position='${data.position}'
                        data-department='${data.department}'
                        data-userRole='${data.userRole}'
                        >`;
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
        'margin-bottom': '30px'
    });

    $('.dataTables_filter input').css({
        'margin-left': '20px', // Add some right margin
        'width': '500px' // Set the width of the search input box
    });
    $('.dataTables_filter label input').attr('id', 'search');

});




//Used for AD Users when inputting UserName
function getUserInfo(userId) {
    var UserId = userId;
    $.ajax({
        url: "GetEmployeeInfo",
        method: "post",
        type: "json",
        headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        data: { UserId },
        success: function (data) {
            if (data.userName != "") {
                $("#fieldFirstName").val(data.firstName);
                $("#fieldLastName").val(data.lastName);
                $("#fieldMiddleInitial").val(data.middleName);
                $("#fieldDepartment").val(data.department);
                $("#fieldJobTitle").val(data.payClass);
                $("#fieldEMail").val(data.email);
                $("#fieldPassword").val("");
            }
            else {
                $("#fieldFirstName").val("");
                $("#fieldLastName").val("");
                $("#fieldMiddleInitial").val("");
                $("#fieldDepartment").val("");
                $("#fieldJobTitle").val("");
                $("#fieldEMail").val("");
                $("#fieldPassword").val("");
            }
        }
    });
}

function enrollUser() {
    if ($("#fieldUserName").val() != "") {
        var newData = {
            UserName: $("#fieldUserId").val(),
            FirstName: $("#fieldFirstName").val(),
            LastName: $("#fieldLastName").val(),
            MiddleName: $("#fieldMiddleInitial").val(),
            Department: $("#fieldDepartment").val(),
            PayClass: $("#fieldJobTitle").val(),
            EMail: $("#fieldEMail").val(),
            Password: $("#fieldPassword").val(),
            UserRole: $("#fieldUserType").val()
        }

        $.ajax({
            url: "EnrollNewUser",
            type: "POST",
            contentType: "application/json",
            headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
            data: JSON.stringify(newData),
            success: function (data) {
                var ProcessStatus = data.processStatus;
                var ProcessMessage = data.processMessage;

                if (ProcessStatus == "1") {
                    $("#txtSystemMessage").html("User Enrolled");
                    $('#modalSystemMessage').modal('show');
                    resetFields();
                    //getUsersList();
                    gTable.ajax.reload();
                }
                else if (ProcessStatus == "2") {
                    $("#txtSystemMessage").html("User Enrollment Failed. Invalid PAGCOR ID");
                    $('#modalSystemMessage').modal('show');
                }
                else if (ProcessStatus == "3") {
                    $("#txtSystemMessage").html("User Enrollment Failed. UserName Field Cannot be blank");
                    $('#modalSystemMessage').modal('show');
                }
                else if (ProcessStatus == "4") {
                    $("#txtSystemMessage").html("User Enrollment Failed. An Input Field has invalid format<br><br><span style='font-size:15px;color:red;'>" + ProcessMessage);
                    $('#modalSystemMessage').modal('show');
                }
                else if (ProcessStatus == "5") {
                    $("#txtSystemMessage").html("User Enrollment Failed. Input Field cannot be blank/empty");
                    $('#modalSystemMessage').modal('show');
                }
                else if (ProcessStatus == "6") {
                    $("#txtSystemMessage").html("User Enrollment Failed. UserName already exists");
                    $('#modalSystemMessage').modal('show');
                }
                else {
                    $("#txtSystemMessage").html("User Enrollment Failed. Unknown Error");
                    $('#modalSystemMessage').modal('show');
                }

            }
        });
    }
    else {
        $("#txtSystemMessage").html("User Enrollment Failed. UserName Field Cannot be blank");
        $('#modalSystemMessage').modal('show');
    }
}

function deleteUser(userID) {
    var newData = {
        UserName: userID
    }
    $.ajax({
        url: "DeleteUser",
        type: "POST",
        contentType: "application/json",
        headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        data: JSON.stringify(newData),
        success: function (data) {
            //alert(data);
            $("#txtSystemMessage").html(data);
            $('#modalSystemMessage').modal('show');

            resetFields();
            gTable.ajax.reload();
            //getUsersList();
        }
    });
}

function updateUser() {
    if ($("#fieldUserName").val() != "") {
        var newData = {
            UserName: $("#fieldUserId").val(),
            FirstName: $("#fieldFirstName").val(),
            LastName: $("#fieldLastName").val(),
            MiddleName: $("#fieldMiddleInitial").val(),
            Department: $("#fieldDepartment").val(),
            PayClass: $("#fieldJobTitle").val(),
            UserRole: $("#fieldUserType").val()
        }

        $.ajax({
            url: "UpdateUser",
            type: "POST",
            contentType: "application/json",
            headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
            data: JSON.stringify(newData),
            success: function (data) {
                //alert(data);
                $("#txtSystemMessage").html(data);
                $('#modalSystemMessage').modal('show');

                //$("#btnCancelEditMode").trigger("click");
                //getUsersList();
                gTable.ajax.reload();
            }
        });
    }
    else {
        $("#txtSystemMessage").html("User Enrollment Failed. Invalid UserName");
        $('#modalSystemMessage').modal('show');
    }
}

//Used for System Users when editing
function getUserInfo_System(UserName) {
    $.ajax({
        url: "GetUserInfo",
        method: "post",
        type: "json",
        headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        data: { UserName },
        success: function (data) {
            if (data.userName != "") {
                $("#fieldUserType").val(data.userRole);
                $("#fieldUserType").trigger("change");
                $("#fieldUserId").val(data.userName);
                $("#fieldFirstName").val(data.firstName);
                $("#fieldLastName").val(data.lastName);
                $("#fieldMiddleInitial").val(data.middleName);
                $("#fieldDepartment").val(data.department);
                $("#fieldJobTitle").val(data.payClass);
            }
            else {
                $("#fieldUserType").val("");
                $("#fieldUserName").val("");
                $("#fieldFirstName").val("");
                $("#fieldLastName").val("");
                $("#fieldMiddleInitial").val("");
                $("#fieldDepartment").val("");
                $("#fieldJobTitle").val("");
            }
        }
    });
}

