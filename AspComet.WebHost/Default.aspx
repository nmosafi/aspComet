<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AspComet.WebHost._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/json2.js" type="text/javascript"></script> 
    <script src="Scripts/jquery-1.2.6.min.js" type="text/javascript"></script> 
    <script src="Scripts/jquery.comet.js" type="text/javascript"></script> 
    
    <script type="text/javascript" language="javascript">
        $(document).ready(function()
        {
            $("#begin").click(function(event)
            {
                $.comet.publish("/aspcomet/channel", "This is the data");
                return false;
            });
            $.comet.init("/Comet.axd");
            $.comet.subscribe("/aspcomet/channel", function(data) { alert(data); });
        });
    
    </script>

</head>
<body>
    <div>
        <a href="#" id="begin">Begin</a>    
    </div>
</body>
</html>
