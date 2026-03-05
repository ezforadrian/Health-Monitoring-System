
function loadDTEcuList() {
    $('#ecuDetailsTable').DataTable({
        "processing": true,
        "searching": true,
        "ordering": true,
        "pageLength": 10,
        "lengthMenu": [5, 10, 50],
        "language": {
            "emptyTable": "No records found"
        }
    });
}



function loadBranchSelectSub(branchGlobal, selectedBranch) {
    $('#BranchCodeEnc').select2({
        data: branchGlobal,
        placeholder: "Select Branch Name...",
        allowClear: true,
        width: '100%'
    }).val(selectedBranch).trigger('change');
}


function updateFindingsSummary() {
    var checkedCount = $('.test-detail-checkbox:checked').length;
    var summaryText = checkedCount > 0 ? 'Number of Findings: ' + checkedCount : 'Normal Findings';
    $('#findingsSummary').text(summaryText);
}

function updateFindingsSummaryEdit() {
    var checkedCount = $('.test-detail-checkbox-edit:checked').length;
    var summaryText = checkedCount > 0 ? 'Number of Findings: ' + checkedCount : 'Normal Findings';
    $('#editfindingsSummary').text(summaryText);
}

function submitEcuFormSub(addEcuUrl, aemViewUrl) {
    const formData = {
        IdEnc: $('#IdEnc').val(),
        EcuDate: $('#EcuDate').val(),
        BranchCodeEnc: $('#BranchCodeEnc').val(),
        Remarks: $('#Remarks').val(),
        TestIdEnc: $('.test-detail-checkbox:checked').map(function () {
            return $(this).val(); // Get the value of each checked checkbox
        }).get() // Convert the jQuery object to an array
    };


    console.log(formData)

    $.ajax({
        url: addEcuUrl,
        type: 'POST',
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        data: JSON.stringify(formData),
        success: function (response) {
            console.log(response);
            if (response.success) {
                $('#addEcuConfirmationModal').modal('hide');
                $('#addDetailModal').modal('hide');

                $('.alert-success').text("ECU Record saved successfully!").show();
                // Refresh the table with the new data
                refreshEcuTable($('#IdEnc').val());
                // Clear all error messages
                $('.text-danger').text('').hide();
            } else {
                displayErrors(response.errors);
                $('#addEcuConfirmationModal').modal('hide');
            }
        },
        error: function (xhr) {
            displayErrors(["An unexpected error occurred. Please try again later."]);
            $('#addEcuConfirmationModal').modal('hide');
        }
    });

}

function displayErrors(errors) {
    var errorList = document.getElementById('errorList');
    var editerrorList = document.getElementById('editerrorList');
    if (!errorList || !editerrorList) {
        console.error("Error list element not found");
        return;
    }

    errorList.innerHTML = ''; // Clear previous errors
    editerrorList.innerHTML = '';

    // Ensure 'errors' is an array, convert if it is a string
    if (typeof errors === 'string') {
        errors = [errors];
    }

    if (errors && Array.isArray(errors)) {
        errors.forEach(function (error) {
            var errorItem = document.createElement('div');
            var editerrorItem = document.createElement('div');
            $('#errorList').show();
            $('#editerrorList').show();
            errorItem.innerText = error;
            editerrorItem.innerText = error;
            errorList.appendChild(errorItem);
            editerrorList.appendChild(editerrorItem);
        });
    } else {
        console.error("displayErrors function received invalid 'errors' parameter:", errors);
    }
}


function refreshEcuTable(idEnc) {
    $.ajax({
        url: "/Ecu/GetEcuDetails",
        type: 'GET',
        data: { idEnc: idEnc },
        success: function (data) {
            console.log(data);
            if (data.success) {
                // Destroy the existing DataTable if it exists
                if ($.fn.dataTable.isDataTable('#ecuDetailsTable')) {
                    $('#ecuDetailsTable').DataTable().clear().destroy();
                }

                // Clear the existing table body
                var tableBody = $('#ecuDetailsTable tbody');
                tableBody.empty();

                // Populate the table with new data
                data.details.vmEcuHeaders.forEach(function (detail, index) {
                    // Determine the findings
                    var findings;
                    if (detail.ecudetails.every(test => !test.result)) {
                        findings = 'Normal findings'; // If all tests have no results
                    } else {
                        findings = detail.ecudetails
                            .filter(test => test.result)
                            .map(test => test.testName) // Get the test names of the positive results
                            .join(', ');
                    }

                    var row = `
                        <tr>
                            <td>${index + 1}</td>
                            <td>${new Date(detail.ecudate).toLocaleDateString()}</td>
                            <td>${detail.branch || 'N/A'}</td>
                            <td>${findings || 'N/A'}</td>
                            <td>${detail.remarks || 'N/A'}</td>
                            <td>
                                <button type="button" class="btn btn-primary" 
                                        onclick="editEcuDetail('${detail.employeeIdNumberEnc}', '${detail.ecuHeaderIdEnc}')">
                                    Edit
                                </button>
                            </td>
                        </tr>`;
                    tableBody.append(row);
                });

                // Reinitialize the DataTable with updated data
                $('#ecuDetailsTable').DataTable({
                    paging: true,
                    searching: true,
                    ordering: true,
                    info: true,
                    destroy: true // Add destroy option to ensure proper reinitialization
                });

                // Optionally, update a message displaying the count
                // (Not needed if DataTables manages this automatically)
                // var recordCount = data.details.vmEcuHeaders.length;
                // $('#recordCount').text(`Showing ${recordCount} records`);
            } else {
                console.error("Failed to refresh table data:", data.message || "No error message provided.");
            }
        },
        error: function (xhr) {
            console.error("Error fetching updated data:", xhr.responseText || "An error occurred.");
        }
    });
}

