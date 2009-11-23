/*
    This is based on the example from the examples-jquery from Cometd
    http://downloads.dojotoolkit.org/cometd/
    However, it's been altered 
    (a) for the simpler chat program we're using, and
    (b) to (hopefulyl) work with both the jQuery and Dojo toolkits
*/
var chat = function() {
    var _chatSubscription;
    var _metaSubscriptions = [];
    var _connected = false;
    var _cometd;
    var _username;

    return {
        init: function(cometd, username) {

            // Store the initialisation parameters    
            _cometd = cometd;
            _username = username;

            // Subscribe to the meta channels
            _metaSubscribe();

            // And finally, initialise the connection
            _cometd.init('/comet.axd');

        }
    }

    function _chatUnsubscribe() {
        if (_chatSubscription) _cometd.unsubscribe(_chatSubscription);
        _chatSubscription = null;
    }

    function _chatSubscribe() {
        _chatUnsubscribe();
        _chatSubscription = _cometd.subscribe('/chat', this, handleIncomingMessage);
    }

    function _metaUnsubscribe() {
        for (var subscription in _metaSubscriptions) {
            _cometd.removeListener(subscription);
        }
        _metaSubscriptions = [];
    }

    function _metaSubscribe() {
        _metaUnsubscribe();
        _metaSubscriptions.push(_cometd.addListener('/meta/handshake', this, _metaHandshake));
        _metaSubscriptions.push(_cometd.addListener('/meta/connect', this, _metaConnect));
    }

    function _metaHandshake(message) {
        _connected = false;
        handleIncomingMessage({ data: { message: "Handshake complete. Successful? " + message.successful} });
    }

    function _metaConnect(message) {
        var wasConnected = _connected;
        _connected = message.successful;
        if (wasConnected) {
            if (_connected) {
                // Normal operation, a long poll that reconnects
            }
            else {
                // Disconnected
                handleIncomingMessage({ data: { message: "Disconnected"} });
            }
        } else {
            // We weren't connected
            if (_connected) {
                // But now we are
                handleIncomingMessage({ data: { message: "Connected"} });
                // Subscribe to the /chat channel and send a message
                _cometd.startBatch();
                _chatSubscribe();
                _cometd.publish('/chat', {
                    message: _username + ' has joined'
                });
                _cometd.endBatch();
            }
            else {
                // Could not connect
                handleIncomingMessage({ data: { message: "Unable to connect"} });
            }
        }
    }

    function leave() {
        if (!_username) return;

        _cometd.startBatch();
        _cometd.publish('/chat', {
            sender: _username,
            message: _username + ' has left'
        });
        _chatUnsubscribe();
        _cometd.endBatch();

        _metaUnsubscribe();

    }


} ();
