-- --------------------------------------------------------
-- 호스트:                          localhost
-- 서버 버전:                        10.5.10-MariaDB - mariadb.org binary distribution
-- 서버 OS:                        Win64
-- HeidiSQL 버전:                  11.2.0.6213
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- embers 데이터베이스 구조 내보내기
CREATE DATABASE IF NOT EXISTS `embers` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `embers`;

-- 테이블 embers.account 구조 내보내기
CREATE TABLE IF NOT EXISTS `account` (
  `Account_id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(50) NOT NULL,
  `Password_hash` varchar(255) NOT NULL,
  `Password_salt` varchar(255) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Created_at` datetime DEFAULT current_timestamp(),
  `Last_login` datetime DEFAULT NULL,
  `Is_online` tinyint(1) DEFAULT 0,
  PRIMARY KEY (`Account_id`) USING BTREE,
  UNIQUE KEY `username` (`Username`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8;

-- 테이블 embers.character 구조 내보내기
CREATE TABLE IF NOT EXISTS `character` (
  `Character_id` int(11) NOT NULL AUTO_INCREMENT,
  `Account_id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Level` int(11) NOT NULL DEFAULT 1,
  `Faction` enum('HOPE','FIRE') NOT NULL DEFAULT 'HOPE',
  `Hp` int(11) NOT NULL DEFAULT 500,
  `MaxHp` int(11) NOT NULL DEFAULT 500,
  `Mp` int(11) NOT NULL DEFAULT 250,
  `MaxMp` int(11) NOT NULL DEFAULT 250,
  `Hxp` int(11) NOT NULL DEFAULT 0,
  `Attack` int(11) NOT NULL DEFAULT 10,
  `Armor` int(11) NOT NULL DEFAULT 10,
  `Class` enum('WARRIOR','MAGE','ROGUE') NOT NULL DEFAULT 'WARRIOR',
  `Gender` enum('MALE','FEMALE') NOT NULL DEFAULT 'MALE',
  `Sp` int(11) NOT NULL DEFAULT 3,
  `Current_position_x` float NOT NULL DEFAULT 33,
  `Current_position_y` float NOT NULL DEFAULT 7.5,
  `Current_position_z` float NOT NULL DEFAULT 36,
  `MapCode` int(11) DEFAULT 0,
  `Gold` int(11) NOT NULL DEFAULT 0,
  `InventorySpace` int(11) NOT NULL DEFAULT 20,
  `Created_at` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`Character_id`) USING BTREE,
  UNIQUE KEY `unique_character_name` (`Name`) USING BTREE,
  KEY `account_id` (`Account_id`) USING BTREE,
  CONSTRAINT `character_ibfk_1` FOREIGN KEY (`Account_id`) REFERENCES `account` (`Account_id`)
) ENGINE=InnoDB AUTO_INCREMENT=149 DEFAULT CHARSET=utf8;

-- 테이블 embers.inventory 구조 내보내기
CREATE TABLE IF NOT EXISTS `inventory` (
  `Inventory_id` int(11) NOT NULL AUTO_INCREMENT,
  `Character_id` int(11) NOT NULL,
  `Item_id` int(11) NOT NULL,
  `Position` int(11) NOT NULL,
  `Amount` int(11) DEFAULT 1,
  PRIMARY KEY (`Inventory_id`) USING BTREE,
  KEY `character_id` (`Character_id`) USING BTREE,
  CONSTRAINT `inventory_ibfk_1` FOREIGN KEY (`Character_id`) REFERENCES `character` (`Character_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=728 DEFAULT CHARSET=utf8;

-- 테이블 embers.skill 구조 내보내기
CREATE TABLE IF NOT EXISTS `skill` (
  `Skill_table_id` int(11) NOT NULL AUTO_INCREMENT,
  `Character_id` int(11) NOT NULL,
  `Skill_id` int(11) NOT NULL,
  `Level` int(11) NOT NULL DEFAULT 1,
  PRIMARY KEY (`Skill_table_id`) USING BTREE,
  UNIQUE KEY `unique_character_skill` (`Character_id`,`Skill_id`) USING BTREE,
  CONSTRAINT `skill_ibfk_1` FOREIGN KEY (`Character_id`) REFERENCES `character` (`Character_id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=95 DEFAULT CHARSET=utf8;
