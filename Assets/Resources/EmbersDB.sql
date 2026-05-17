CREATE DATABASE IF NOT EXISTS `embers` /*!40100 DEFAULT CHARACTER SET utf8mb3 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `embers`;

CREATE TABLE IF NOT EXISTS `account` (
  `Account_id` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) NOT NULL,
  `Password_hash` varchar(255) NOT NULL,
  `Password_salt` varchar(255) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `Last_login` datetime DEFAULT NULL,
  `Is_online` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`Account_id`) USING BTREE,
  UNIQUE KEY `username` (`Username`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb3;

CREATE TABLE IF NOT EXISTS `character` (
  `Character_id` int NOT NULL AUTO_INCREMENT,
  `Account_id` int NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Level` int NOT NULL DEFAULT '1',
  `Faction` enum('HOPE','FIRE') NOT NULL DEFAULT 'HOPE',
  `Hp` int NOT NULL DEFAULT '500',
  `MaxHp` int NOT NULL DEFAULT '500',
  `Mp` int NOT NULL DEFAULT '250',
  `MaxMp` int NOT NULL DEFAULT '250',
  `Hxp` int NOT NULL DEFAULT '0',
  `Attack` int NOT NULL DEFAULT '10',
  `Armor` int NOT NULL DEFAULT '10',
  `Class` enum('WARRIOR','MAGE','ROGUE') NOT NULL DEFAULT 'WARRIOR',
  `Gender` enum('MALE','FEMALE') NOT NULL DEFAULT 'MALE',
  `Sp` int NOT NULL DEFAULT '3',
  `Current_position_x` float NOT NULL DEFAULT '33',
  `Current_position_y` float NOT NULL DEFAULT '7.5',
  `Current_position_z` float NOT NULL DEFAULT '36',
  `MapCode` int DEFAULT '0',
  `Gold` int NOT NULL DEFAULT '0',
  `InventorySpace` int NOT NULL DEFAULT '20',
  `Created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `EquippedWeaponPosition` int NOT NULL DEFAULT '-1',
  `EquippedArmorPosition` int NOT NULL DEFAULT '-1',
  PRIMARY KEY (`Character_id`) USING BTREE,
  UNIQUE KEY `unique_character_name` (`Name`) USING BTREE,
  KEY `account_id` (`Account_id`) USING BTREE,
  CONSTRAINT `character_ibfk_1` FOREIGN KEY (`Account_id`) REFERENCES `account` (`Account_id`)
) ENGINE=InnoDB AUTO_INCREMENT=153 DEFAULT CHARSET=utf8mb3;

CREATE TABLE IF NOT EXISTS `inventory` (
  `Inventory_id` int NOT NULL AUTO_INCREMENT,
  `Character_id` int NOT NULL,
  `Item_id` int NOT NULL,
  `Position` int NOT NULL,
  `Amount` int DEFAULT '1',
  PRIMARY KEY (`Inventory_id`) USING BTREE,
  KEY `character_id` (`Character_id`) USING BTREE,
  CONSTRAINT `inventory_ibfk_1` FOREIGN KEY (`Character_id`) REFERENCES `character` (`Character_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=754 DEFAULT CHARSET=utf8mb3;


CREATE TABLE IF NOT EXISTS `skill` (
  `Skill_table_id` int NOT NULL AUTO_INCREMENT,
  `Character_id` int NOT NULL,
  `Skill_id` int NOT NULL,
  `Level` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`Skill_table_id`) USING BTREE,
  UNIQUE KEY `unique_character_skill` (`Character_id`,`Skill_id`) USING BTREE,
  CONSTRAINT `skill_ibfk_1` FOREIGN KEY (`Character_id`) REFERENCES `character` (`Character_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=98 DEFAULT CHARSET=utf8mb3;
