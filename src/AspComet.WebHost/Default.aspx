<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AspComet.WebHost._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="Scripts/dojo/dojo.js"></script>
    <script type="text/javascript" language="javascript">

        dojo.require("dojox.cometd");

        function setUpConnection()
        {
            dojox.cometd.init("/Comet.axd");
            dojox.cometd.subscribe("/test",
                function(comet)
                {
                    alert("message received");
                });
        }

        function sendMessage()
        {
            dojox.cometd.publish("/test", {}); 
        }

    </script>

</head>
<body>
    <div>
        <a href="#" id="begin" onclick="setUpConnection();return false;">Begin</a>    
    </div>
    <div>
        <a href="#" id="send" onclick="sendMessage();return false;">Send</a>    
    </div>
</body>
</html>
