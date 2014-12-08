


//register the commands specified in this file
function GetJSCommandsDefinitions() {
    var _cmds = [];
    //**** Command definitions ****************

        //System-Internal commands 
    _cmds.push.apply(_cmds, [new CMD_reload, new CMD_clear, new CMD_meow, new CMD_debug, new CMD_help, new CMD_goto, new CMD_cd, new CMD_test, new CMD_mobile]);

        //*** add your commands here ***

    //*****************************************
    return _cmds;
}