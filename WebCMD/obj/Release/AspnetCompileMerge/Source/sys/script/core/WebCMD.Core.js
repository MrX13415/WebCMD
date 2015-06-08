/// <reference path="../net/WebCMD.Net.js" />

/* --------------------------------------------------------------------------------
 *      WebCMD v1.0
 * --------------------------------------------------------------------------------
 *
 *  JavaScript: WebCMD.Core.js
 * 
 *
 *
 *  Version: 2.0
 *  Copyright: Team Icelane (c) 2014
 *
 */


//--------------------------------------------------------------------------------
//     Global vars
//--------------------------------------------------------------------------------

//current client object holder
var _client_current = new Client();

//--------------------------------------------------------------------------------
//     CLASS : Client
//--------------------------------------------------------------------------------

function Client() {

    var _client_debugMode = false;
    var _client_halted = false;
    var _client_connection = new Connection();

    this.debugMode = function () { return _client_debugMode; }
    this.setDebugMode = function (val) { _client_debugMode = val; }

    this.isHalted = function () { return _client_halted; }
    this.terminate = function () { return _client_halted = true; }

    this.connection = function () { return _client_connection; }

}

Client.instance = function () { return _client_current; }

Client.debugMode = function () { return _client_current.debugMode(); }
Client.setDebugMode = function (val) { _client_current.setDebugMode(val); }

Client.terminate = function () { return _client_current.terminate(); }
Client.isHalted = function () { return _client_current.isHalted(); }

Client.connection = function () { return _client_current.connection(); }


//--------------------------------------------------------------------------------
//     Global Vars
//--------------------------------------------------------------------------------

var insertmode = false;
var lastInput = [];
var lastInputIndex = 0;
var jsCommands = GetJSCommandsDefinitions();
var forceMobile = false;
var consoleInputDisabled = false;

//--------------------------------------------------------------------------------
//     HTML Elements
//--------------------------------------------------------------------------------

var CmdDebugLastKeyVal = new HtmlElement("CmdDebugLastKeyVal");

//--------------------------------------------------------------------------------
//     Global Consts
//--------------------------------------------------------------------------------

const CMDCursorNormal = "_";
const CMDCursorFull = "█";
//KEYS:special
const Key_Enter = 13;
const Key_Backspace = 8;
const Key_Tab = 9;
const Key_Space = 32;
//KEYS:arrows
const Key_ArrowLeft = 37;
const Key_ArrowUp = 38;
const Key_ArrowRight = 39;
const Key_ArrowDown = 40;
//KEYS:edit
const Key_Ins = 45;
const Key_Del = 46;
const Key_Home = 36;
const Key_End = 35;
const Key_PageUp = 33;
const Key_PageDown = 34;
//KEYS:special-chars
const Key_Slash = 39;
const Key_Apostrophe = 47;

//--------------------------------------------------------------------------------
//     Event Handler
//--------------------------------------------------------------------------------

window.onclick = function (e) {
    if (isMobile()) focusMobileKeyboard();
}

window.onkeypress = function (e) {
    if (!isMobile()) handleKeyPressEvent(e);
}

//--------------------------------------------------------------------------------
//TODO CLEAN THIS FILE 
//
$(document).ready(function () {
    Client.connection().start();
    console.log(" (i) Ready GUID:" + GUID);
});

window.onerror = function (msg, url, row, col, ex) {

    var template = "<div class=\"console-line {0}\">\n" +
                    "\t<span>{1}</span>\n" +
                    "</div>\n";

    var message = msg; //TODO: message only ...

    if (ex != null && ex.name == "Sys.WebForms.PageRequestManagerServerErrorException") {
        errout = String.format(template, "msg-server", " /!\\ Can't reach server. Connection could not be established ...");

    } else if (ex != null && ex.name == "Sys.WebForms.PageRequestManagerTimeoutException") {
        errout = String.format(template, "msg-server", " /!\\ Request could not be processed. Connection timed out ...");

    } else {
        errout = String.format(template, "msg-error", " /!\\ FATAL ERROR: " + message);
    }
    hiddeConsoleInput(false);
    ConsoleOutput.get().innerHTML += errout;
}

function handlePageLoadEvent() {
    ConsoleDebug.get().innerHTML += String.format("<span class=\"yellow\">[Mobile device: </span><span class=\"blue\">{0}</span><span class=\"yellow\">]</span>\n", isMobile());
}

function showLastKey(code) {
    if (CmdDebugLastKeyVal.get() != null) {
        CmdDebugLastKeyVal.get().innerHTML = code;
    } else {
        ConsoleDebug.get().innerHTML += String.format("<span class=\"yellow\">[Last key: </span><span id=\"CmdDebugLastKeyVal\" class=\"blue\">{0}</span><span 	class=\"yellow\">]</span>\n", code);
    }
}

