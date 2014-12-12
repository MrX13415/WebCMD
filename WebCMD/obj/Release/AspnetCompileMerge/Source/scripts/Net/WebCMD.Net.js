﻿
/* --------------------------------------------------------------------------------
 *      WebCMD v1.0
 * --------------------------------------------------------------------------------
 *
 *  JavaScript: WebCMD.Net.js
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
var onReconnectData = null;

//TODO
var template = "<div class=\"console-line {0}\">\n" +
            "\t<span>{1}</span>\n" +
            "</div>\n";

function sendRequest(data) {
    if (_debugMode) {
        cbout = String.format("<div class=\"console-line debug {0}\">\n" +
                               "\t<span>{1}</span>\n" +
                               "</div>\n", "msg-info", " (i) " + data);
        ConsoleOutput.get().innerHTML += cbout;
    }

    if (!connected) {
        onReconnectData = data;
        connectServer();
    } else {
        _request(data);
    }
}

function _request(data) {
    var webcmd = $.connection.consoleHub;
    webcmd.server.request(data, connectionID);
    sss++;
}

function initConnection() {
    if (inited) return;

    $.connection.hub.reconnecting(function () {
        console.log(' (i) Reconnecting ...');

        errout = String.format(template, "msg-client", " (i) Reconnecting ...");
        ConsoleOutput.get().innerHTML += errout;
    });

    $.connection.hub.reconnected(function () {
        console.log(' (i) Connection successfully restored.');

        errout = String.format(template, "msg-success", " (i) Connection has been successfully restored.");
        ConsoleOutput.get().innerHTML += errout;
    });

    $.connection.hub.disconnected(function () {
        console.log(' /!\\ Disconnected.');

        errout = String.format(template, "msg-warn", " /!\\ Disconnected from Server.");
        ConsoleOutput.get().innerHTML += errout;
    });

    $.connection.hub.received(function (data) {
        //console.log("RS: " + data);
    });

    $.connection.hub.connectionSlow(function (data) {
        console.log(' /!\\ Difficulties with the connection detected.');

        errout = String.format(template, "msg-warn", " /!\\ Difficulties with the connection detected!");
        ConsoleOutput.get().innerHTML += errout;
    });

    $.connection.hub.error(function (error) {
        connected = false;

        console.log(' /!\\ Connecting lost.');
        errout = String.format(template, "msg-error", " /!\\ Connection lost. Can't reach server ...");
        ConsoleOutput.get().innerHTML += errout;

        //throw error;
    });

    $.connection.hub.stateChanged(function (change) {
        if (change.newState === $.signalR.connectionState.connecting) {
            console.log(' (i) Connecting ...');

            errout = String.format(template, "msg-client", " (i) Connecting ...");
            ConsoleOutput.get().innerHTML += errout;
        }
        else if (change.newState === $.signalR.connectionState.reconnecting) {
           
        }
        else if (change.newState === $.signalR.connectionState.connected) {
            console.log(' (i) Connected. The server is online.');

            errout = String.format(template, "msg-success", "(i) Connected. The server is online. ");
            ConsoleOutput.get().innerHTML += errout;
        }
    });

    inited = true;
}

function connectServer() 
{
    initConnection();

    $.connection.hub.qs = { 'guid': GUID }
    $.connection.hub.start().done(function () {
        connectionID = $.connection.hub.id;
        connected = true;

        if (onReconnectData != null) {
            _request(onReconnectData);
            onReconnectData = null;
        }
    });
    
    var webcmd = $.connection.consoleHub;
    webcmd.client.processServerData = function (data) {
        processResponse(data);
    }
}


var crrs = 0;

function processResponse(rsdata) {

    crrs++;

    if (rsdata == "") return;

    rsdata = rsdata.replace("<crrsdata>", crrs);

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

        var id = eventArgs[1];
        var mode = eventArgs[2];
        var property = eventArgs[3];
        var args = rs.substr("_RS".length + id.length + mode.length + property.length + 4);

        if (_debugMode) {
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