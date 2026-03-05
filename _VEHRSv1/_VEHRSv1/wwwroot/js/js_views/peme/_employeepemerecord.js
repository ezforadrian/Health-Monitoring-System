$(document).ready(function () {


    // Open modal and set action based on button clicked
    $('#addDetailModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);  // Button that triggered the modal
        var actionType = button.hasClass('edit-detail-btn') ? 'edit' : 'add';

        if (actionType === 'edit') {
            $('#addDetailModalLabel').text("Edit Pre Employment Detail");
            populateFormData(button);
            $('#confirmSaveDetailBtn').text("Save Changes").data('action', 'edit');
        } else {
            $('#addDetailModalLabel').text("Add Pre Employment Detail");
            clearFormAndErrors();
            $('#confirmSaveDetailBtn').text("Save Detail").data('action', 'add');
        }
    });

    // Populates form fields with button data (for Edit actions)
    function populateFormData(button) {
        console.log(button); // Log button to inspect data attributes
        $('#PemeIdStringModal').val(button.data('pemeid'));
        $('#PemeDetailStringModal').val(button.data('detailid')); // Corrected to 'detailid'
        $('#ExamDate').val(button.data('examdate'));
        $('#MedicalEvaluatorModal').val(button.data('mdid')).trigger('change');
        $('#Remarks').val(button.data('remarks'));
    }

    // Clear errors and form fields when the modal is hidden
    $('#addDetailModal').on('hidden.bs.modal', function () {
        clearFormAndErrors();
    });

    $('#confirmSaveDetailBtn').click(function () {
        $('#addDetailConfirmationModal').modal('hide'); // Close confirmation modal
        $('#addDetailModal').modal('show'); // Open the edit/add detail modal
    });

    function clearFormAndErrors() {
        $('#addPemeDetailForm')[0].reset();
        $('#errorList').html('');
        $('#MedicalEvaluatorModal').val(null).trigger('change');
    }

    $('#pemeDetailsTable').DataTable({
        paging: true,
        searching: false,
        ordering: false,
        info: true,
        autoWidth: false,
    });


    $('#UpdateRecordBtn').hide(); // Show the update button
    

    $('#editRecordBtn').click(function () {
        if ($(this).text() === "Edit Record") {
            // Enable form fields for editing
            $('#pemeForm input, #pemeForm select').removeAttr('readonly').removeAttr('disabled');
            $('#UpdateRecordBtn').show(); // Show the update button
            $(this).text("Cancel Update"); // Change button text to "Cancel Update"
        } else {
            // Disable form fields again
            $('#pemeForm input, #pemeForm select').attr('readonly', true).attr('disabled', true);
            $('#UpdateRecordBtn').hide(); // Hide the update button
            $(this).text("Edit Record"); // Change button text back to "Edit Record"

            // Hide the success message
            $('.alert-success').hide(); // Hide the success message
        }
    });


    // Handle the form submission
    $('#pemeForm').submit(function (event) {
        event.preventDefault();

        $.ajax({
            type: $(this).attr('method'),
            url: $(this).attr('action'),
            data: $(this).serialize(),
            success: function (response) {
                if (response.success) {
                    $('.alert-success').text("Record updated successfully!").show();
                    $('#pemeForm input, #pemeForm select').attr('readonly', true).attr('disabled', true);
                    $('#editRecordBtn').text("Edit Record"); // Change button text back to "Edit Record"
                    $('#UpdateRecordBtn').hide(); // Hide the update button after successful update

                    // Clear all error messages
                    $('.text-danger').text('').hide(); // Clear text and hide spans


                } else {
                    if (response.errors) {

                        $('#pemeForm input, #pemeForm select').removeAttr('readonly').removeAttr('disabled');
                        $('.text-danger').text('');
                        $.each(response.errors, function (fieldName, errorMessage) {
                            var errorSpan = $('[name="' + fieldName + '"]').siblings('.text-danger'); // Target the sibling span directly
                            if (errorSpan.length) {
                                errorSpan.text(errorMessage).show(); // Show the error span
                            }
                        });

                    } else {
                        alert(response.message);
                    }
                }
            },
            error: function () {
                alert('An error occurred while submitting the form.');
            }
        });
    });








    //-----Saving of PemeDetail
    let formData = {}; // To store the form data temporarily
    let isEditAction = false; // Flag to check if action is Add or Edit

    // Event listener for Save Detail button in Add/Edit modal
    $('#saveDetailBtn').click(function () {
        isEditAction = false; // Set as Add action by default

        // Prepare form data
        formData = {
            PemeIdString: $('#PemeIdStringModal').val(),
            ExamDate: $('#ExamDate').val(),
            MedicalEvaluatorIdString: $('#MedicalEvaluatorModal').val(),
            Remarks: $('#Remarks').val()
        };

        // Show confirmation modal
        $('#addDetailConfirmationModal').modal('show');
    });



    $('#confirmSaveDetailBtn').click(function () {

        const isEditAction = $(this).data('action') === 'edit';
        const actionUrl = isEditAction
            ? '/PEME/UpdatePemeDetail' //@Url.Action("UpdatePemeDetail", "PEME")
            : '/PEME/SavePemeDetail'; //@Url.Action("SavePemeDetail", "PEME")


        // Prepare form data for submission
        const formData = {
            PemeIdString: $('#PemeIdStringModal').val(),
            PemeDetailIdString: $('#PemeDetailStringModal').val(),
            ExamDate: $('#ExamDate').val(),
            MedicalEvaluatorIdString: $('#MedicalEvaluatorModal').val(),
            Remarks: $('#Remarks').val()
        };

        // Get the anti-forgery token value from the form
        var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

        // Send data to the server via AJAX after confirmation
        $.ajax({
            url: actionUrl,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'RequestVerificationToken': antiForgeryToken // Include the anti-forgery token in the header
            },
            data: JSON.stringify(formData),
            success: function (response) {
                if (response.success) {
                    $('#addDetailModal').modal('hide');
                    $('#addDetailConfirmationModal').modal('hide');
                    $('.alert-success').text("Pre-employment detail saved successfully!").show();
                    // Refresh the table with the new data
                    refreshTable($('#PemeIdStringModal').val());
                    // Clear all error messages
                    $('.text-danger').text('').hide();
                } else {
                    displayErrors(response.errors);
                    $('#addDetailConfirmationModal').modal('hide');
                }
            },
            error: function (xhr) {
                displayErrors(["An unexpected error occurred. Please try again later."]);
                $('#addDetailConfirmationModal').modal('hide');
            }
        });

    });

    function displayErrors(errors) {

        var errorList = document.getElementById('errorList');
        if (!errorList) {
            console.error("Error list element not found");
            return;
        }

        errorList.innerHTML = ''; // Clear previous errors

        if (errors && Array.isArray(errors)) {

            errors.forEach(function (error) {
                var errorItem = document.createElement('div');
                $('#errorList').show();
                errorItem.innerText = error;
                errorList.appendChild(errorItem);

            });
        } else {

            console.error("displayErrors function received invalid 'errors' parameter:", errors);
        }
    }

    function refreshTable(pemeId) {
        $.ajax({
            url: '/PEME/GetPemeDetails', //@Url.Action("GetPemeDetails", "PEME")
            type: 'GET',
            data: { pemeId: pemeId },
            success: function (data) {
                if (data.success) {

                    var tableBody = $('#pemeDetailsTable tbody');
                    tableBody.empty(); // Clear existing rows

                    // Accessing details.pemeDetails instead of data.pemeDetails
                    data.details.pemeDetails.forEach(function (detail, index) {
                        var row = `
                                <tr>
                                        <td>${index + 1}</td>
                                        <td>${new Date(detail.examDate).toLocaleDateString()}</td>
                                        <td>${detail.medicalEvaluatorName || 'N/A'}</td>
                                        <td>${detail.remarks || 'N/A'}</td>
                                        <td>
                                            <button type="button" class="btn btn-info edit-detail-btn"
                                                    data-bs-toggle="modal" data-bs-target="#addDetailModal"
                                                    data-pemeid="${detail.pemeIdString}"
                                                    data-detailid="${detail.detailIdString}"
                                                    data-mdid="${detail.medicalEvaluatorIdString}"
                                                    data-examdate="${detail.examDate.split('T')[0]}"
                                                    data-remarks="${detail.remarks || 'N/A'}">
                                                Edit
                                            </button>
                                        </td>
                                    </tr>`;
                        tableBody.append(row);
                    });
                } else {
                    console.error("Failed to refresh table data:", data.message || "No error message provided.");
                }
            },
            error: function (xhr) {
                console.error("Error fetching updated data:", xhr.responseText || "An error occurred.");
            }
        });
    }















});