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




CREATE TABLE friendgraphlinks(
    LinkID INT AUTO_INCREMENT,
    NodeID INT,
    FriendNodeID INT,

    PRIMARY KEY (LinkID),
    FOREIGN KEY (NodeID) REFERENCES users(UserID),
    FOREIGN KEY (FriendNodeID) REFERENCES users(UserID)
);

CREATE TABLE friendrequests(
    RequestID INT AUTO_INCREMENT,
    Sender INT,
    Recipient INT,

    PRIMARY KEY (RequestID),
    FOREIGN KEY (Sender) REFERENCES users(UserID),
    FOREIGN KEY (Recipient) REFERENCES users(UserID)
);


SELECT ConversationID FROM conversations WHERE Source = (SELECT UserID FROM users WHERE Username = 'Matthew') AND Destination = (SELECT UserID FROM users WHERE Username = 'Mick')

SELECT EXISTS(SELECT * FROM users WHERE Username = @username);


INSERT INTO messages (TimeSent) VALUES (SELECT CURRENT_TIMESTAMP)