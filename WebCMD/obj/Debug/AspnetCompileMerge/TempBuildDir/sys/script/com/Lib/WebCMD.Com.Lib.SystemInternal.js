/* --------------------------------------------------------------------------------
 *      WebCMD v1.0
 * --------------------------------------------------------------------------------
 *
 *  JavaScript: Internal commands
 * 
 *  System-Internal console commands
 *
 *  Version: 1.0
 *  Copyright: Team Icelane (c) 2014
 *
 */

//
// COMMAND: Reload
//
CMD_reload.prototype = new JSCommand;

function CMD_reload() {

    this._aliase = ["reload"];      //command aliase (lower case)

    this._execute = function (args)
    {
        document.getElementById("ConsoleInput").style.display = "none"; //make some func. out of this ...
        addOutput("<div class=\"console-line msg-client\"><span>Reloading ...</span></div>");
        location.reload();
    }
}

//
// COMMAND: Reload
//
CMD_exit.prototype = new JSCommand;

function CMD_exit() {

    this._aliase = ["exit", "terminate"];      //command aliase (lower case)

    this._execute = function (args) {

        Client.connection().stop(); //disconnect from server ...
        document.getElementById("ConsoleInput").style.display = "none"; //TODO: make some func. out of this ...
        Client.terminate();
        
        addOutput("<div class=\"console-line msg-client\"><span>[terminated] WebCMD has been halted.</span></div>");
        window.close();
        addOutput("<br><div class=\"console-line msg-warn\"><span> /!\\ Not allowed to close this window, please close it manually ...</span></div>");
    }
}

//
// COMMAND: Reload
//
CMD_reconnect.prototype = new JSCommand;

function CMD_reconnect() {

    this._aliase = ["reconnect"];      //command aliase (lower case)

    this._execute = function (args) {

        addOutput("<div class=\"console-line msg-client\"><span> (i)  Reconnecting to the server ...</span></div>");
        Client.connection().reconnect();
    }
}


//
// COMMAND: Clear
//
CMD_clear.prototype = new JSCommand;

function CMD_clear() {

    this._aliase = ["clear", "cls", "clear-host"];      //command aliase (lower case)

    this._execute = function (args)
    {
        setOutput("");
        setInput();
    }
}

//
// COMMAND: Meow
//
CMD_meow.prototype = new JSCommand;

function CMD_meow() {

    this._aliase = ["meow", "nya"];      //command aliase (lower case)

    this._execute = function (args)
    {
        addOutput("<div class=\"console-line back-gray\"><span class=\"yellow\">😺 Meow Meow </span><span class=\"green\">:3</span></div>");
    }
}

//
// COMMAND: Debug
//
CMD_debug.prototype = new JSCommand;

function CMD_debug() {

    this._aliase = ["debug"];      //command aliase (lower case)

    this._execute = function (args)
    {
        Client.setDebugMode(!Client.debugMode());
        ShowDebug(Client.debugMode());
    }
}

function ShowDebug(b) {
    document.getElementById("ConsoleDebug").hidden = !b;
    if (!b) document.getElementById("ConsoleFooter").innerHTML = "";
    calcHeaderMargins();
}

//
// COMMAND: Debug
//
CMD_mobile.prototype = new JSCommand;

function CMD_mobile() {

    this._aliase = ["mobile"];      //command aliase (lower case)

    this._execute = function (args) {
        forceMobile = !forceMobile;
        addOutput("<div class=\"console-line\"><span class=\"yellow\">Force mobile input handler: </span><span class=\"blue\">" + forceMobile + "</span></div>");
    }
}

//
// COMMAND: Help
//
CMD_help.prototype = new JSCommand;

