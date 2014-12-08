<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebCMD.Core.WebConsolePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    
    <title>CMD - Team Icelane</title>

    <link rel="stylesheet" type="text/css" href="style/colors.css"/>
    <link rel="stylesheet" type="text/css" href="style/main.css"/>
    <link rel="stylesheet" type="text/css" href="style/elements.css"/>
    <link rel="stylesheet" type="text/css" href="style/effects.css"/>
    <link rel="stylesheet" type="text/css" href="style/anim.css"/>


    <script type="text/javascript">
        var _debugMode = false;
    </script>
    <script type="text/javascript" src="scripts/Core/ErrorHandler.js" ></script>  
    <script type="text/javascript" src="scripts/Util/Utilities.js" ></script>
    <script type="text/javascript" src="scripts/Util/HtmlElementWrapper.js" ></script>  
                    
</head>
<body onload="handlePageLoadEvent();">
    <div id="Main">
		<div id="Content">
            <form id="form1" runat="server">
                <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server" />

                <script src="Scripts/jquery-2.1.1.min.js"></script>
                
                <script src="Scripts/jquery.signalR-2.1.2.js"></script>
                <script src="/signalr/hubs"></script>

                <script type="text/javascript" src="scripts/Com/MasterCommand.js" ></script>
                <script type="text/javascript" src="scripts/Com/JSCommands.js" ></script>
                <!-- **** JavaScript Command definitions ********************************************-->
                    <script type="text/javascript" src="scripts/Com/Commands/SystemInternal.command.js" ></script>
                
                    <!-- **** Add your new command script file here ****-->
                    <!-- <script type="text/javascript" src="Script/Commands/<Your-Command-Scriptfile-here.js>" ></script> -->

                <!-- ********************************************************************************-->
                <script type="text/javascript" src="scripts/Net/RequestHandler.js" ></script>
                <script type="text/javascript" src="scripts/IO/InputHandler.js" ></script>            
                <script type="text/javascript">
                    
                    //TODO: PUT THIS INOT A SCRIPT FILE

                    var _pm = getParameterByName("mobile");
                    if (_pm != null && _pm == "1" || _pm == "true") {
                        forceMobile = true;
                    } else {
                        forceMobile = false;
                    }

                    connectServer();

                    var webcmd = $.connection.consoleHub;

                        // Send a maximum of 10 messages per second 
                        // (mouse movements trigger a lot of messages)
                        //messageFrequency = 10,
                        // Determine how often to send messages in
                        // time to abide by the messageFrequency
                        //updateRate = 1000 / messageFrequency;

                    var crrs = 0;
                    
                    webcmd.client.processServerData = function (rsdata) {

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

                </script>
                 <script type="text/javascript">
                     //*** JS HTML-ELEMENT HANDLEING **********************************************************************************
                     var ConsoleHeader = new HtmlElement('<%= ConsoleHeader.ClientID %>');
                     var ConsoleDebug = new HtmlElement('<%= ConsoleDebug.ClientID %>');
                     var ConsoleOutput = new HtmlElement('<%= ConsoleOutput.ClientID %>');
                     var ConsoleInput = new HtmlElement('<%= ConsoleInput.ClientID %>');
                     var ConsoleFooter = new HtmlElement('<%= ConsoleFooter.ClientID %>');
                     var CmdPromt = new HtmlElement('<%= CmdPromt.ClientID %>');
                     var CmdInputPrefix = new HtmlElement('<%= CmdInputPrefix.ClientID %>');
                     var CmdInputPostfix = new HtmlElement('<%= CmdInputPostfix.ClientID %>');
                     var CmdCursor = new HtmlElement('<%= CmdCursor.ClientID %>');
                     var CmdInputHelper = new HtmlElement('<%= CmdInputHelper.ClientID %>');
                     var CmdInputBackground = new HtmlElement('<%= CmdInputBackground.ClientID %>');
                     //****************************************************************************************************************
                </script>

                <div id="ConsoleHeader" class="console-output" runat="server">

                </div>
				<div id="ConsoleDebug" class="console-output" style="margin-bottom: 10px;" runat="server" hidden="hidden">
						
                </div>
                <div id="ConsoleOutput" class="console-output" runat="server">

                </div>

                <div id="ConsoleInput" class="console-output" runat="server">
                    <div id="CmdInputBackground" class="console-line console-inputline" runat="server">
                        <span id="CmdPromt" class="cmd-promt" runat="server">&gt;</span><span id="CmdInputPrefix" class="cmd-inputstr" runat="server"></span><span id="CmdCursor" class="cmd-cursor blink" runat="server">_</span><span id="CmdInputPostfix" class="cmd-inputstr" runat="server"></span>
                        <asp:TextBox ID="CmdInputHelper" style="width: 0; height: 0; opacity:0; filter:alpha(opacity=0);" onkeydown="handleMobileKeyboardKeyEvent(event);" autocomplete="off" runat="server" autofocus=""></asp:TextBox>
                    </div>
                </div>

                <div id="ConsoleFooter" class="console-output" runat="server">

                </div>

                <asp:UpdatePanel ID="ConsoleInputPanel" UpdateMode="Conditional" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="CmdInputHelper" />
                    </Triggers>
                    <ContentTemplate>
                        
                    </ContentTemplate>
                </asp:UpdatePanel>
            </form>

        </div>
		<div id="Footer" class="effect-textdown-dark">Team IceLane (c) 2014</div>
	</div>		
</body>
</html>
