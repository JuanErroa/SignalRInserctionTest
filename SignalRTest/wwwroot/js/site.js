$(() => {
    let connection = new signalR.HubConnectionBuilder().withUrl("/signalServer").build();

    connection.start();

    connection.on("newPersonDetected", function () {
        loadData();
    });

    function loadData() {
        $.ajax({
            url: '/Home/GetLastPersona',
            method: 'GET',
            success: (result) => {
                addPersonToTable(result);
            },
            error: (error) => {
                console.log(error);
            }
        });
    }
});

var tablePersonas;
$(document).ready(function () {
    tablePersonas = $('#myTable').DataTable({
        "order": [[0, "desc"]],
        "ordering": true

    });
});

function addPersonToTable(person) {
    tablePersonas.row.add([person.id, person.firstName, person.lastName]).draw(true);
}

$('#newPersonForm').submit(function (e) {
    e.preventDefault();
    var data = $(this).serialize();

    $.ajax({
        url: '/Home/InsertPerson',
        type: 'post',
        data: data
    });
});