function CMD_help() {

    this._aliase = ["help", "?"];      //command aliase (lower case)

    this._execute = function (args) {
        addOutput("<div class=\"console-line\"><br/><span class=\"yellow\">Help -- WebCMD v1.0 -- Team Icelane (c) 2014</span><br/><br/><table>" +
            "<tr><td class=\"blue\">help </td><td> </td><td>Prints this help screen</td></tr>" +
            "<tr><td class=\"blue\">clear </td><td> </td><td>Clears the console output</td></tr>" +
            "<tr><td class=\"blue\">reload </td><td> </td><td>Reloads the console</td></tr>" +
            "<tr><td class=\"blue\">debug </td><td> </td><td>Toggles the debug mode</td></tr>" +
            "<tr><td class=\"blue\">goto </td><td> </td><td>Redirects to the given URL</td></tr>" +
            "<tr><td class=\"blue\">todo </td><td> </td><td>View and/or report bugs/todos</td></tr>" +
            "<tr><td class=\"blue\">srvtest </td><td> </td><td>Test the server</td></tr>" +
            "</table><br/></div>");
    }
}

//
// COMMAND: Goto
//
CMD_goto.prototype = new JSCommand;

function CMD_goto() {

    this._aliase = ["goto", "redirect"];      //command aliase (lower case)

    this._execute = function (args) {
        if (args.length <= 0 )
        {
            addOutput("<div class=\"console-line\"><span class=\"red\">Error: Invalid arguments: </span><span>Please specifie a target URL (e.g. \"goto http://www.google.de\")</span></div>");
        }
        else
        {
            var addr = args[0];
            if (addr.indexOf("://") <= -1) {
                addr = "http://" + addr;
            }
            if (addr.indexOf(".") <= -1) {
                addr += ".com";
            }

            addOutput("<div class=\"console-line msg-client\"><span>Redirecting to \'</span><span class=\"white\">" + args[0] + "</span><span>\' ...</span></div>");
            window.location.href = addr;
        }
    }
}


//
// COMMAND: Console test command
//
CMD_test.prototype = new JSCommand;

function CMD_test() {

    this._aliase = ["test"];      //command aliase (lower case)

    this._execute = function (args) {
        if (args.length <= 0 )
        {
            addOutput("<div><span>Usage: Test [color|error]</span></div>");
        }
        else
        {
            switch (args[0].toLowerCase()) {
                case "error":
                    CMD_test_error();
                    break;
                case "color":
                    CMD_test_color();
                    break;
                default:
                    addOutput("<div><span>Usage: Test [color|error]</span></div>");
                    break;
            }
        }
    }
}

    function CMD_test_error() {
        throw new Error("Internal test error occured! Don't panic! This is just a test :)");
    }

    function CMD_test_color() {
        var t = "<div class=\"back-{0}-dark {1}\">\n" +
                "\t<span>{2}</span>\n" +
                "</div>\n";

        var colors = ["red", "yellow", "orange", "green", "turky", "blue", "magenta", "gray", "lightgray"];

        var out = "";
        for (var i = 0; i < colors.length; i++) {
            var c = colors[i];
            out += String.format(t, c, c, "COLOR TEST -- Color: " + c + " -- This is a example text ...");
            out += String.format(t, c, c + "-light", "COLOR TEST -- Color: " + c + "-light -- This is a example text ...");
        }

        addOutput(out);
    }


//
// COMMAND: Goto
//
CMD_cd.prototype = new JSCommand;

function CMD_cd() {

    this._aliase = ["jscd"];      //command aliase (lower case)

    this._execute = function (args) {
        if (args.length <= 0 || args[0].toLowerCase().startsWith("http://") || args[0].toLowerCase().startsWith("https://"))
        {
            addOutput("<div class=\"console-line\"><span class=\"red\">Error: Invalid arguments: </span><span>Please specifie a directory</span></div>");
        }
        else
        {
            var cur = getParameterByName("p");

            if (cur == null) cur = "";

            var s = "";
            var n = args[0];

            if (args[0].toLowerCase().trim() == "..")
            {
                s = cur;
                var lps = s.lastIndexOf("/");
                s = s.substr(0, lps);
            } else
            {
                s = cur + "/" + args[0];
            }

            addOutput("<div class=\"console-line\"><span class=\"orange\">Changing directory to \'" + n + "\' ...</span></div>");
            window.location.href = "Default.aspx?p=" + s;
        }
    }
}


if (typeof String.prototype.startsWith != 'function') {
    // see below for better implementation!
    String.prototype.startsWith = function (str) {
        return this.indexOf(str) == 0;
    };
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}
