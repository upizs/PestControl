// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    var ticketTable = $('#ticketTable').DataTable({
        pagingType: 'full_numbers',
        scrollX: true,
        createdRow: function (row, data, index) {
            if (data[3] === "Highest") {
                $('td', row).eq(3).addClass('text-danger')
            } if (data[3] === "High") {
                $('td', row).eq(3).addClass('text-danger')
            } 
        }
    })
    //Learn to remove pagination and how many entries depending on data size
    var universalTable = $('#universalTable').DataTable({
        scrollX:true
    })

});