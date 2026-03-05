

$(document).ready(function () {
    var idEnc = getQueryParameter('id');
    console.log(idEnc);
    if (idEnc) {
        refreshAmeTable(idEnc);
    } else {
        console.error('idEnc not found in URL');
    }

    $('#ameDetailsTable').DataTable({
        "processing": true,
        "searching": false,
        "ordering": true,
        "pageLength": 10,
        "lengthMenu": [5, 10, 50],
        "language": {
            "emptyTable": "No records found"
        }
    });



    function refreshAmeTable(idEnc) {
        $.ajax({
            url: "/Ame/GetAmeDetails",
            type: 'GET',
            data: { idEnc: idEnc },
            success: function (data) {
                console.log(data);
                if (data.success) {
                    // Destroy the existing DataTable if it exists
                    if ($.fn.dataTable.isDataTable('#ameDetailsTable')) {
                        $('#ameDetailsTable').DataTable().clear().destroy();
                    }

                    // Clear the existing table body
                    var tableBody = $('#ameDetailsTable tbody');
                    tableBody.empty();

                    // Populate the table with new data
                    data.details.vmAmeHeaders.forEach(function (detail, index) {
                        // Determine the findings
                        var findings;
                        if (detail.amedetails.every(test => !test.result)) {
                            findings = 'Normal findings'; // If all tests have no results
                        } else {
                            findings = detail.amedetails
                                .filter(test => test.result)
                                .map(test => test.testName) // Get the test names of the positive results
                                .join(', ');
                        }

                        var row = `
                    <tr>
                        <td>${index + 1}</td>
                        <td>${new Date(detail.amedate).toLocaleDateString()}</td>
                        <td>${detail.branch || 'N/A'}</td>
                        <td>${findings || 'N/A'}</td>
                        <td contenteditable="true" class="editable-remarks" data-id="${detail.ameHeaderIdEnc}">
                            ${detail.remarks || 'N/A'}
                        </td>
                    </tr>`;
                        tableBody.append(row);
                    });

                    // Reinitialize the DataTable with updated data
                    $('#ameDetailsTable').DataTable({
                        paging: true,
                        searching: true,
                        ordering: true,
                        info: true,
                        destroy: true // Add destroy option to ensure proper reinitialization
                    });

                    $('.editable-remarks').on('blur', function () {
                        var newRemarks = $(this).text();
                        var detailId = $(this).data('id');
                        saveRemarks(detailId, newRemarks);
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

    function saveRemarks(detailId, newRemarks) {
        var token = $('input[name="__RequestVerificationToken"]').val();

        console.log(detailId);
        console.log(newRemarks.trim());
      
        $.ajax({
            url: '/Ame/SaveRemarks',
            type: 'POST',
            data: {
                id: detailId,
                remarks: newRemarks.trim()
            },
            headers: { 'RequestVerificationToken': token },
            success: function (response) {
                if (response.success) {
                    $('.alert-success').text(response.message).show();
                    $('.text-danger').text('').hide();
                } else {
                    $('.alert-success').text(response.message).show();
                    $('.text-danger').text('').hide();
                    
                }
            },
            error: function (xhr) {
                console.error("Error updating remarks:", xhr.responseText || "An error occurred.");
            }
        });
    }


  
});



