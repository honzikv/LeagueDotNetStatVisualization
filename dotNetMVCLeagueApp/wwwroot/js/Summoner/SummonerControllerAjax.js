// hodnoty summoner a server jsou deklarovany pred timto skriptem

$('#formFilter').on('submit', (e) => {
    e.preventDefault();

    let url = $('#formFilter').attr('action'); // ziskame url
    // let queueType = $('#queueFilter').val();
    let queue = $("#queueFilter").children("option").filter(":selected").val();
    console.log(queue);
    let numberOfGames = $('#numberOfGames').val();

    let form = new FormData();
    form.append("Name", summoner);
    form.append("Server", server);
    form.append("NumberOfGames", numberOfGames);
    form.append("Queue", queue);

    let settings = {
        "async": true,
        "crossDomain": true,
        "url": url,
        "method": "POST",
        "processData": false,
        "contentType": false,
        "mimeType": "multipart/form-data",
        "data": form
    }

    $.ajax(settings).done(response => {
        $('#matchListPartial').html(response)
    });

   
    
});

