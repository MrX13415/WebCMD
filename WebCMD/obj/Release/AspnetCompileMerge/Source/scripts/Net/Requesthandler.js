
/* --------------------------------------------------------------------------------
 *      WebCMD v1.0
 * --------------------------------------------------------------------------------
 *
 *  JavaScript: RequestHandler
 * 
 *
 *
 *  Version: 1.0
 *  Copyright: Team Icelane (c) 2014
 *
 */
var connected = false;
var connectionID;
var sss = 0;
var sid = new Date().getTime();
var inited = false;

function sendRequest(data) {
    if (_debugMode) {
        cbout = String.format("<div class=\"console-line debug {0}\">\n" +
                               "\t<span>{1}</span>\n" +
                               "</div>\n", "msg-info", " (i) " + data);
        ConsoleOutput.get().innerHTML += cbout;
    }

    _request(data);
}

var template = "<div class=\"console-line {0}\">\n" +
            "\t<span>{1}</span>\n" +
            "</div>\n";

function connectServer() {
    initConnection();
}

function initConnection() {
    if (inited) {
        $.connection.hub.start();
    } else {
        inited = true;

        $.connection.hub.received(function (data) {
            //console.log("RS: " + data);
        });

        $.connection.hub.error(function (error) {
            connected = false;

            errout = String.format(template, "msg-error", " /!\\ Connection lost. Can't reach server ...");
            ConsoleOutput.get().innerHTML += errout;

            //throw error;
        });

        $.connection.hub.stateChanged(function (change) {
            if (change.newState === $.signalR.connectionState.reconnecting) {
                console.log('Re-connecting');
                errout = String.format(template, "msg-client", " (i) Re-connecting ...");
                ConsoleOutput.get().innerHTML += errout;
            }
            else if (change.newState === $.signalR.connectionState.connected) {
                console.log('The server is online');
                errout = String.format(template, "msg-success", "(i) Connected. The server is online");
                ConsoleOutput.get().innerHTML += errout;
                
            }
        });

        $.connection.hub.reconnected(function () {
            console.log('Reconnected');
            errout = String.format(template, "msg-success", " (i) Reconnected. The server is online");
        });

        $.connection.hub.start().done(function () {
            connected = true;
            connectionID = $.connection.hub.id;
        });
    }
}

function _request(data) {
    if (!connected) connectServer();

    try {
        var webcmd = $.connection.consoleHub;
        webcmd.server.request(data, connectionID);
        sss++;
    } catch (e) {
        connected = false;
        throw e;
    }
}
