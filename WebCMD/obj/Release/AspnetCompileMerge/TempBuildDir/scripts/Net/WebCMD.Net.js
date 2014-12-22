
/* --------------------------------------------------------------------------------
 *      WebCMD v1.0
 * --------------------------------------------------------------------------------
 *
 *  JavaScript: WebCMD.Net.js
 * 
 *
 *
 *  Version: 2.0
 *  Copyright: Team Icelane (c) 2014connection_IsConnected(
 *
 */

//--------------------------------------------------------------------------------
//     CLASS : Connection
//--------------------------------------------------------------------------------

function Connection() {

    var _this       = this;
    var _id         = null;
    var _inited     = false;
    var _recondata  = null;

    var _rqcount    = 0;
    var _rscount    = 0;

    //TODO
    var template    = "<div class=\"console-line {0}\">\n" +
                "\t<span class=\"{1}\">{2}</span>\n" +
                "</div>\n";

    //states:
    //  connecting
    //  connected
    //  connectionSlow
    //  reconnecting
    //  reconnected
    //  disconnected
    this.isConnected = function()
    {
        return $.connection.connectionState === $.signalR.connectionState.connected;
    }

    this.isConnecting = function()
    {
        return $.connection.connectionState === $.signalR.connectionState.connecting
            || $.connection.connectionState === $.signalR.connectionState.reconnecting;
    }

    this.send = function(data) {
        if (Client.debugMode()) {
            cbout = String.format("<div class=\"console-line debug {0}\">\n" +
                                   "\t<span>{1}</span>\n" +
                                   "</div>\n", "msg-info", " (i) " + data);
            ConsoleOutput.get().innerHTML += cbout;
        }

        if (!_this.isConnected() && !_this.isConnecting()) {
            _recondata = data;
            _this.start();
        } else {
            _request(data);
        }
    }

    this.start = function()
    {
        if (Client.isHalted()) return;
       _init();

        $.connection.hub.qs = { 'guid': GUID }
        $.connection.hub.start().done(function ()
        {
            _id = $.connection.hub.id;

            if (_recondata != null) {
                _request(_recondata);
                _recondata = null;
            }
        });

        $.connection.consoleHub.client.processServerData = function (data)
        {
            _processResponse(data);
        }
    }
    
    this.stop = function()
    {
        $.connection.hub.stop();
    }

    this.reconnect = function()
    {
        _this.stop();
        setTimeout(function () { _this.start(); }, 250);
    }

    function _updateHtmlState(msg) {
        $('#ConsoleConInfo').fadeTo(0, 100, function () { });
        ConsoleConInfo.get().innerHTML = msg;
    }

    function _request(data) {
        $.connection.consoleHub.server.request(data, _id);
        _rqcount++; //update request count sended by the client
    }

    function _init() {
        if (_inited || Client.isHalted()) return;

        $.connection.hub.reconnecting(function () {
            console.log(' (i)  Reconnecting ...');

            errout = String.format(template, "msg-client", "blink", " (i)  Reconnecting ...");
            //ConsoleOutput.get().innerHTML += errout;
            _updateHtmlState(errout);
        });

        $.connection.hub.reconnected(function () {
            console.log(' (i)  Connection successfully restored.');

            errout = String.format(template, "msg-success", "", " (i)  Connection has been successfully restored.");
            //ConsoleOutput.get().innerHTML += errout;
            _updateHtmlState(errout);
        });

        $.connection.hub.disconnected(function () {
            console.log(' /!\\  Disconnected.');

            errout = String.format(template, "msg-warn", "", " /!\\  Disconnected from Server.");
            //ConsoleOutput.get().innerHTML += errout;
            _updateHtmlState(errout);
        });

        $.connection.hub.received(function (data) {

        });

        $.connection.hub.connectionSlow(function (data) {
            console.log(' /!\\  Difficulties with the connection detected.');

            errout = String.format(template, "msg-warn", "", " /!\\  Connection difficulties detected!");
            //ConsoleOutput.get().innerHTML += errout;
            _updateHtmlState(errout);
        });

        $.connection.hub.error(function (error) {
            console.log(' /!\\  Connecting lost.');
            errout = String.format(template, "msg-error", "", " /!\\  Connection lost. Can't reach server ...");
            //ConsoleOutput.get().innerHTML += errout;
            _updateHtmlState(errout);
            //throw error;
        });

        $.connection.hub.stateChanged(function (change) {

            if (change.newState === $.signalR.connectionState.connecting) {
                console.log(' (i)  Connecting ...');

                errout = String.format(template, "msg-client", "blink", " (i)  Connecting ...");
                //ConsoleOutput.get().innerHTML += errout;
                _updateHtmlState(errout);
            }
            else if (change.newState === $.signalR.connectionState.connected) {
                console.log(' (i)  Connected. The server is online.');

                errout = String.format(template, "msg-success", "", "(i)  Connected. The server is online. ");
                //ConsoleOutput.get().innerHTML += errout;
                _updateHtmlState(errout);
            }
        });

        _inited = true;
    }

    function _processResponse(rsdata) {
        _rscount++; //update the number of responses the client handler has reseived and tryed to process ...

        if (rsdata == "") return;

        rsdata = rsdata.replace("<rscountdata>", _rscount);

        //cbout = String.format("<div class=\"console-line debug {0}\">\n" +
        //                                "\t<span>{1}</span>\n" +
        //                                "</div>\n", "msg-success", " (i) " + rsdata + " | " + (rsdata - ii));
        //ConsoleOutput.get().innerHTML += cbout;

        //hiddeConsoleInput(false);
        //doCmdScroll();

        //return;

        var rs = rsdata;
        var cbout = "";
        var eventArgs = rs.split(':');

        if (eventArgs[0] != "_RS") return;

        if (eventArgs.length >= 4) {

            //"_RS:{0}:{1}:{2}:{3}"  // id : mode : property/function : data

            var type = eventArgs[1];
            var id = eventArgs[2];
            var mode = eventArgs[3];
            var property = eventArgs[4];
            var args = rs.substr("_RS".length + type.length + id.length + mode.length + property.length + 5);

            if (Client.debugMode()) {
                cbout = String.format("<div class=\"console-line debug {0}\">\n" +
                                        "\t<span>{1}</span>\n" +
                                        "</div>\n", "msg-success", " (i) " + rs.substr(0, rs.length - args.length));
                ConsoleOutput.get().innerHTML += cbout;
            }

            cbout = args;

            var target = document.getElementById(id);

            switch (mode) {
                case "1": //PropertyEditMode.AddEnd
                    target[property] += args;
                    break;
                case "2": //PropertyEditMode.Set
                    target[property] = args;
                    break;
                case "3": //PropertyEditMode.AddTop
                    target[property] = args + target[property];
                    break;
                default:
                    //target[property] += args;
                    break;
            }

            hiddeConsoleInput(false);
            doCmdScroll();
        } else {

            cbout = String.format("<div class=\"console-line debug {0}\">\n" +
                                        "\t<span>{1}</span>\n" +
                                        "</div>\n", "msg-server", " (i) RAW: " + rsdata);

            ConsoleOutput.get().innerHTML += cbout;

            hiddeConsoleInput(false);
            doCmdScroll();
        }
    }
}