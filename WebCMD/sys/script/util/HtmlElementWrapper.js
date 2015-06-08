/* --------------------------------------------------------------------------------
 *      WebCMD v1.0
 * --------------------------------------------------------------------------------
 *
 *  JavaScript: HtmlElementWarpper
 * 
 *  
 *
 *  Version: 1.0
 *  Copyright: Team Icelane (c) 2014
 *
 */

function HtmlElement(elementID) {

    var element = null;

    this.ClientID = elementID;    

    this.get = function () {
        if (element == null)
            element = document.getElementById(this.ClientID);
        return element;
    }
}
