-- MySQL dump 10.13  Distrib 8.0.28, for Win64 (x86_64)
--
-- Host: localhost    Database: NEA
-- ------------------------------------------------------
-- Server version	8.0.28

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `chainkeys`
--

DROP TABLE IF EXISTS `chainkeys`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chainkeys` (
  `ChainKeyID` int NOT NULL AUTO_INCREMENT,
  `PrivateChainKey` varchar(255) DEFAULT NULL,
  `PrivateRatchetKey` varchar(255) DEFAULT NULL,
  `PublicRatchetKey` varchar(255) DEFAULT NULL,
  `IsNextMessageInAChain` tinyint(1) DEFAULT NULL,
  `ConversationID` int DEFAULT NULL,
  `CurrentSequenceCount` int DEFAULT NULL,
  PRIMARY KEY (`ChainKeyID`),
  KEY `ConversationID` (`ConversationID`),
  CONSTRAINT `chainkeys_ibfk_1` FOREIGN KEY (`ConversationID`) REFERENCES `conversations` (`ConversationID`)
) ENGINE=InnoDB AUTO_INCREMENT=172 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chainkeys`
--

LOCK TABLES `chainkeys` WRITE;
/*!40000 ALTER TABLE `chainkeys` DISABLE KEYS */;
INSERT INTO `chainkeys` VALUES (170,'+fAG1knnBkf9Ks1ka7BrP9SibHczCmzX7/IwDhZGWEIog2hQJx32Qa/fDSqjUrCvoh6mh8BfW7YZnB69EV1b0g==','8PAsj0/dBna7KPZsV+NpRICvGyhuWlSn25QEdE9LKFFTzRlnPT3xcqO0WyI=','YPuJ1YtbLkMXIIPRL6viHkjxTrtugGIKE8IbrJs5NzMB',1,204,2),(171,'B1N9zZveutNizSIKT960qYSYUtG21XacgmU5TEwQl0MbvSoS1ilaFWWvu3l9pFPcraOnNk72LAG/MyULWqaZ0g==','cWxWm5uzu/Q/jRReQrLJ9PWDForTiVmrhxkcLDADkk5b6nJmzz8vHlWi6Q0=','EvyYpp8G8nMm6ES53igQhz+YJWYmLdtR+nKuAKgggNgB',0,205,1);
/*!40000 ALTER TABLE `chainkeys` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `conversations`
--

DROP TABLE IF EXISTS `conversations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `conversations` (
  `ConversationID` int NOT NULL AUTO_INCREMENT,
  `Source` int DEFAULT NULL,
  `Destination` int DEFAULT NULL,
  PRIMARY KEY (`ConversationID`),
  KEY `Source` (`Source`),
  KEY `Destination` (`Destination`),
  CONSTRAINT `conversations_ibfk_1` FOREIGN KEY (`Source`) REFERENCES `users` (`UserID`),
  CONSTRAINT `conversations_ibfk_2` FOREIGN KEY (`Destination`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=206 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `conversations`
--

LOCK TABLES `conversations` WRITE;
/*!40000 ALTER TABLE `conversations` DISABLE KEYS */;
INSERT INTO `conversations` VALUES (204,33,31),(205,31,33);
/*!40000 ALTER TABLE `conversations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `handshakevalues`
--

DROP TABLE IF EXISTS `handshakevalues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `handshakevalues` (
  `ValueID` int NOT NULL AUTO_INCREMENT,
  `UserID` int DEFAULT NULL,
  `PublicIDkey` varchar(255) DEFAULT NULL,
  `PublicSignedPreKey` varchar(255) DEFAULT NULL,
  `PreKeySignature` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ValueID`),
  KEY `UserID` (`UserID`),
  CONSTRAINT `handshakevalues_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `handshakevalues`
--

LOCK TABLES `handshakevalues` WRITE;
/*!40000 ALTER TABLE `handshakevalues` DISABLE KEYS */;
INSERT INTO `handshakevalues` VALUES (15,31,'TmuYQ1/3GQtVyei5L/HoPYMiYUkVv7SP4rRr7yTsOLgA','ls9oju6S1camw+0KXWqUQegDppIC8ju2IkLIlupc1akA','I3ie9IeBHrY4eNDgcbsnPzJNmm66as9AH8SQImcJn4AbfN2fUa7BDK4IEquX4BAbSLPwpRoj63pcI9zGSN4A3Q=='),(16,32,'guZS7JqbHAo5ZQkpGCTlkh2E5gH/w3oAIAky87wTl3kB','ligghDaQxfHAjN9ZJ0eRFStyZ0D+2s+JfS06EoQA+WQB','LA824V20WTGoWKGECCJGgEh8+6/RzRSX5RQVDX6/nk0q/VrhEhF7MJeNtuFyqCzJAeBR0zpgJBrHVUzFCwKYxw=='),(17,33,'yEVon3bafce5LtQDd/plxLQyivvcPoIo1DqVYoD+Ya8A','3zcX0vVCpIwar44py6saNBvzzqaFsU8iSfigaV4VvOMA','lyZX2uZta8//ykNEkyx8qHgNWd+iKy6uvZ/sPKdlLNhApqsw3TYbmPmHH5onb1UVu4qixKye1YruKhdeBD7D+g==');
/*!40000 ALTER TABLE `handshakevalues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `managementrequests`
--

DROP TABLE IF EXISTS `managementrequests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `managementrequests` (
  `RequestID` int NOT NULL AUTO_INCREMENT,
  `RequestData` text,
  `Recipient` int DEFAULT NULL,
  PRIMARY KEY (`RequestID`),
  KEY `Recipient` (`Recipient`),
  CONSTRAINT `managementrequests_ibfk_1` FOREIGN KEY (`Recipient`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=98 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `managementrequests`
--

LOCK TABLES `managementrequests` WRITE;
/*!40000 ALTER TABLE `managementrequests` DISABLE KEYS */;
/*!40000 ALTER TABLE `managementrequests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `messages`
--

DROP TABLE IF EXISTS `messages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `messages` (
  `MessageID` int NOT NULL AUTO_INCREMENT,
  `SequenceCount` int DEFAULT NULL,
  `MessageData` text,
  `TimeSent` datetime DEFAULT NULL,
  `ConversationID` int DEFAULT NULL,
  `MessageType` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`MessageID`),
  KEY `ConversationID` (`ConversationID`),
  CONSTRAINT `messages_ibfk_1` FOREIGN KEY (`ConversationID`) REFERENCES `conversations` (`ConversationID`)
) ENGINE=InnoDB AUTO_INCREMENT=194 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `messages`
--

LOCK TABLES `messages` WRITE;
/*!40000 ALTER TABLE `messages` DISABLE KEYS */;
/*!40000 ALTER TABLE `messages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `oldmessages`
--

DROP TABLE IF EXISTS `oldmessages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `oldmessages` (
  `OldMessageID` int NOT NULL AUTO_INCREMENT,
  `MessageData` text,
  `TimeSent` datetime DEFAULT NULL,
  `ConversationID` int DEFAULT NULL,
  `Sender` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`OldMessageID`),
  KEY `ConversationID` (`ConversationID`),
  CONSTRAINT `oldmessages_ibfk_1` FOREIGN KEY (`ConversationID`) REFERENCES `conversations` (`ConversationID`)
) ENGINE=InnoDB AUTO_INCREMENT=194 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `oldmessages`
--

LOCK TABLES `oldmessages` WRITE;
/*!40000 ALTER TABLE `oldmessages` DISABLE KEYS */;
INSERT INTO `oldmessages` VALUES (186,'0NAsixQ=','2022-09-12 17:58:26',204,'Dave'),(187,'LnNXkMY=','2022-09-12 17:58:25',205,'Dave'),(188,'P3k=','2022-09-12 17:58:33',205,'Matthew'),(189,'wdo=','2022-09-12 17:58:32',204,'Matthew'),(190,'3toyilXsWXeidet1BqF+N9i2','2022-09-12 18:43:48',204,'Dave'),(191,'IHlJkYfV5eM9kgQbIs+hoYiM','2022-09-12 18:43:47',205,'Dave'),(192,'3toyilXsWXeidet1BqF+N9i2','2022-09-12 18:43:58',204,'Dave'),(193,'IHlJkYfV5eM9kgQbIs+hoYiM','2022-09-12 18:43:57',205,'Dave');
/*!40000 ALTER TABLE `oldmessages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `privatekeys`
--

DROP TABLE IF EXISTS `privatekeys`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `privatekeys` (
  `PrivateKeyID` int NOT NULL AUTO_INCREMENT,
  `UserID` int DEFAULT NULL,
  `PrivateIDkey` varchar(255) DEFAULT NULL,
  `PrivateSignedPreKey` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`PrivateKeyID`),
  KEY `UserID` (`UserID`),
  CONSTRAINT `privatekeys_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `privatekeys`
--

LOCK TABLES `privatekeys` WRITE;
/*!40000 ALTER TABLE `privatekeys` DISABLE KEYS */;
INSERT INTO `privatekeys` VALUES (15,31,'F0dfzdnQ3+VmlgpWYIDN+sPfEteviwCqlDUVOQ0G1Ddj+lMDoQEbM0+hug0=','EXwIqpjo4dARtDkDbdPT1PL6FYKzglu8kj05T0If6C9J4y87/BgLHVf533E='),(16,32,'xpK6wOKh1kRy8d9D5yJ0qS867IfJ+8bzSA70iStgm1OA0UXwl3GZkQBY8Co=','9Prcz5WswGVxuIRm0RxA7gBh+rPwzJjrWjyZpFxalTOPiyqFpS6myitd/Co='),(17,33,'6eN4ojLxZ3C1f+IfZMdvcI3VFj83RlvinZI5TGtBLUlpgzdaMiSmd4yiTyI=','l8UMrhPUQlT7NespXs4VIbb1DhFXbWLK7JATd2N5XWpH/BheIiSFM4CTeV4=');
/*!40000 ALTER TABLE `privatekeys` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `privateprekeys`
--

DROP TABLE IF EXISTS `privateprekeys`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `privateprekeys` (
  `PrivatePreKeyID` int NOT NULL AUTO_INCREMENT,
  `AssociatedPreKeyID` int DEFAULT NULL,
  `PreKey` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`PrivatePreKeyID`),
  KEY `AssociatedPreKeyID` (`AssociatedPreKeyID`),
  CONSTRAINT `privateprekeys_ibfk_1` FOREIGN KEY (`AssociatedPreKeyID`) REFERENCES `publicprekeys` (`PreKeyID`)
) ENGINE=InnoDB AUTO_INCREMENT=42 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `privateprekeys`
--

LOCK TABLES `privateprekeys` WRITE;
/*!40000 ALTER TABLE `privateprekeys` DISABLE KEYS */;
INSERT INTO `privateprekeys` VALUES (27,28,'HCBUkZzV6toZnSxlULbj1+beIYvUjHX3ugYbDBc4mksazGx44FoLJWfiww0='),(28,29,'F2dBzsPGvP0TmwNgZ7/yqoHGN6fr2H+8hyEeSjMBxjUY3mAD2yoCYlPY+Q0='),(29,30,'PCJ9vcLE2KUovRNAb7/Z7PSDKqzC1G7pjmkUP0kM0GVcyWog5lEFM0bG5XE='),(30,31,'LUNLr9Gpx6YIvDF1fLb19YXuW9jBo0S9uQUdQykzjFMF2E0RzCwMF0TS/XE='),(31,32,'PFRciYKsxKYDyAtlebXR1PX9CKT+gUW98zQtEzwzxFYWvlwyoV0nOni8zw0='),(32,33,'9pnC++ag/jJPlad+tCZ8tCM32fbW98HtSijlh0BdgTSd93/3nCW2wyB7+FY='),(33,34,'3I/ExPyezWZT+btn9Qtqo19xwK356sOJU27AoRR+pwmy4zLrg3TrkTVu1lY='),(34,35,'7sTR6vWv4WRvtIcitDNor11U0a3Zq8fuCRb4mCtyiC20yFqMmy6F1jhqo1Y='),(35,36,'uOXBwcyO9HBllIRz2RxCmlhnjZKwq4LvDzDYqgFglVe89S/cmESy6C4cxio='),(36,37,'/M2988Cu8E9MiNV6ySRYkFs32vfXycKRCznqgjVduFSj4kj4/Dio9jZfoyo='),(37,38,'88EOkTriZkqvee83SP5tco3eMgp1XRv82741dBVLNVtt5mJSCxCYbt2eDCI='),(38,39,'0dgDsUOSY2SfbOQpWsowc9TEPwxoSBX85vUUf0RCAng2+3V4A2n4YZ2IYV4='),(39,40,'0tYapk7UYkL7b+oRW8AvY4DQORR7Blzqyf40c1VHCXh+gm1VUmy2bdm0cSI='),(40,41,'7/4ioDKUW0CJLc8QYOpuaIrEORBlRkjJ26M5dndoGHFNjTFrCxGwRaWpdV4='),(41,42,'zORr3gnqei6dZvoXYtoUQ9G9Bg9jCV7n+4ggAFVYPTRWhDBLKR6YMbipCF4=');
/*!40000 ALTER TABLE `privateprekeys` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `publicprekeys`
--

DROP TABLE IF EXISTS `publicprekeys`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `publicprekeys` (
  `PreKeyID` int NOT NULL AUTO_INCREMENT,
  `UserID` int DEFAULT NULL,
  `Identifier` varchar(255) DEFAULT NULL,
  `PreKey` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`PreKeyID`),
  KEY `UserID` (`UserID`),
  CONSTRAINT `publicprekeys_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=43 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `publicprekeys`
--

LOCK TABLES `publicprekeys` WRITE;
/*!40000 ALTER TABLE `publicprekeys` DISABLE KEYS */;
INSERT INTO `publicprekeys` VALUES (28,31,'459c6ce7-85e6-46da-8990-54844aaf2baa','1WewH5CGytUZtWRIFbyMpwaD/hGrV5isYr/hV8dOetAB'),(29,31,'92250476-ef0a-483a-a965-0ec9d62df9d8','oC+ussSg0GHhB6fp/8wQEXkedsNPjnPCsYiHI1Ca54UB'),(30,31,'a85f83a4-77c1-4688-9a75-02ccdd14720c','M+JNGTH5LE7A0xIpsiScrW4jj2a01iBgVj2I53lYbFYB'),(31,31,'6cef33af-040d-4662-9844-65c0c9bc7a61','zJSSG510+dSwpsHX8kqDUutIBXhhEiv7PePe4YyLnBkA'),(32,31,'433e9f3b-7127-4862-9b72-6ecb3862a9d5','prkMC018KhL2oxpnu+RByl4OAK1myn2ZmmNFVmRVZ7wB'),(33,32,'5677c1c8-c58e-43f0-8415-4d4eb08c2fa5','6Td+OR0GFRqKxS36C6QucjYcU/PY/YF7rQCUF8wyflIB'),(34,32,'24b52aa5-ab7f-4f49-a0db-2e324ec1e1f2','4w0cJV959zHnEbe6lYMa+jhxVbrYdlyA0kbdjDxXLQMB'),(35,32,'3cef44dd-3337-42c9-90f5-8d2a1b9c8ff5','wKu6tEtHlNgIlg0vhNkDcg73c2LUEfIl0YVv6EXbeYsA'),(36,32,'78c0010f-19d2-40c2-acc0-db5c6d2ca6af','qVVxeCCUmF8W/127TtIGNA3jXDmxqfCwv6+tO35Ox9oA'),(37,32,'fa739c4d-e65e-4042-a228-8d269fbbb01c','l6OF8/CXXouMZqZcFFAngCkjTgCSekmh3gMRhLRyWqcA'),(38,33,'aab3f872-0dab-4954-abe7-2c7353d81efe','5vLhI0yTrpp/J0wKLwIsDINLNSUFulnx9yz+bitHcn8A'),(39,33,'49b021a6-bb18-431a-a56f-d73cab9ec804','QoIp1OZqpQ2elUYDpX1XwQ5qQouPL6pQBQnT2isT44EB'),(40,33,'da8bd4f9-5ec7-497d-a3fb-33e21e245a21','Qzujvp+a1Qw9Yjg8FUnMOiHuKEqzDmZQ0IZj7M+KUKwA'),(41,33,'7d5a5caa-49aa-4267-a41e-bab5c92f008a','0+Oo2h7agjA3wR2oHw9K7TRA2U/MWLTClzJxOtIPNawA'),(42,33,'9645421e-aeca-462d-93d0-fac35a3b6b60','eSdn1uPnMUi6rF0EO085csfxg99l+FRH5Clt0u8yWUMB');
/*!40000 ALTER TABLE `publicprekeys` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tokens`
--

DROP TABLE IF EXISTS `tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tokens` (
  `TokenID` int NOT NULL AUTO_INCREMENT,
  `Token` varchar(255) DEFAULT NULL,
  `ExpiryDate` bigint DEFAULT NULL,
  `UserID` int DEFAULT NULL,
  PRIMARY KEY (`TokenID`),
  KEY `UserID` (`UserID`),
  CONSTRAINT `tokens_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tokens`
--

LOCK TABLES `tokens` WRITE;
/*!40000 ALTER TABLE `tokens` DISABLE KEYS */;
INSERT INTO `tokens` VALUES (23,'d290dbc3-efaf-452d-a754-d45a6c51ab69',1663088273,31),(24,'cdf5df56-ef65-4d57-bed9-63713adf7911',1663084889,32),(25,'9663be96-a287-4e93-b255-14e1ed0de62c',1663088294,33);
/*!40000 ALTER TABLE `tokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `UserID` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(255) NOT NULL,
  `AuthenticationCode` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `Username` (`Username`)
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (31,'Matthew','BC1FDC2A4ABFDC349E7048A8CFD24F526ABB0597C6A7182F32C7F5BDD8C14C4FoWtALZo='),(32,'Mick','AB3B3DA5FA59F7D2AB3FA0F5FB21C86C13C5D80E74B6F5BFED9DE3C51A1C4BD5C0GiQdA='),(33,'Dave','DE8A8A8491EA120E39DDEBBDFDE5D0B2B00CE4561741EA7B275304B2912C1721o/EmfZI=');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `x3dhhandshake`
--

DROP TABLE IF EXISTS `x3dhhandshake`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `x3dhhandshake` (
  `HandshakeID` int NOT NULL AUTO_INCREMENT,
  `PublicEphemeralKey` varchar(255) DEFAULT NULL,
  `PreKeyID` int DEFAULT NULL,
  `ConversationID` int DEFAULT NULL,
  PRIMARY KEY (`HandshakeID`),
  KEY `PreKeyID` (`PreKeyID`),
  KEY `ConversationID` (`ConversationID`),
  CONSTRAINT `x3dhhandshake_ibfk_1` FOREIGN KEY (`PreKeyID`) REFERENCES `publicprekeys` (`PreKeyID`),
  CONSTRAINT `x3dhhandshake_ibfk_2` FOREIGN KEY (`ConversationID`) REFERENCES `conversations` (`ConversationID`)
) ENGINE=InnoDB AUTO_INCREMENT=94 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `x3dhhandshake`
--

LOCK TABLES `x3dhhandshake` WRITE;
/*!40000 ALTER TABLE `x3dhhandshake` DISABLE KEYS */;
INSERT INTO `x3dhhandshake` VALUES (93,'0cFboz+GwsYu2pZUWz5uUdNdy0tyIa+RsuLgmA6ti+oB',28,204);
/*!40000 ALTER TABLE `x3dhhandshake` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-09-12 20:24:13
