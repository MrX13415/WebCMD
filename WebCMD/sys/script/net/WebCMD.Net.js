
/* --------------------------------------------------------------------------------
 *      WebCMD v1.0
 * --------------------------------------------------------------------------------
 *
 *  JavaScript: WebCMD.Net.js
 * 
 *
 *
 *  Version: 2.0
 *  Copyright: Team Icelane (c) 2014
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
    var template    = "<span class=\"console-line {0}\">" +
                "\t<span class=\"{1}\">{2}</span>" +
                "</span>";

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
                                   "</div>\n", "console-message msg-info", " (i) " + data);
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
        //ConsoleConInfo.get().opacity = 1.0;
        ConsoleConInfo.get().innerHTML = msg;
        calcHeaderMargins();
        //$('#ConsoleConInfo').fadeTo(1000, 0.4, "linear", function () { });
    }

    function _request(data) {
        $.connection.consoleHub.server.request(data, _id);
        _rqcount++; //update request count sended by the client
    }

    function _init() {
        if (_inited || Client.isHalted()) return;

        $.connection.hub.reconnecting(function () {
            console.log(' (i)  Reconnecting ...');

            errout = String.format(template, "console-message msg-client", "blink", " (i)  Reconnecting ...");
            //ConsoleOutput.get().innerHTML += errout;
            _updateHtmlState(errout);
        });

        $.connection.hub.reconnected(function () {
            console.log(' (i)  Connection successfully restored.');

            errout = String.format(template, "console-message msg-success", "", " (i)  Connection has been successfully restored.");
            //ConsoleOutput.get().innerHTML += errout;
            _updateHtmlState(errout);
        });

        $.connection.hub.disconnected(function () {
            console.log(' /!\\  Disconnected.');

            errout = String.format(template, "console-message msg-warn", "", " /!\\  Disconnected from Server.");
            //ConsoleOutput.get().innerHTML += errout;
            _updateHtmlState(errout);
        });

        $.connection.hub.received(function (data) {

        });

        $.connection.hub.connectionSlow(function (data) {
            console.log(' /!\\  Difficulties with the connection detected.');

            errout = String.format(template, "console-message msg-warn", "", " /!\\  Connection difficulties detected!");
            //ConsoleOutput.get().innerHTML += errout;
            _updateHtmlState(errout);
        });

        $.connection.hub.error(function (error) {
            console.log(' /!\\  Connecting lost.');
            errout = String.format(template, "console-message msg-error", "", " /!\\  Connection lost. Can't reach server ...");
            //ConsoleOutput.get().innerHTML += errout;
            _updateHtmlState(errout);
            //throw error;
        });

        $.connection.hub.stateChanged(function (change) {

            if (change.newState === $.signalR.connectionState.connecting) {
                console.log(' (i)  Connecting ...');

                errout = String.format(template, "console-message msg-client", "blink", " (i)  Connecting ...");
                //ConsoleOutput.get().innerHTML += errout;
                _updateHtmlState(errout);
            }
            else if (change.newState === $.signalR.connectionState.connected) {
                console.log(' (i)  Connected. The server is online.');

                errout = String.format(template, "console-message msg-success", "", "(i)  Connected. The server is online. ");
                //ConsoleOutput.get().innerHTML += errout;
                _updateHtmlState(errout);
            }
        });

        _inited = true;
    }
    
    var li = 0;

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
            
            var modeN = parseInt(eventArgs[3]);
            var mode = (modeN >= 10 ? (modeN - 10) : modeN).toString();
            var anim = modeN >= 10;

            var property = eventArgs[4];
            var args = rs.substr("_RS".length + type.length + id.length + (modeN).toString().length + property.length + 5);

            var linetarget = "Line_" + li;
            li++;

            if (Client.debugMode()) {
                cbout = String.format("<div class=\"console-line debug {0}\">\n" +
                                        "\t<span>{1}</span>\n" +
                                        "</div>\n", "console-message msg-success", " (i) " + rs.substr(0, rs.length - args.length));
                ConsoleOutput.get().innerHTML += cbout;
            }

            cbout = args;

            //args = "<div id=\"" + linetarget + "\">" + args + "</div>";

            var target = document.getElementById(id);

            if (property.indexOf(".") > -1) {
                var plist = property.split(".");
                var obj = target;
                var index;
                for (index = 0; index < plist.length - 1; ++index) {
                    var p = plist[index];
                    obj = obj[p];
                }
                target = obj;
                property = plist[plist.length - 1];
            }


            //var asdf = document.getElementById("ConsoleInput");
            //asdf["style"]["visibility"] = "hidden";

            if (!anim || property != "innerHTML") {
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
                        
                        break;
                }

            } else if (property == "innerHTML") {

                //*********************************
                //**** TYPING INPUT ANIMATION *****
                //*********************************

                var _const_charDelay = 1;       //in ms. the time to wait between each char
                var _const_blockDelay = 20;    //in ms. the time to wait between each block (special text portion)

                var _str = args + " ";          //the full response text
                var _index = 0;                 //the current index in the text
                var _text = "";                 //the current output of the text
                var _depth = 0;                 //the current tag depth
                var isHtmlTag = false;          //the next text portion is a tag
                var isCharTag = false;          //the next text portion is a special char tag
                var lineMode = false;           //wait longer then usual. (usualy after a block)
                var noDelay = false;            //don't wait at all
                var prevInnerHTML = target.innerHTML;

                //*** INIT ******

                //pause cursor animation
                CmdCursor.get().style.animation = '0';       

                //hide promt
                CmdPromt.get().style.display = 'none';
                CmdChevron.get().style.display = 'none';

                doCmdScroll();

                //*** BEGINN OF ANIMATION ******

                //insert the text with a animation
                (function type(noEnd) {
                    
                    //just get one char more of the full text ...
                    _index++;
                    _text = _str.slice(0, _index);

                    //end of animation?
                    if (_text === _str)
                    {
                        //*** ANIMATION HAS REACHED THE END ******

                        //reactivate cursor animation 
                        CmdCursor.get().style = '';

                        // +++ TODO +++ 
                        // Do this only after the commenand has been quit ! (event required !!)

                        //show promt
                        CmdPromt.get().style = '';
                        CmdChevron.get().style = '';

                        // +++ TODO +++  

                        //keep the screen pos. up to date 
                        //doCmdScroll();

                        return; //END
                    }

                    //get the last char in text ...
                    var char = _text.slice(-1);

                    //next text is a HTML tag (e.g <span>)
                    if (char === '<')
                    {
                        isHtmlTag = true;
                        _depth += 1;

                        //find tag def. ...
                        var _tag = _str.slice(_index - 1, _str.indexOf('>', _index) + 1);

                        //is a "special" tag, aka <div> aka use a different animation (start tags only)
                        if (_tag.match(/<([\s])*div([\s]|>)/g))
                        {
                            lineMode = true;

                            //targeting start and end tags!
                            var i_ = _str.regexIndexOf(/<([\s])*(\/)*([\s])*div([\s]|>)/g, _index);

                            if (i_ > -1)
                            {
                                _text = _str.slice(0, i_);
                                _index = i_;
                            }
                        }
                    }
                    else if (isHtmlTag && char === '>')
                    {
                        isHtmlTag = false;
                        _depth -= 1;
                        noDelay = true;
                    }

                    //next text is a special (HTML) char (e.g &#0571;)
                    if (char === '&')
                    {
                        isCharTag = true;
                    }
                    //if the next special char is not a '#' 
                    else if (isCharTag && char.match(/[^A-Za-z0-9#]/g))
                    {
                        isCharTag = false;
                        noDelay = true;
                    }

                    if (isHtmlTag || isCharTag) return type();  //no output and no delay


                    //handle the edit mode of the property
                    switch (mode)
                    {
                        case "1": //PropertyEditMode.AddEnd
                            target.innerHTML = prevInnerHTML + _text;
                            break;
                        case "2": //PropertyEditMode.Set
                            target.innerHTML = _text;
                            break;
                        case "3": //PropertyEditMode.AddTop
                            target.innerHTML = _text + prevInnerHTML;
                            break;
                        default:
                            //target.innerHTML = prevInnerHTML + _text;
                            break;
                    }

                    //keep the screen pos. up to date
                    doCmdScroll();

                    //determine the deleay type
                    if (lineMode)
                    {
                        lineMode = false;
                        _delay = _const_blockDelay;
                    }
                    else if (noDelay) {
                        noDelay = false;
                        return type();  //no delay
                    }
                    else
                    {
                        _delay = _const_charDelay;
                    }

                    setTimeout(type, _delay);  // ANIMATION SPEED (delay in ms)
                }());

                //********************************
                //*** END OF ANIMATION CODE ******
                //********************************
            }

            hiddeConsoleInput(false);
          //  doCmdScroll();   FUCK !
        } else {

            cbout = String.format("<div class=\"console-line debug {0}\">\n" +
                                        "\t<span>{1}</span>\n" +
                                        "</div>\n", "console-message msg-server", " (i)  RAW: " + rsdata);

            ConsoleOutput.get().innerHTML += cbout;

            hiddeConsoleInput(false);
            doCmdScroll();
        }
    }
}