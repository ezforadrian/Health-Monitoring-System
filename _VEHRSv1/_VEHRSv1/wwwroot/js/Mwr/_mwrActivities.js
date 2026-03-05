
$("#btnAdd").on("click", function () {
    AddActivity();
    resetFields();
});
$("#btnAddDate").on("click", function () {
    AddActivityDate();
    resetFieldsDate();
});


$("#btnAddParticipant").on("click", function () {
    AddMwrParticipant();
});
$("#btnCanceDatelModal").on("click", function () {
    
    resetFieldsDate();
});

function resetFieldsDate() {
    $('#fieldMwrDate').val('');
}

$("#btnConfirmMwrDelete").on("click", function () {
    deleteMWR();
});

$("#btnUpdateMwr").on("click", function () {

    $('#modalConfirmUpdate').modal('show');
});

$("#fieldParticipantID").on("keyup", function () {
    getUserInfo(this.value);


});
$('#modalAddMwrDate').on('hidden.bs.modal', function () {
    $('#MwrActivityDate').DataTable().destroy();

});


$("#btnConfirmUpdate").on("click", function () {

    updateMwr();
    resetFields();
});
$("#btnCancelModal").on("click", function () {
    resetFields();
});


$("#btnCancelMode").on("click", function () {
    resetFields();
});

$("#MwrTableBody").on("click", ".UpdateMWR", function () {
   
    var MwrID = $(this).data("id");
    var Name = $(this).data("actname");
    var ActivityTYPE = $(this).data("acttype"); 
    $("#fieldActivityID").val(MwrID);
    $("#fieldActivityName").val(Name);
    $("#fieldActivity").val(ActivityTYPE);
    $("#btnUpdateMwr").prop("hidden", false);
    $("#btnAdd").prop("hidden", true);

});

