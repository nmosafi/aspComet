<%@ Page %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>AspComet chat sample - jQuery</title>
		<!-- Load the jQuery libraries --> 
		<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" type="text/javascript"></script>
		<script src="lib/jquery.json-2.2.js" type="text/javascript"></script>
		<!--- Load the core Cometd library, and the jQuery binding --->
		<script src="lib/cometd.js" type="text/javascript"></script>
		<script src="lib/jquery.cometd.js" type="text/javascript"></script>
		<!-- Load our chat JS file --> 
		<script src="chat.js" type="text/javascript"></script>
		<script type="text/javascript" language="javascript">

		    $(document).ready(function() {

    		    // Ensure we disconnect appropriately
	    	    $(window).unload(chat.leave);
		    
		        // Get the users name
	    	    var name = window.prompt('Enter your nick name:');
	    	    var password = window.prompt('Enter your password ("password" will work!):');

		        // Initialise the chat - this will take the jQuery comet object, 
		        // handshake with the server
		        // and then subscribe to the /chat channel
		        chat.init($.cometd, name, password);

		        // Publish any messages the user enters
		        $('#entry').submit(function() {
		            $.cometd.publish('/chat', { sender: name, message: $('#message').val() });
		            $('#message').focus().val('');
		            return false;
		        });

		        // focus the entry box
		        $('#message').focus();

		    });

		    function handleIncomingMessage(comet) {
                $('<li>' + (comet.data.sender || 'System') + ': ' + comet.data.message + '</li>').
           				    appendTo('#messages').css({ fontStyle: comet.data.sender ? 'normal' : 'italic' });
	        }

		</script>
		<style type="text/css">
		
		* { margin: 0; padding: 0; }
		body { height: 100%; font: 75% Arial, Helvetica, sans-serif; }
		ul#messages { position: absolute; bottom: 0; top:0; left: 0; right: 0; margin: 0 0 32px 0; padding: 5px; overflow: auto; background: #eee; list-style-type: none; }
		form#entry { height: 22px; position: absolute; bottom: 0; left: 0; width: 100%; padding: 5px 0; background: #ddd; }
		form#entry input { margin: 0 0 0 5px; }
		input#message { width: 30em; }
		
		</style>
	</head>
	<body>
		<ul id="messages">
		</ul>
		
		<form id="entry" action="">
			<input type="text" id="message" />
			<input type="submit" value="&raquo;" />
		</form>
	</body>
</html>
