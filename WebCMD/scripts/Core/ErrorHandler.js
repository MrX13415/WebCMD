
/* --------------------------------------------------------------------------------
 *      WebCMD v1.0
 * --------------------------------------------------------------------------------
 *
 *  JavaScript: ErrorHandler
 * 
 *
 *
 *  Version: 1.0
 *  Copyright: Team Icelane (c) 2014
 *
 */

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
