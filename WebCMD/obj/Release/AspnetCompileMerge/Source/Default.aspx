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
                <%--<ajaxToolkit:ToolkitScriptManager ID="ScriptManager12" runat="server" />--%>
                <asp:ScriptManager ID="ScriptManager1" runat="server" />    

                <script src="Scripts/jquery-2.1.1.min.js"></script>
                
                <script src="Scripts/jquery.signalR-2.1.2.js"></script>
                <script src="/signalr/hubs"></script>

                <script type="text/javascript" src="scripts/Com/MasterCommand.js" ></script>
                <script type="text/javascript" src="scripts/Com/JSCommands.js" ></script>
                <!-- **** JavaScript Command definitions ********************************************-->
                    <script type="text/javascript" src="scripts/Com/Lib/WebCMD.Com.Lib.SystemInternal.js" ></script>
                
                    <!-- **** Add your new command script file here ****-->
                    <!-- <script type="text/javascript" src="Script/Commands/<Your-Command-Scriptfile-here.js>" ></script> -->

                <!-- ********************************************************************************-->
                <script type="text/javascript" src="scripts/Net/WebCMD.Net.js" ></script>
                <script type="text/javascript" src="scripts/Core/WebCMD.Core.js" ></script>            
                <script type="text/javascript">
                    
                    var _pm = getParameterByName("mobile");
                    if (_pm != null && _pm == "1" || _pm == "true") {
                        forceMobile = true;
                    } else {
                        forceMobile = false;
                    }

                </script>
                 <script type="text/javascript">
                     //****************************************************************************************************************
                     var GUID = '<%= GUID %>';
                     var CommandRequestID = '<%= WebCMD.Net.IO.ServerResponse.GetDefaultRequestID %>';
                     //*** JS HTML-ELEMENT HANDLEING **********************************************************************************
                     var ConsoleHeader = new HtmlElement('<%= ConsoleHeader.ClientID %>');
                     var ConsoleConInfo = new HtmlElement('<%= ConsoleConInfo.ClientID %>');
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

                <div id="ConsoleConInfo" class="console-output" runat="server">
                    
                </div>
                <br/>

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
