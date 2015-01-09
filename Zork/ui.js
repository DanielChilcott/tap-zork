function start() {

    //check a valid game ID has been passed
    var gameId = getQueryStringValue("gameid");
    if ((gameId == null) || (gameId == "")) {
        var url = window.location.toString();

        var i = url.indexOf("://");
        var j = url.indexOf("/", (i > -1 ? i + 3 : 0));
        url = url.substring(0, j + 1);
        url += "?gameId=" + Math.floor(Math.random() * 1000000000);
        window.location = url;
        return;
    }

    //grab event for adding words from the game window
    $("#gameWindow").on('click', 'span.word', function (event) {
        addWordToCommand(event.target.innerText);
    });

    //grab event for adding verbs from buttons
    $("#verbContainer").on('click', 'button', function (event) {

        var element = $(event.target);
        element = (element.is("button") ? element : element.parent("button"));

        if (element.attr("id") == "togglePaletteButton") {
            $("#moveVerbContainer").toggle();
            $("#extVerbContainer").toggle();

            var $target = $('html,body');
            $target.animate({ scrollTop: $target.height() }, 200);
        } else {
            if (!element.attr("command")) {
                addWordToCommand(element.text());
            } else {
                clearCurrentCommand();
                addWordToCommand(element.attr("command"));
                executeCurrentCommand();
            }
        }
    });

    $("#manualCommandTextbox").on("keypress", function (event) {
        if (event.keyCode == 13) {
            executeCurrentCommand();
        }
    });

    //grab event for removing words from current commands
    $("#commandContainer").on('click', function (event) {
        event.target.remove();
    });

    //grab handler to execute command
    $("#executeCommand").click(function () {
        executeCurrentCommand();
    });

    addVerbPalette();

    //show controls

    $("#verbContainer").show();

    executeCurrentCommand();
}

function getQueryStringValue(key) {
    return unescape(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + escape(key).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
}

//adds a set of available verbs
function addVerbPalette() {

    var BASIC_VERBS = [
        "look", "take", "drop"
    ];

    var EXTENDED_VERBS = [
        "read", "open", "close", "tie", "turn",
        "put", "with", "on", "off", "in", "to",
        "kill", "break", "throw",
        "eat", "drink", "pray",
        "move", "score"
    ];

    for (var i = 0; i < BASIC_VERBS.length; i++) {
        $("<button />", { class: "palWord pad btn btn-default" }).text(BASIC_VERBS[i]).prependTo($("#priVerbContainer"));
    }

    EXTENDED_VERBS = EXTENDED_VERBS.sort();
    for (var i = 0; i < EXTENDED_VERBS.length; i++) {
        $("<button />", { class: "palWord pad btn btn-default" }).text(EXTENDED_VERBS[i]).appendTo($("#extVerbContainer"));
    }
}

//adds the specified word to the current command
function addWordToCommand(word) {

    //clean word
    word = word.replace(".", "").replace(",", "").replace("?", "").replace("!", "").trim();

    $("<button />", { class: "pad btn btn-default" }).text(word).appendTo($("#commandContainer"));
}

function getCurrentCommand() {
    var command = "";
    $("#commandContainer").children("button").each(function (key, value) {
        if (command.length > 0) command += " ";
        command += value.innerText;
    });

    //tidy up the command.
    if ((command != "look") && (command.indexOf("look") > -1))
        command = command.replace("look", "look at");

    var manualCommand = $("#manualCommandTextbox").val();
    if (manualCommand.length > 0) command += " " + manualCommand;

    return command;
}

function clearCurrentCommand() {
    $("#commandContainer").empty();
    $("#manualCommandTextbox").val("");
}

//writes command or response text to the main window
function writeString(text, extraClass) {

    var para = $("<p />");

    for (var l = 0; l < text.length; l++) {
        var words = text[l].split(" ");

        var line = $("<div />", { class: "content-main" });

        for (var w = 0; w < words.length; w++) {
            var span = $("<span />", { class: "word " + extraClass }).text(words[w] + " ").appendTo(line);

        }
        line.appendTo(para);

    }

    para.appendTo($("#gameWindow"));
};

//executes the current command by echoing it, sending it, getting and echoing the response and then clearing the current command
function executeCurrentCommand() {

    var command = getCurrentCommand();

    writeString([command], "text-muted");

    var gameId = getQueryStringValue("gameid");

    $("#executeCommand").prop("disabled", true);
    $("#loaderImage").show();

    $.ajax({
        url: "/api/game/" + gameId,
        type: "POST",
        data: JSON.stringify({ "command": command }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {

            writeString(response.Response, "text-primary");

            clearCurrentCommand();

            var $target = $('html,body');
            $target.animate({ scrollTop: $target.height() }, 200);

            $("#loaderImage").hide();
            $("#executeCommand").prop("disabled", false);
        }
    });
}