function focusMobileKeyboard() {
    if (isMobile()) CmdInputHelper.get().focus();
}

function handleMobileKeyboardKeyEvent(e) {
    if (isMobile()) handleKeyPressEvent(e);
}

function handleKeyPressEvent(e) {

    if (consoleInputDisabled) return;

    var eventHandled = true;
    var allowDefaultBehaviour = false;
    var scroll = true;

    //store postback data 
    var doServerRequest = false;
    var serverargs;

    //mod key is active ...
    var modKey = e.ctrlKey || e.altKey || e.metaKey;
    var keyCode = e.keyCode;
    var charCode = e.charCode;

    //debug
    showLastKey(keyCode ? keyCode : charCode);

    //make sure input on input and textarea elements are still working probaly ...
    var obj = e.srcElement || e.target;
   // if (obj.id != CmdInputHelper.ClientID && obj.tagName == 'INPUT' || obj.tagName == 'TEXTAREA') return true;
    if (obj.tagName == 'INPUT' || obj.tagName == 'TEXTAREA') return true;

    //handle mobile input methode ...
    //CmdInputHelper.get().value = "";

    //HANDLE: Special-keys
    if (keyCode == Key_Enter) {
        //hiddeConsoleInput(true);
        if (getInput().length > 0) {

            var input = getInput();
            sendToOutput(input);
            var _handled = handleJsCommands(input);

            if (!_handled) {

                //TODO
                var s = calcMath(input);

                if (s != "") {
                    ConsoleOutput.get().innerHTML += s;
                }

                serverargs = String.format("_RQ:{0}:{1}:{2}", '__Page', CommandRequestID, input);
                doServerRequest = true;
            } else {
                hiddeConsoleInput(false);
            }
        } else {
            sendToOutput("");
            hiddeConsoleInput(false);
        }
    }

    if (keyCode == Key_Backspace) removeLastPrefix();
    if (keyCode == Key_Tab);
    //HANDLE: Arrow-keys
    if (keyCode == Key_ArrowLeft) keyArrowLeft();
    if (keyCode == Key_ArrowRight) keyArrowRight();
    if (keyCode == Key_ArrowUp) keyArrowUp();
    if (keyCode == Key_ArrowDown) keyArrowDown();
    //HANDLE: Edit-keys
    if (keyCode == Key_Ins) keyIns();
    if (keyCode == Key_Del) removeFirstPostfix();
    if (keyCode == Key_Home) keyHome();
    if (keyCode == Key_End) keyEnd();
    if (keyCode == Key_PageUp) { scroll = false; allowDefaultBehaviour = true; }
    if (keyCode == Key_PageDown) { scroll = false; allowDefaultBehaviour = true; }
        //HANDLE: Any char-key
    else if (/*!modKey &&*/ charCode != 0) {
        add(String.fromCharCode(charCode));
        if (insertmode) removeFirstPostfix();

        allowDefaultBehaviour = true;
        //HANDLE: Speical settings for char-keys
        if (charCode == Key_Space) allowDefaultBehaviour = false;
        if (charCode == Key_Slash) allowDefaultBehaviour = false;
        if (charCode == Key_Apostrophe) allowDefaultBehaviour = false;
    }
        //HANDLE: Everything else
    else eventHandled = false;

    //***** execute for all captured key events *****

    if (scroll) doCmdScroll();
    if (!allowDefaultBehaviour) e.preventDefault();

    //send request fron ENTER key if set ...
    if (doServerRequest) Client.connection().send(serverargs);

    return allowDefaultBehaviour;
}

//--------------------------------------------------------------------------------
//     Handle command input
//--------------------------------------------------------------------------------

function calcMath(input)
{
    try {
        var cr = eval(input);
        return cr;
    } catch (e) {
        return "";
    }
}

function sendToOutput(input) {
    var template = "<span class=\"console-line\">" +
                   "<span class=\"cmd-promt blue-light\">{0}</span><span class=\"cmd-chevron\">{1}</span>" +
                   "<span class=\"cmd-inputstr\">{2}</span>" +
                   "</span></br>";
    var out = String.format(template, /* No promt on empty lines: input.length <= 0 ? " " : */ getPromt(), getChevron(), encodeHTML(input));

    addOutput(out);
    setPrefix("");
    setPostfix("");
}

function setInput() {
    var template = CmdInputHtmlTemplate.get().innerHTML;
    var out = replaceAll(template, "__ID_TMPL__", "");

    CmdInputHtmlTemplate.get().innerHTML = replaceAll(template, "id=\"", "id=\"__ID_TMPL__");
    addOutput(out);
}