var Popup2, gTable2;
$("#MwrTableBody").on("click", ".mwrDate", function () {
    
    var MwrID = $(this).data("id");
    var Name = $(this).data("actname");
    var ActivityTYPE = $(this).data("acttype");
    $('#modalAddMwrDate').modal('show');
    
    $("#fieldActIdDate").val(MwrID);
    $("#fieldActNameDate").val(Name);
    $("#fieldActTypeDate").val(ActivityTYPE);
    
    //GETACTIVITYDATE
    $(document).ready(function () {
        gTable2 = $("#MwrActivityDate").DataTable(
            {
                "paging": true,
                "select":
                {
                    style: 'os',
                    info: false
                },

                // "dom": 'rtp',
                "processing": true,
                "pagingType": 'full_numbers',
                "ajax": {

                    "url": "/MWR/GetMwrlistDate",
                    "type": "GET",
                    "serverSide": true,
                    "datatype": "json",
                    "data": function (d) {
                        d.mwrlistId = MwrID;
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


                    {
                        data: 'mwractDate', render: function (data) {
                            // Convert the date string into a Date object
                            const date = new Date(data);
                            // Format the date as desired (e.g., 'MM/DD/YYYY')
                            return date.toLocaleDateString('en-US', {
                                year: 'numeric',
                                month: '2-digit',
                                day: '2-digit'
                            });; // or use any format you prefer
                        }
                    },
                    {
                        "data": null, // Use null since we're customizing the output
                        "orderable": false,
                        "render": function (data) {
                            return `<input type='button' value='Delete' class='btn btn-danger mwrDateDelete' style='width:auto'
                        data-id='${data.mwrlistId}'
                        data-actdate='${data.mwractDate}'
                        >`;
                        }
                    },
                    

                ],

                "lengthChange": false, // Remove the show entries drop-down
                "pageLength": 3, // Set the initial page length
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


});



var MwrForDeletion = "";
$("#MwrTableBody").on("click", ".DeleteMWR", function () {
    // Retrieve role information directly from the button's data attributes
    var IDMwr = $(this).data("id");
    var Name = $(this).data("actname");
    var ActivityTYPE = $(this).data("acttype"); 
    // Set the role information for deletion
    MwrForDeletion = IDMwr;

    $('#modalConfirmDeletemwr').modal('show');
    $("#fieldMwrNamemodal").val(Name);
   
});
$("#btnConfirmDelete").on("click", function () {

    deleteMWR(MwrForDeletion);
    MwrForDeletion = "";
});

var MwrIdDateForDeletion = "";
var MwrDateForDeletion = "";
$("#MwrAddDateTableBody").on("click", ".mwrDateDelete", function () {
    // Retrieve role information directly from the button's data attributes
    var IDMwr = $(this).data("id");
    var actiDate = $(this).data("actdate");
    var ActivityTYPE = $(this).data("acttype");
    var date = new Date(actiDate);
    var formattedDate = date.toLocaleDateString('en-US');
    // Set the role information for deletion
    MwrIdDateForDeletion = IDMwr;
    MwrDateForDeletion = actiDate;
    $('#modalConfirmDeletemwrDate').modal('show');
    
    $("#fieldMwrNameDatemodal").val(formattedDate);

});

$("#btnConfirmDateDelete").on("click", function () {

    deleteMWRDate(MwrIdDateForDeletion,MwrDateForDeletion);
    MwrDateForDeletion = "";
    MwrIdDateForDeletion = "";
});

function resetFields() {$
    const dropdown = document.getElementById("fieldActivity");
    dropdown.selectedIndex = 0;
    $("#fieldParticipantID").val("");
    $("#fieldLastName").val("");
    $("#fieldFirstName").val("");
    $("#fieldMiddleInitial").val("");
    $("#fieldDepartment").val("");
    $("#fieldJobTitle").val("");
    $("#fieldActivityName").val("");
    $("#btnUpdateMwr").prop("hidden", true);
    $("#btnAdd").prop("hidden", false);
}



function updateMwr() {
    if ($("#fieldActivityName").val() != "") {
        var newData = {
           mwrlistId: $("#fieldActivityID").val(),
           activityName : $("#fieldActivityName").val(),
           activityType : $("#fieldActivity").val(),
           
        }

        $.ajax({
            url: "/MWR/UpdateMwr",
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
        $("#txtSystemMessage").html("Invalid HSWD Activity");
        $('#modalSystemMessage').modal('show');
    }
}



function updateFields() {
    $("#fieldModifyActivity").prop("disabled", false);
    $("#fieldModifyActivityType").prop("disabled", false);
    $("#fieldModifyActivityTypeDis").hide();
    $("#fieldModifyActivityType").prop("hidden", false);
    $("#btnMWRDelete").prop("hidden", true);
    $("#btnMWRUpdate").prop("hidden", true);
    $("#btnMWRUpdateYes").prop("hidden", false);
    $("#btnMWRCancel").prop("hidden", false);
    
}



function deleteMWR(IDMwr) {
    var newData = {
        mwrlistId: IDMwr
    };

    $.ajax({
        url: "/MWR/DeleteMwr",
        type: "POST",
        contentType: "application/json",
        headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        data: JSON.stringify(newData),
        success: function (data) {
            $("#txtSystemMessage").html(data);
            $('#modalSystemMessage').modal('show');

            //resetFields();
            //getRolesList(); 
            gTable.ajax.reload();
        }
    });
}

function deleteMWRDate(id, actiDate) {
    var newData = {
        mwrlistId: id,
        mwractDate: actiDate,
    };

    $.ajax({
        url: "/MWR/DeleteMwrDate",
        type: "POST",
        contentType: "application/json",
        headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        data: JSON.stringify(newData),
        success: function (data) {
            $("#txtSystemMessage").html(data);
            $('#modalSystemMessage').modal('show');

            //resetFields();
            //getRolesList(); 
            gTable2.ajax.reload();
        }
    });
}



//GETACTIVITY
var Popup, gTable;
$(document).ready(function () {

    gTable = $("#MwrTable").DataTable(
        {
            "paging": true,
            "select":
            {
                style: 'os',
                info: false
            },

            // "dom": 'rtp',
            "processing": true,
            // "serverSide":true, // Enable server-side processing
            "pagingType": 'full_numbers',

            "ajax": {

                "url": "/MWR/GetMwrlist",
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
                { "data": "mwrlistId" },
                { "data": "activityName" },
                { "data": "activityType" },
                {
                    data: 'createdDateTime', render: function (data) {
                        // Convert the date string into a Date object
                        const date = new Date(data);
                        // Format the date as desired (e.g., 'MM/DD/YYYY')
                        return date.toLocaleDateString('en-US', {
                            year: 'numeric',
                            month: '2-digit',
                            day: '2-digit',
                            hour: '2-digit',
                            minute: '2-digit',
                            second: '2-digit',
                            hour12: true
                        });; // or use any format you prefer
                    }
                },
               /* { "data": "createdBy" },*/
                {
                    "data": null, // Use null since we're customizing the output
                    "orderable": false,
                    "render": function (data) {
                        return `<input type='button' value='Date' class='btn btn-primary mwrDate' style='width:auto'
                        data-id='${data.mwrlistId}'
                        data-actName='${data.activityName}'
                        data-actType='${data.activityType}'
                        >`;
                    }
                },
                {
                    "data": null, // Use null since we're customizing the output
                    "orderable": false,
                    "render": function (data) {
                        return `<input type='button' value='Participant' class='btn  btn-primary addParticipant' style='width:auto' 
                        data-id='${data.mwrlistId}'
                        data-actName='${data.activityName}'
                        data-actType='${data.activityType}'
                        >`;
                    }
                },
              
                {
                    "data": null, // Use null since we're customizing the output
                    "orderable": false,
                    "render": function (data) {
                        return `<input type='button' value='Update' class='btn btn-primary UpdateMWR' style='width:auto;'
                        data-id='${data.mwrlistId}'
                        data-actName='${data.activityName}'
                        data-actType='${data.referenceCode}'
                        >`;
                    }
                },

                {
                    "data": null, // Use null since we're customizing the output
                    "orderable": false,
                    "render": function (data) {
                        return `<input type='button' value='Delete' class='btn btn-danger DeleteMWR' style='width:auto'
                        data-id='${data.mwrlistId}'
                         data-actName='${data.activityName}'
                        data-actType='${data.activityType}'
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
        'margin-bottom': '10px'
    });

    $('.dataTables_filter input').css({
        'margin-left': '20px', // Add some right margin
        'width': '300px' // Set the width of the search input box
    });
    $('.dataTables_filter label input').attr('id', 'search');
    $('#MwrActivityDate').DataTable().clear().destroy();

});

$("#MwrTableBody").on("click", ".addParticipant", function () {
    var MwrID = $(this).data("id");
    var ActivityNAME = $(this).data("actname");;
    var ActivityTYPE = $(this).data("acttype");

    $('#modalAddParticipant').modal('show');
    $("#fieldParticipantActivityID").val(MwrID); //ID
    $("#fieldActName").val(ActivityNAME);
    $("#fieldActType").val(ActivityTYPE);

    resetFields();


    $.ajax({
        url: '/MWR/GetActivityDate',
        type: 'GET',
        data: { mwrlistId: MwrID },
        success: function (response) {
            // Clear previous options to avoid appending data multiple times
            $('#fieldActivityDate').empty();

            // Check if data is available and has items
            if (response.data && response.data.length > 0) {
                // Populate the dropdown with data
                $.each(response.data, function (index, item) {
                    // Convert string to Date object
                    var date = new Date(item.mwractDate);

                    // Format date in 'MM/DD/YYYY' format
                    var formattedDate = date.toLocaleDateString('en-US');

                    // Append option with formatted date
                    $('#fieldActivityDate').append(
                        $('<option></option>').val(item.mwractDate).text(formattedDate)
                    );
                });
            } else {
                // If no data is found, append a default "No data available" option
                $('#fieldActivityDate').append(
                    $('<option></option>').val('').text('No dates available')
                );
            }
        },
        error: function () {
            alert('Error retrieving data.');
            // Optional: Append a "Loading error" message or default option in case of an error
            $('#fieldActivityDate').empty().append(
                $('<option></option>').val('').text('Error loading data')
            );
        }
    });


});



$(document).ready(function() {
        
        $.ajax({
            url: '/MWR/GetActivity', 
            type: 'GET',
            success: function (response) {
                if (response.data && response.data.length > 0) {
                    // Populate the dropdown with data
                    $.each(response.data, function (index, item) {
                        $('#fieldActivity').append(
                            $('<option></option>').val(item.referenceCode).text(item.referenceName)
                        );
                    });
                } else {
                    alert(response.message); 
                }
            },
            error: function () {
                alert('Error retrieving data.');
            }
        });
});





function AddActivity() {
    var ActivityName = $("#fieldActivityName").val();
    var Activity = $("#fieldActivity").val();
    if ($("#fieldActivityName").val() != "") {

        var newData = {
            ActivityName: ActivityName,
            ActivityType: Activity
        }
        $.ajax({
            url: "/MWR/AddMwr",
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
                    //resetFields();
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
        $("#txtSystemMessage").html("Activity is required.");
        $('#modalSystemMessage').modal('show');
    }
}

//AddActDate
function AddActivityDate() {
    var ActivityId = $("#fieldActIdDate").val();
    var ActivityDate = $("#fieldMwrDate").val();
    if ($("#fieldActIdDate").val() != "") {
        var newData = {
            MwrlistId: ActivityId,
            MwractDate: ActivityDate
        }
        $.ajax({
            url: "/MWR/AddMwrDate",
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
                    //resetFields();
                    /*getRolesList();*/
                    gTable2.ajax.reload();
                    gTable2.draw();
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
        $("#txtSystemMessage").html("Activity is required Date.");
        $('#modalSystemMessage').modal('show');
    }
}
//AddParticipation
function AddMwrParticipant() {
    var ParticipantActId = $("#fieldParticipantActivityID").val();
    var ParticipantId = $("#fieldParticipantID").val();
    var MwrActivityDate = $("#fieldActivityDate").val();
    if ($("#ParticipantId").val() != "") {
        var newData = {
            MwrlistId: ParticipantActId,
            IdNumber: ParticipantId,
            ActDate: MwrActivityDate
        }
        $.ajax({
            url: "/MWR/AddMwrParticipant",
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
                    //resetFields();
                    /*getRolesList();*/
                    //gTable2.ajax.reload();
                    //gTable2.draw();
                } else if (ProcessStatus == "2") {
                    $("#txtSystemMessage").html(ProcessMessage);
                    $('#modalSystemMessage').modal('show');

                } else if (ProcessStatus == "3") {
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
        $("#txtSystemMessage").html("Activity is required Date.");
        $('#modalSystemMessage').modal('show');
    }
}


//Get Emloyee Info
function getUserInfo(userId) {
    var UserId = userId;
    $.ajax({
        url: "/Enrollment/GetEmployeeInfo",
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
               
            }
            else {
                $("#fieldFirstName").val("");
                $("#fieldLastName").val("");
                $("#fieldMiddleInitial").val("");
                $("#fieldDepartment").val("");
                $("#fieldJobTitle").val("");
             
            }
        }
    });

}



