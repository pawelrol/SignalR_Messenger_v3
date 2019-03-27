


var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();     // to jest wszystko z tej biblioteki SignalR.js tam jest ten obiekt SignalR i on ma te metody i funkcje, lecimy z pamięci - wskazujemy mu tylko miejsce / adresten ze startup - tam to skonfigurowaliśmy

connection.start();


$("#sendButton").click(function () {
    connection.invoke("SendMessage", "Pawel", "Hello");    //nasz klient ma uruchomic metodę SendMessage na serverze, i podajemy parametry do tej metody (invoke jest wbudowany i uruchomi SendMessage)

})

connection.on("ReciveMessage", function (user, message) {       //invoke wrzucamy na server i metoda on - ortrzymaj coś z servera/ nasz klient który wysyłą też otrzyma to co wysyła
    alert(user + " " + message);
});

connection.on("ReciveMessage2", function (userId, conId) {       //invoke wrzucamy na server i metoda on - ortrzymaj coś z servera/ nasz klient który wysyłą też otrzyma to co wysyła
    $("[name='" + userId + "']").attr('id', conId);
    $("[name='" + userId + "']").css({ 'color': 'green' });
    console.log(userId);
});

connection.on("ReciveMessage3", function (list) {       //przekazujemy całąliste znajomych którzy są terazonline
    var arr = JSON.parse(list);

    for (var i = 0; i < arr.length; i++) {
        var updateId = arr[i].Id;             // lecimy po liscie userów która dostaliśmyuod servera i dlakażego bierzemy connection ID (to jest JSON więc on pierwszą literą nam zrobił dużą, uwaga na to) -strigify zmniejsza litery, parsowanie na JSON ich nie zmienia
        $("[name='" + updateId + "']").attr('id', arr[i].ConnectionId);
        $("[name='" + updateId + "']").css({ 'color': 'green' });
    }

    console.log(arr);
});

connection.on("ReciveMessage4", function (user, message, conIdFromUser) {
    $(".chat-with-user").html(user);
    $(".chat-with-user").attr('id', conIdFromUser);
    $("#msg-window").append("\n" + message);
})



$("#send-msg").click(function () {
    var msg = $("#new-msg").val();
    $("#new-msg").val('');

    var conId = $(".chat-with-user").attr('id');
    $("#msg-window").append("\n" + msg);
    connection.invoke("SendMessageToUser", conId, msg);

    



});