function escapeRegExp(string) {
    return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

function replaceAll(string, find, replace) {
    return string.replace(new RegExp(escapeRegExp(find), 'g'), replace);
}

function handleJsCommands(input) {
    input = input.trim();
    if (input.length <= 0) return;

    if (lastInput.length <= 0 || lastInput[lastInput.length - 1].toLowerCase() != input.toLowerCase()) {
        lastInput.push(input);
        lastInputIndex = lastInput.length - 1;
    }

    var input_cmd = input.split(/\s+/)[0];
    var cmd = input_cmd.trim().toLowerCase();
    var args = input.substr(input_cmd.length).trim();
    var _handled = false;

    for (var i = 0; i < jsCommands.length; i++) {
        var _c = jsCommands[i];

        _handled = _c.execute(cmd, args);
        if (_handled) break;
    }

    return _handled;
}

//--------------------------------------------------------------------------------
//     HTML-Element modifier
//--------------------------------------------------------------------------------

function hiddeConsoleInput(hidden) {
    if (hidden) {
        CmdInputBackground.get().style.background = "none";
        CmdInputPrefix.get().setAttribute("hidden", "hidden");
        CmdInputPostfix.get().setAttribute("hidden", "hidden");
        CmdPromt.get().setAttribute("hidden", "hidden");
        consoleInputDisabled = true;
    } else {
        CmdInputBackground.get().removeAttribute("style");
        CmdInputPrefix.get().removeAttribute("hidden");
        CmdInputPostfix.get().removeAttribute("hidden");
        CmdPromt.get().removeAttribute("hidden");
        consoleInputDisabled = false;
    }
}

function doCmdScroll() {
    var x = document.body.scrollWidth;
    var y = document.body.scrollHeight;

    window.scrollTo(0, y);
}

function getOutput() {
    return ConsoleOutput.get().innerHTML;
}

function setOutput(text) {
    ConsoleOutput.get().innerHTML = text;
}

function addOutput(text) {
    setOutput(getOutput() + text);
}

function getInput() {
    return getPrefix() + getPostfix();
}

function getPromt() {
    return CmdPromt.get().innerHTML;
}

function getChevron() {
    return CmdChevron.get().innerHTML;
}

function setCursorChar(text) {
    CmdCursor.get().innerHTML = encodeHTML(text);
}

function getPrefix() {
    return decodeHTML(CmdInputPrefix.get().innerHTML);
}

function setPrefix(text) {
    CmdInputPrefix.get().innerHTML = encodeHTML(text);
}

function getPostfix(text) {
    return decodeHTML(CmdInputPostfix.get().innerHTML);
}

function setPostfix(text) {
    CmdInputPostfix.get().innerHTML = encodeHTML(text);
}

function add(text) {
    setPrefix(getPrefix() + text);
}

function addPostfix(text) {
    setPostfix(text + getPostfix());
}

function removeLastPrefix() {
    var s = getPrefix();
    if (s.length <= 0) return;
    setPrefix(s.substr(0, s.length - 1));
}

function removeFirstPostfix() {
    var s = getPostfix();
    if (s.length <= 0) return;
    setPostfix(s.substr(1));
}

function getLastPrefix() {
    var s = getPrefix();
    return s.length > 0 ? s.substring(s.length - 1, s.length) : "";
}

function getFirstPostfix() {
    var s = getPostfix();
    return s.length > 0 ? s.substring(0, 1) : "";
}

//--------------------------------------------------------------------------------
//     Key Handler
//--------------------------------------------------------------------------------

function keyArrowLeft() {
    addPostfix(getLastPrefix());
    removeLastPrefix();
}

function keyArrowRight() {
    add(getFirstPostfix());
    removeFirstPostfix();
}

function keyArrowUp() {
    if (lastInput.length <= 0) return;
    setPostfix("");
    setPrefix(lastInput[lastInputIndex]);
    lastInputIndex = lastInputIndex - 1;
    if (lastInputIndex < 0) lastInputIndex = 0;
}

function keyArrowDown() {
    if (lastInput.length <= 0) return;
    setPostfix("");
    setPrefix(lastInput[lastInputIndex]);
    lastInputIndex = lastInputIndex + 1;
    if (lastInputIndex >= lastInput.length) lastInputIndex = lastInput.length - 1;
}

function keyHome() {
    var s = getPrefix();
    if (s.length <= 0) return;
    setPostfix(s + getPostfix());
    setPrefix("");
}

function keyEnd() {
    var s = getPostfix();
    if (s.length <= 0) return;
    add(s);
    setPostfix("");
}

function keyIns() {
    insertmode = !insertmode;
    if (insertmode) setCursorChar(CMDCursorFull);
    else setCursorChar(CMDCursorNormal);
}



