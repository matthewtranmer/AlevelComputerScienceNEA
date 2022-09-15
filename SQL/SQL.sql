The client the performs an X3DH with the pre key bundle. 

The pre key bundle contains: 
The public identity key,
The public signed pre key and the
The ephemeral pre key

Once the client generates the root key, it should generate a key pair, perform a diffie hellman and create a new chain key.  

The chain key will then be used to derive the next chain and then the message will be encrypted.

The client then will upload the encrypted message, the chain key, the private key



PROBLEM TO SOLVE:
how do we know if its the first message in a chain or not.

LOGIC FOR SENDING MESSAGES
Every time the first message in a chain is sent, they need to generate a new key pair.

No Messages Whatsoever
----------------------------------------------------------------------------------------------------
If the chain keys table isn't populated with any of the conversations then return a pre key bundle. 
The client will then generate X3DH constant, a key pair and then will encrypt their message.

The client will return the encrypted message, the rachet private key, the rachet public key and the
public ephemeral key.

The server will then populate the X3DH handshake table. The "is next message in chain" flag should be
set to true after this. The other parties flag should be set to false.



Sending The First Message In A Chain
----------------------------------------------------------------------------------------------------
Send the client the chain key and the other parties public key.

The client should then generate a new key pair, and then generate the encrypted message. 

The client will return the encrypted message, and the private key.

The "is next message in chain" flag should be set to true after this. The other parties flag should
be set to false.


Sending A Message Within A Chain
----------------------------------------------------------------------------------------------------
Send the client the chain key and the sequence count.

The client will then encrypt the message.

The client will then return the encrypted message.



LOGIC FOR RECIEVING MESSAGES

Recieving The First Message Ever
----------------------------------------------------------------------------------------------------
Send the client the senders public X3DH values which consists of: the public identity key, the public
ephemeral key. Also, send the client the pre-key identifier and the time the message was sent.

The client will then decrypt the message and display it. After this, the client will then upload the 
the encrypted message to the server so that it can be stored in the old messages table.



Receiving The First Message In A Chain
----------------------------------------------------------------------------------------------------
Send the client the encrypted message, the private key, the chain key and the other parties public key.

The client will then decrypt the message and display it. The client will upload the new chain key to the
server. After this, the client will then upload the encrypted the encrypted message to the server so that
it can be stored in the old messages table.



Recieving A Message Within A Chain
----------------------------------------------------------------------------------------------------
Send the client the encrypted message, the chain key and the sequence count. 

The client will then decrypt the message and display it. The client will upload the new chain key to 
the server. After this, the client will then upload the encrypted the encrypted message to the server
so that it can be stored in the old messages table.





CREATE TABLE users (
    UserID INT AUTO_INCREMENT,
    Username VARCHAR(255) UNIQUE NOT NULL,
    AuthenticationCode VARCHAR(255),

    PRIMARY KEY (UserID)
); 

CREATE TABLE tokens (
    TokenID INT AUTO_INCREMENT,
    Token VARCHAR(255),
    ExpiryDate BIGINT,
    UserID INT,

    PRIMARY KEY (TokenID),
    FOREIGN KEY (UserID) REFERENCES users(UserID)
); 

CREATE TABLE managementrequests (
    RequestID INT AUTO_INCREMENT,
    RequestData TEXT,
    Recipient INT,

    PRIMARY KEY (RequestID),
    FOREIGN KEY (Recipient) REFERENCES users(UserID)
); 




CREATE TABLE handshakevalues(
    ValueID INT AUTO_INCREMENT,
    UserID INT,
    PublicIDkey VARCHAR(255),
    PublicSignedPreKey VARCHAR(255),
    PreKeySignature VARCHAR(255),

    PRIMARY KEY (ValueID),
    FOREIGN KEY (UserID) REFERENCES users(UserID)
);

CREATE TABLE privatekeys(
    PrivateKeyID INT AUTO_INCREMENT,
    UserID INT,
    PrivateIDkey VARCHAR(255),
    PrivateSignedPreKey VARCHAR(255),

    PRIMARY KEY (PrivateKeyID),
    FOREIGN KEY (UserID) REFERENCES users(UserID)
);



CREATE TABLE publicprekeys(
    PreKeyID INT AUTO_INCREMENT,
    UserID INT,
    Identifier VARCHAR(255),
    PreKey VARCHAR(255),

    PRIMARY KEY (PreKeyID),
    FOREIGN KEY (UserID) REFERENCES users(UserID)
);


