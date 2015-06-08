/* --------------------------------------------------------------------------------
 *      WebCMD v1.0
 * --------------------------------------------------------------------------------
 *
 *  JavaScript: MasterCommand
 * 
 *
 *
 *  Version: 1.0
 *  Copyright: Team Icelane (c) 2014
 *
 */

function JSCommand() {

    //--------------------------------------------------------------------------------
    //     Global Vars
    //--------------------------------------------------------------------------------

    this._aliase = [];      //command aliase (lower case)

    //--------------------------------------------------------------------------------
    //     Functions
    //--------------------------------------------------------------------------------

    //
    // Check if the given command is a aliase of this class 
    //
    this.check = function (input) {
        var cmd = input.trim().toLowerCase();

        for (var i = 0; i < this._aliase.length; i++) {
            var c = this._aliase[i];
            if (c == cmd) return true;
        }

        return false;
    }

    //
    // Runs this command with the given arguments
    //
   this.execute = function (cmd, args){
       if (!this.check(cmd)) return false;
       this._execute(getArgArray(args));
       return true;
    }

    var _execute = function(args){}

    var getArgArray = function(args){
        var a = args.split(/\s+/);
        if (a.length == 1 && a[0] == "") a = [];
        return a;
    }
}
