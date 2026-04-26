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

-- 테이블 데이터 embers.account:~22 rows (대략적) 내보내기
/*!40000 ALTER TABLE `account` DISABLE KEYS */;
REPLACE INTO `account` (`Account_id`, `Username`, `Password_hash`, `Password_salt`, `Email`, `Created_at`, `Last_login`, `Is_online`) VALUES
	(7, 'admin', 'Zkhe1cJ0VMN/msd2sJdjcfZ/ehCYImXAP/jRCAlkZHg=', '8ynDNK4LEpTQsYfFy4tJ33V5zsA09KLDCEXpPqzfKoU=', 'admin@naver.com', '2024-12-28 20:28:10', NULL, 1),
	(8, 'admin2', 'adU76Ed9zVnEtN+shiYF8OGiHxvCGJ3ETItPEur0XWg=', 'nCg/LMHira9DFAvt9AMnJNV0wgURyVAWRUGDR4dvsNA=', 'admin@naver.com', '2024-12-28 20:28:15', NULL, 1),
	(9, 'admin3', '74gLi3tTYXVrztGA+l27GVjOSLsJDay4P45lOzbXJL8=', 'sWQ3W3aCLVdKmSo2piukgpMM6KKlu38AoAJxPz+flRs=', 'admda', '2024-12-28 20:30:22', NULL, 0),
	(10, 'asd', 'Tyb7t8I+nfmx0A+YDxbPu/lZt/NfrEpvFKZc3mSpA50=', 'qgu1w0giRvzwWARrxxjM52MTDD+TR92rXeHVKv6D4Gc=', 'asd', '2024-12-28 21:04:58', NULL, 1),
	(11, 'admin33', 'mpCDhUB3LY4x5j4K6xfJA8n/62b06D8j6wnfCfE7KUQ=', 'v6cYL6dXtMQ7vZByIkQNdIHZnwyfBSymAbqtOvkjE4Q=', 'zzz', '2024-12-28 21:17:33', NULL, 1),
	(12, 'holy', 'Jqhz0+X+kNpX4DAPm7VrvwuQ85Ct5MrScR4xvqFd4jw=', 'N95IvasJZn6Iay2VKCboo51MocqLla24kb5JCEleN9w=', 'jloy@dlavc.com', '2024-12-29 01:18:20', NULL, 0),
	(13, 'admin23', 'ld8Zkl6MVo+XgOXNWutBIxo4XqDX/EpVDl4hkaMA18o=', 'Ta5CNaicqgrDK/NXlDnydlSOgnU2c7g9IZGvKCW32x8=', 'asdasd@nasdf.com', '2024-12-29 03:09:54', NULL, 1),
	(14, 'admin5', '9l10urxjd5Lqo5YD1JDyyj5yDPFcVuJJRcSmiuTzlBo=', 'ktwXNjSoc1Gx8BDVaheT8KrmhXbuDWgqHY23UFhRLuk=', 'ss', '2024-12-29 03:22:57', NULL, 0),
	(15, 'admin8', '7Ivs5Qffcmu0WVIhmaqia8hw1dEPwvEWTIW2o6vDsAA=', '0cwtsgU7eG2rOhhE/jX11Th3fRlwbcQqnyHiLzcCbVs=', 'adddd', '2024-12-29 03:28:06', NULL, 1),
	(16, 'admin6', 't/vY4+PE70saCxJcsxEXeJT2X0jfA+baJtx6ez0zHIw=', 'A9JLr+YRmTsUXXuoHdDALzWCICLo28UBDJWZHZsocts=', 'ddd', '2024-12-29 04:16:30', NULL, 1),
	(17, '', 'gHxraBZP/VqUE7l5QYGLS7CLOTZpfMUYL8RauqZamqA=', 'rh+KnhJgr3nn9xoHrJRNQ8WxAey2rH3/C/pEwPzsCJs=', 'asdasdasd', '2024-12-29 14:04:11', NULL, 0),
	(18, 'dd', 'f96Kd/gQxsQC77K5PKEfPXzaIKPpobFQGbvwmEOIi9c=', 'jZtoruXwIdNkF2ooSsTdcCCKe/YqcSGF+Tkd2UdBUoY=', '', '2024-12-29 14:11:05', NULL, 1),
	(19, 'admin65', 'GzLRgin3lzcw/dc/YvN285RAt6/AhIv/iTXip1mfkks=', 'UcSDS3cuwwIxZs6W1k3MWu5Khjnc4aVWY/RrYo8cc8c=', 'dasdasd', '2024-12-29 18:21:49', NULL, 1),
	(20, 'admin77', 'VWupgIBCzZ0z+GBSqYQ1T4j2r/ymOhBkecXVTCxcQvE=', 'AiK3BNqqmLzi9rKqP+HJib7acWv+F+82KN1zqhdYoMU=', '', '2024-12-29 18:22:18', NULL, 1),
	(21, 'admin64', 'X9mz4Ounjrs0/vN3dsCeU69emYxiXCJ3jOdIxV0+Z/U=', 'twLt4xG1oKw5rEl1fdNep1bLWVXw3IL1+fjtn9YHjc8=', 'asdasd@naver.com', '2024-12-29 20:09:29', NULL, 1),
	(22, 'tlrmsjtm77', 'vTklbXqnsCnldHDz/oAfEicvrcB2gCjOZTLdAL/i5NE=', 'CJZbGg7halWjh4kXbkrON+XXr8p96hfX+1BKo9sOb/w=', '2dasd@naver.com', '2024-12-30 14:51:30', NULL, 1),
	(23, 'zxcfcv', 'ZIYjS9NzioRWyBQl2ejGFEh8bqsRz5rED9N/9P+llfo=', 'l99VlhS56R6Ux+jXGKxZa6Id4Kqm1wFge8EXptx/1oc=', 'sdsd', '2024-12-31 16:02:20', NULL, 1),
	(24, 'sssd', 'dSKmuMDEh1dPiVFD5hfgQRJAsygejtTdI50lR07HVlU=', 'qgU5yh74CdkRjZJg5IAZ4pMQB/FsbK7Mw/WXx46bmkk=', '', '2024-12-31 23:58:01', NULL, 0),
	(25, 'sssd2', 'kHvQk5RDjGlYCYvOyi3l/AVnB7+j2VgET0phYyp33qI=', 'Vip39f7Q5458baffbvVFKxguqOEpdIgaL7b/auJedIo=', '', '2024-12-31 23:58:09', NULL, 1),
	(26, 'admin11', 'n0xGUxZMun69r3ZBtXFZmZN/Hgj9X5ObCeconGOI824=', 'b8rAnjHdlAR/Qkafs18YUmH0/6TbDvAsEHUyuKkZh9U=', 'asdasd@naver.com', '2025-01-01 23:29:48', NULL, 1),
	(27, 'dfdfsdf', 'gfHvtamgDMJHiTud4gLMLwIs4bTipqKoXEzY3mSVX6w=', 'DK0l1korB5wyhwTP09qxOqheBEFTHzQ0QLPg3OCs7fA=', '', '2025-01-04 22:03:34', NULL, 0),
	(28, 'test3', '9PHpT2M2t5GV148X7y/ap6JtJgj9Hogpj15Z3GJHHZc=', 'odWpYULX9/xK6Pym0w8Um9WXP/lfHNRJpF/cu7HsyDQ=', '', '2025-01-12 06:12:19', NULL, 1),
	(29, 'ddddd', 'tEyZeecrJfWqh9gkFD5x3+/OZSqvclC/6ns6Q97PY9U=', 'Pct84udTiCciPoR1gcxuQJTp2c96YqX1lBalbgTQC3g=', 'asdasd@naver.com', '2025-01-13 22:39:12', NULL, 1),
	(30, 'asdfg', 'M/txgKpcvQ07geazl+/AMSTnvIPdzq34bho2sJ16kAs=', 'lwK73iGmk7jMeYOx1zlOB08hkrsKcTc0DIuPVnnoM3E=', '', '2025-01-17 00:34:08', NULL, 1),
	(31, 'sdsdsdd', 'EcvNbKIt5Gt1lLzGOAnz18E+u14EzXrs83LlcISW1jk=', 'Bogjeh38oIV4k9QGzvfmRGma5ZxWAY8strMaMuyk9I4=', '', '2025-03-02 18:03:35', NULL, 0),
	(32, 'admin66', 'xjHcIqQnGiM81G4N3JbpADQcbj0OYe/jO/qUF9gItC4=', 'q6UEvnvPHYLaLb1JoHflM1Hjnuowqedj3pWNafbXrgI=', '', '2025-08-17 16:05:20', NULL, 1),
	(33, 'admin667', 'hupPr58tCkwRiIe3ATy7QhgDFnkCUB9TOVUk4WKT2go=', 'zYgukTZUo1XJf14VV1q/LjqECc0W73akLKW7pXz4mHI=', '', '2025-08-17 16:17:15', NULL, 0),
	(34, 'admin7', '/HukeavIfN4c7pLNyyLP6twHGHaYGWT7JKpGG74MDio=', 'Rxjs9ye3YRWYoX18NEDglboK6RBXMTvLHYqr9uD+3Yo=', '', '2025-10-25 23:01:33', NULL, 1);