function clearFormAndErrors() {
    console.log("clear");
    $('#addEcuForm')[0].reset();
    $('#errorList').html('');
    $('#BranchCodeEnc').val(null).trigger('change');
}

function editEcuDetail(empIdEnc, ecuHeaderIdEnc) {

    console.log(empIdEnc);
    console.log(ecuHeaderIdEnc);

    var formData = {
        EmpIdEnc: empIdEnc,
        EcuHeaderIdEnc: ecuHeaderIdEnc
    };

    var token = $('input[name="__RequestVerificationToken"]').val();

    console.log(formData);
    //get the info using vmAddEcuRecord
    $.ajax({
        type: 'POST',
        url: '/Ecu/OpenEcuEditModal',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        headers: { 'RequestVerificationToken': token },
        success: function (response) {
            console.log(response);
            //Populate the modal with the received data
            $('#EditEmpIdEnc').val(response.details.idEnc);
            $('#EditEcuHeaderIdEnc').val(response.details.ecuHeaderIdEnc);
            $('#EditEcuDate').val(response.details.ecuDate);

            $('#EditRemarks').val(response.details.remarks);

            $('#EditBranchCodeEnc').select2({
                data: GlobalBranch,
                placeholder: "Select Branch Name...",
                allowClear: true,
                width: '100%'
            }).val(response.details.branchCodeEnc).trigger('change');

            // Populate the region checkboxes
            var editfindingsContainer = $('#editfindingsContainer');
            editfindingsContainer.empty();

            var countFindings = 0;
            $.each(response.details.testResults, function (index, testdetails) {
                var isChecked = testdetails.result == true ? 'checked' : '';
                if (testdetails.result == true) {
                    countFindings = countFindings + 1;
                }
                var checkbox = $('<div class="form-check">')
                    .append('<input type="checkbox" name="EditTestIdEnc"  class="test-detail-checkbox-edit"  value="' + testdetails.testIdEnc + '" id="findings' + testdetails.testIdEnc + '" ' + isChecked + '>')
                    .append('<label class="form-check-label" for="findings' + testdetails.testIdEnc + '">' + testdetails.testName + '</label>');
                editfindingsContainer.append(checkbox);
            });

            if (countFindings == 0) {
                $('#editfindingsSummary').text("Normal Findings");
            } else {
                $('#editfindingsSummary').text("Number of Findings: " + countFindings);
            }



            $('#editDetailModal').modal('show');
        },
        error: function (xhr, status, error) {
            // Handle error
            var errors = [];
            if (xhr.responseJSON && xhr.responseJSON.Errors) {
                errors = xhr.responseJSON.Errors;
            } else {
                errors.push(xhr.responseText);
            }

            var errorMessage;
            errors.forEach((e, index) => {
                errorMessage += e;
            });
            errorMessage;

            console.log(errorMessage);
            alert(errorMessage);
        }
    });
}

function updateEcuForm() {
    const formData = {
        IdEnc: $('#EditEmpIdEnc').val(),
        EcuHeaderIdEnc: $('#EditEcuHeaderIdEnc').val(),
        EcuDate: $('#EditEcuDate').val(),
        BranchCodeEnc: $('#EditBranchCodeEnc').val(),
        Remarks: $('#EditRemarks').val(),
        TestIdEnc: $('.test-detail-checkbox-edit:checked').map(function () {
            return $(this).val(); // Get the value of each checked checkbox
        }).get() // Convert the jQuery object to an array
    };


    console.log(formData)

    $.ajax({
        url: "/Ecu/UpdateEcuRecord",
        type: 'POST',
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        data: JSON.stringify(formData),
        success: function (response) {
            console.log(response);
            if (response.success) {
                $('#editDetailModal').modal('hide');
                $('#updateEcuConfirmationModal').modal('hide');

                $('.alert-success').text("ECU Record updated successfully!").show();
                // Refresh the table with the new data
                refreshEcuTable($('#EditEmpIdEnc').val());
                // Clear all error messages
                clearFormAndErrors();
                $('.text-danger').text('').hide();
            } else {
                displayErrors(response.errors);
                $('#updateEcuConfirmationModal').modal('hide');
            }
        },
        error: function (xhr) {
            displayErrors(["An unexpected error occurred. Please try again later."]);
            $('#updateEcuConfirmationModal').modal('hide');
        }
    });

}



