
/* --------------------------------------------------------------------------------
 *      WebCMD v1.0
 * --------------------------------------------------------------------------------
 *
 *  JavaScript: Utilities
 * 
 *
 *
 *  Version: 1.0
 *  Copyright: Team Icelane (c) 2014
 *
 */


function sleep(millis) {
    var date = new Date();
    var curDate = null;

    do { curDate = new Date(); }
    while (curDate - date < millis);
}

String.prototype.regexIndexOf = function (regex, startpos) {
    var indexOf = this.substring(startpos || 0).search(regex);
    return (indexOf >= 0) ? (indexOf + (startpos || 0)) : indexOf;
}

String.prototype.regexLastIndexOf = function (regex, startpos) {
    regex = (regex.global) ? regex : new RegExp(regex.source, "g" + (regex.ignoreCase ? "i" : "") + (regex.multiLine ? "m" : ""));
    if (typeof (startpos) == "undefined") {
        startpos = this.length;
    } else if (startpos < 0) {
        startpos = 0;
    }
    var stringToWorkWith = this.substring(0, startpos + 1);
    var lastIndexOf = -1;
    var nextStop = 0;
    while ((result = regex.exec(stringToWorkWith)) != null) {
        lastIndexOf = result.index;
        regex.lastIndex = ++nextStop;
    }
    return lastIndexOf;
}

function isMobile() {
    return forceMobile || (typeof window.orientation !== "undefined") || (navigator.userAgent.indexOf('IEMobile') !== -1);
}

function encodeHTML(text) {
    var out = text;
    out = code(out, "&", "&amp;");  //has to be the first!
    out = code(out, " ", "&nbsp;");
    out = code(out, "<", "&lt;");
    out = code(out, ">", "&gt;");
    return out;
}

function decodeHTML(text) {
    var out = text;
    out = code(out, "&nbsp;", " ");
    out = code(out, "&lt;", "<");
    out = code(out, "&gt;", ">");
    out = code(out, "&amp;", "&"); //has to be the last!
    return out;
}

function code(input, str1, str2) {
    return input == null ? "" : input.replace(new RegExp(str1, "g"), str2);
}

function elementInViewport(el) {
    var top = el.offsetTop;
    var left = el.offsetLeft;
    var width = el.offsetWidth;
    var height = el.offsetHeight;

    while (el.offsetParent) {
        el = el.offsetParent;
        top += el.offsetTop;
        left += el.offsetLeft;
    }

    return (top >= window.pageYOffset &&
      left >= window.pageXOffset &&
      (top + height) <= (window.pageYOffset + window.innerHeight) &&
      (left + width) <= (window.pageXOffset + window.innerWidth));
}

function elementOutOfRightBounds(el) {
    var top = el.offsetTop;
    var left = el.offsetLeft;
    var width = el.offsetWidth;
    var height = el.offsetHeight;

    while (el.offsetParent) {
        el = el.offsetParent;
        top += el.offsetTop;
        left += el.offsetLeft;
    }

    return (left + width) > (window.pageXOffset + window.innerWidth);
}