/*!40000 ALTER TABLE `account` ENABLE KEYS */;

-- 테이블 embers.character 구조 내보내기
CREATE TABLE IF NOT EXISTS `character` (
  `Character_id` int(11) NOT NULL AUTO_INCREMENT,
  `Account_id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Level` int(11) NOT NULL DEFAULT 1,
  `Faction` enum('HOPE','FIRE') NOT NULL DEFAULT 'HOPE',
  `Hp` int(11) NOT NULL DEFAULT 100,
  `MaxHp` int(11) NOT NULL DEFAULT 100,
  `Mp` int(11) NOT NULL DEFAULT 50,
  `MaxMp` int(11) NOT NULL DEFAULT 50,
  `Hxp` int(11) NOT NULL DEFAULT 0,
  `Attack` int(11) NOT NULL DEFAULT 10,
  `Armor` int(11) NOT NULL DEFAULT 10,
  `Class` enum('WARRIOR','MAGE','ROGUE') NOT NULL DEFAULT 'WARRIOR',
  `Gender` enum('MALE','FEMALE') NOT NULL DEFAULT 'MALE',
  `Sp` int(11) NOT NULL DEFAULT 0,
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

-- 테이블 데이터 embers.character:~10 rows (대략적) 내보내기
/*!40000 ALTER TABLE `character` DISABLE KEYS */;
REPLACE INTO `character` (`Character_id`, `Account_id`, `Name`, `Level`, `Faction`, `Hp`, `MaxHp`, `Mp`, `MaxMp`, `Hxp`, `Attack`, `Armor`, `Class`, `Gender`, `Sp`, `Current_position_x`, `Current_position_y`, `Current_position_z`, `MapCode`, `Gold`, `InventorySpace`, `Created_at`) VALUES
	(101, 8, '잉걸불', 1, 'HOPE', 100, 100, 50, 50, 0, 10, 10, 'WARRIOR', 'MALE', 0, 151.874, 18.2837, 140.978, 1, 0, 20, '2025-01-06 23:23:28'),
	(103, 8, 'dasdasdadASD', 1, 'HOPE', 100, 100, 50, 50, 0, 10, 10, 'WARRIOR', 'FEMALE', 0, 77.4606, 19.1927, 145.919, 1, 0, 20, '2025-01-07 00:52:50'),
	(109, 28, 'dd', 1, 'HOPE', 100, 100, 50, 50, 0, 10, 10, 'WARRIOR', 'MALE', 0, 62.9638, 16.0068, 123.75, 1, 0, 20, '2025-01-12 06:12:25'),
	(110, 28, '', 1, 'FIRE', 100, 100, 50, 50, 0, 10, 10, 'WARRIOR', 'FEMALE', 0, 66.237, 10.4591, 79.5194, 1, 0, 20, '2025-01-12 06:13:00'),
	(113, 29, 'zzz', 5, 'FIRE', 500, 500, 450, 450, 200, 30, 10, 'WARRIOR', 'MALE', 0, 70.0307, 8.01026, 73.0544, 1, 0, 20, '2025-01-13 22:39:44'),
	(114, 30, 'asdasdasd', 1, 'HOPE', 100, 100, 50, 50, 0, 10, 10, 'MAGE', 'FEMALE', 0, 70, 5, 65, 1, 0, 20, '2025-01-17 00:47:17'),
	(117, 8, '현현현', 1, 'FIRE', 100, 100, 50, 50, 0, 10, 10, 'MAGE', 'MALE', 0, 31.2247, 4.18888, 46.5327, 1, 0, 20, '2025-01-17 00:50:12'),
	(141, 32, 'Embers1', 1, 'HOPE', 100, 100, 50, 50, 0, 10, 10, 'WARRIOR', 'MALE', 0, 30.4525, 4.12251, 48.6059, 1, 0, 20, '2025-08-17 16:05:35'),
	(147, 7, 'asdasd', 4, 'HOPE', 400, 400, 350, 350, 350, 25, 40, 'WARRIOR', 'MALE', 0, -16.8891, 1.015, 61.0954, 1, 0, 20, '2025-11-09 18:03:59'),
	(148, 8, '1234123', 1, 'HOPE', 100, 100, 50, 50, 0, 10, 10, 'WARRIOR', 'MALE', 0, 33, 5.83113, 36, 1, 0, 20, '2025-11-09 21:32:06');
/*!40000 ALTER TABLE `character` ENABLE KEYS */;

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

-- 테이블 데이터 embers.inventory:~0 rows (대략적) 내보내기
/*!40000 ALTER TABLE `inventory` DISABLE KEYS */;
/*!40000 ALTER TABLE `inventory` ENABLE KEYS */;

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

-- 테이블 데이터 embers.skill:~3 rows (대략적) 내보내기
/*!40000 ALTER TABLE `skill` DISABLE KEYS */;
REPLACE INTO `skill` (`Skill_table_id`, `Character_id`, `Skill_id`, `Level`) VALUES
	(86, 147, 0, 3),
	(87, 147, 1, 5),
	(90, 147, 2, 1);
/*!40000 ALTER TABLE `skill` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