CREATE TABLE privateprekeys(
    PrivatePreKeyID INT AUTO_INCREMENT,
    AssociatedPreKeyID INT,
    PreKey VARCHAR(255),

    PRIMARY KEY (PrivatePreKeyID),
    FOREIGN KEY (AssociatedPreKeyID) REFERENCES publicprekeys(PreKeyID)
);




SELECT Orders.OrderID, Customers.CustomerName, Orders.OrderDate FROM Orders INNER JOIN Customers ON Orders.CustomerID=Customers.CustomerID;

SELECT publicprekeys.Identifier, privateprekeys.PreKey FROM publicprekeys INNER JOIN privateprekeys ON privateprekeys.AssociatedPreKeyID=publicprekeys.PreKeyID WHERE publicprekeys.UserID = (SELECT UserID FROM Users WHERE username = 'Matthew')

SELECT publicprekeys.PreKey, publicprekeys.Identifier, handshakevalues.PublicIDkey, handshakevalues.PublicSignedPreKey, handshakevalues.PreKeySignature FROM publicprekeys INNER JOIN handshakevalues ON publicprekeys.UserID = handshakevalues.UserID WHERE publicprekeys.UserID = (SELECT UserID FROM Users WHERE username = @recipient_username) LIMIT 1;


SELECT chainkeys.PublicRatchetKey, publicprekeys.Identifier, x3dhhandshake.PublicEphemeralKey, handshakevalues.PublicIDkey FROM chainkeys INNER JOIN x3dhhandshake ON x3dhhandshake.ConversationID = chainkeys.ConversationID INNER JOIN publicprekeys ON publicprekeys.PreKeyID = x3dhhandshake.PreKeyID INNER JOIN conversations ON conversations.ConversationID = chainkeys.ConversationID INNER JOIN handshakevalues ON handshakevalues.UserID = conversations.Source  WHERE chainkeys.ConversationID = 23;

CREATE TABLE conversations(
    ConversationID INT AUTO_INCREMENT,
    Source INT,
    Destination INT,

    PRIMARY KEY (ConversationID),
    FOREIGN KEY (Source) REFERENCES users(UserID),
    FOREIGN KEY (Destination) REFERENCES users(UserID)
);

CREATE TABLE messages (
    MessageID INT AUTO_INCREMENT,
    SequenceCount INT,
    MessageData TEXT,
    TimeSent DATETIME,
    ConversationID INT,

    PRIMARY KEY (MessageID),
    FOREIGN KEY (ConversationID) REFERENCES conversations(ConversationID)
);

CREATE TABLE x3dhhandshake(
    HandshakeID INT AUTO_INCREMENT,
    PublicEphemeralKey VARCHAR(255),
    PreKeyID INT,
    ConversationID INT,

    PRIMARY KEY (HandshakeID),
    FOREIGN KEY (PreKeyID) REFERENCES publicprekeys(PreKeyID),
    FOREIGN KEY (ConversationID) REFERENCES conversations(ConversationID)
);

CREATE TABLE chainkeys(
    ChainKeyID INT AUTO_INCREMENT,
    PrivateChainKey VARCHAR(255),
    PrivateRatchetKey VARCHAR(255),
    PublicRatchetKey VARCHAR(255),
    IsNextMessageInAChain BOOL,
    ConversationID INT,

    PRIMARY KEY (ChainKeyID),
    FOREIGN KEY (ConversationID) REFERENCES conversations(ConversationID)
);

CREATE TABLE oldmessages(
    OldMessageID INT AUTO_INCREMENT,
    MessageData TEXT,
    TimeSent DATETIME,
    ConversationID INT,

    PRIMARY KEY (OldMessageID),
    FOREIGN KEY (ConversationID) REFERENCES conversations(ConversationID)
);




CREATE TABLE friendgraphnodes(
    NodeID INT AUTO_INCREMENT,
    UserID INT UNIQUE,

    PRIMARY KEY (NodeID),
    FOREIGN KEY (UserID) REFERENCES users(UserID)
);

CREATE TABLE friendgraphlinks(
    LinkID INT AUTO_INCREMENT,
    NodeID INT,
    FriendNodeID INT,

    PRIMARY KEY (LinkID),
    FOREIGN KEY (NodeID) REFERENCES friendgraphnodes(NodeID),
    FOREIGN KEY (FriendNodeID) REFERENCES friendgraphnodes(NodeID)
);




SELECT ConversationID FROM conversations WHERE Source = (SELECT UserID FROM users WHERE Username = 'Matthew') AND Destination = (SELECT UserID FROM users WHERE Username = 'Mick')

SELECT EXISTS(SELECT * FROM users WHERE Username = @username);


INSERT INTO messages (TimeSent) VALUES (SELECT CURRENT_TIMESTAMP)