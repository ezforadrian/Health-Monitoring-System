$(document).ready(function () {
    console.log("I'm here");
    var idEncMwr = getQueryParameter('id');
    console.log(idEncMwr);
    if (idEncMwr) {
        refreshMwrTable(idEncMwr);
    } else {
        console.error('idEnc not found in URL');
    }

    $('#mwrDetailsTable').DataTable({
        "processing": true,
        "searching": false,
        "ordering": true,
        "pageLength": 10,
        "lengthMenu": [5, 10, 50],
        "language": {
            "emptyTable": "No records found"
        }
    });

    function refreshMwrTable(idEncMwr) {
        $.ajax({
            url: "/MWR/GetMwrList_DataTable",
            type: 'GET',
            data: { idEncMwr: idEncMwr },
            success: function (data) {
                console.log('Response Data:', data);
                if (data.success) {
                    // Destroy the existing DataTable if it exists
                    if ($.fn.dataTable.isDataTable('#mwrDetailsTable')) {
                        $('#mwrDetailsTable').DataTable().clear().destroy();
                    }

                    // Clear the existing table body
                    var tableBody = $('#mwrDetailsTable tbody');
                    tableBody.empty();

                    if (data.details && data.details.length > 0) {
                        // Populate the table with new data
                        data.details.forEach(function (detail, index) {
                            var row = `
                                <tr>
                                    <td>${index + 1}</td>
                                    <td>${new Date(detail.actDate).toLocaleDateString()}</td>
                                    <td>${detail.activityName}</td>
                                    <td>${detail.activityType}</td>
                                </tr>`;
                            tableBody.append(row);
                        });

                        // Reinitialize the DataTable with updated data
                        $('#mwrDetailsTable').DataTable({
                            paging: true,
                            searching: true,
                            ordering: true,
                            info: true,
                            destroy: true // Add destroy option to ensure proper reinitialization
                        });
                    } else {
                        console.warn("No data available in details array.");
                        // Optionally initialize DataTable to show "No records found" message
                        $('#mwrDetailsTable').DataTable({
                            paging: true,
                            searching: true,
                            ordering: true,
                            info: true,
                            destroy: true // Add destroy option to ensure proper reinitialization
                        });
                    }
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
