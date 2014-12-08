
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