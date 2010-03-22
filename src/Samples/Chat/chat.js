/*
    This uses the libraries from cometd-javascript 1.0.0
    It is based on the example from the examples-jquery from Cometd
    http://downloads.dojotoolkit.org/cometd/
    However, it's been altered 
    (a) for the simpler chat program we're using, and
    (b) to work with both the jQuery and Dojo toolkits
*/
var chat = function() {
    var _chatSubscription;
    var _metaSubscriptions = [];
    var _cometd;
    var _username;
    var _disconnecting;

    return {
        init: function(cometd, username, password) {

            // Store the initialisation parameters    
            _cometd = cometd;
            _username = username;

            // Subscribe to the meta channels
            _metaSubscribe();

            // Configure the connection
            _cometd.configure({ url: 'comet.axd' });

            // And handshake - with authentication, as described at
            // http://cometd.org/documentation/howtos/authentication
            _cometd.handshake({
                ext: {
                    authentication: {
                        user: username,
                        credentials: password
                    }
                }
            });
        }

        , leave: function() {
            if (!_username) return;

            _cometd.startBatch();
            _cometd.publish('/chat', {
                message: _username + ' has left'
            });
            _unsubscribe();
            _cometd.disconnect();
            _cometd.endBatch();

            _metaUnsubscribe();
            _disconnecting = true;
        }

    }

    function _unsubscribe() {
        if (_chatSubscription) _cometd.unsubscribe(_chatSubscription);
        _chatSubscription = null;
    }

    function _subscribe() {
        _unsubscribe();
        _chatSubscription = _cometd.subscribe('/chat', this, handleIncomingMessage);
    }

    function _metaUnsubscribe() {
        for (var subNumber in _metaSubscriptions) {
            _cometd.removeListener(_metaSubscriptions[subNumber]);
        }
        _metaSubscriptions = [];
    }

    function _metaSubscribe() {
        _metaUnsubscribe();
        _metaSubscriptions.push(_cometd.addListener('/meta/handshake', this, _metaHandshake));
        _metaSubscriptions.push(_cometd.addListener('/meta/connect', this, _metaConnect));
        _metaSubscriptions.push(_cometd.addListener('/meta/unsuccessful', this, _metaUnsuccessful));
    }

    function _metaHandshake(message) {
        _connected = false;
        _chatSubscription = null;
        handleIncomingMessage({ data: { message: "Handshake complete. Successful? " + message.successful} });
    }

    function _connectionEstablished() {
        handleIncomingMessage({
            data: {
                message: 'Connection to Server Opened'
            }
        });
        _cometd.startBatch();
        _subscribe();
        _cometd.publish('/chat', {
            message: _username + ' has joined'
        });
        _cometd.endBatch();
    }

    function _connectionBroken() {
        handleIncomingMessage({
            data: {
                message: 'Connection to Server Broken'
            }
        });
    }

    function _connectionClosed() {
        handleIncomingMessage({
            data: {
                message: 'Connection to Server Closed'
            }
        });
    }

    var _connected = false;
    function _metaConnect(message) {
        if (_disconnecting) {
            _connected = false;
            _connectionClosed();
        }
        else {
            var wasConnected = _connected;
            _connected = message.successful === true;
            if (!wasConnected && _connected) {
                _connectionEstablished();
            }
            else if (wasConnected && !_connected) {
                _connectionBroken();
            }
        }
    }

    function _metaUnsuccessful(message) {
        handleIncomingMessage({ data: { message: "Request on channel " + message.channel + " failed: " + (message.error == undefined ? "No message" : message.error)} });
    }

} ();
