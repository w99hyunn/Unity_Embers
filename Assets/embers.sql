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
  `account_id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `password_salt` varchar(255) NOT NULL,
  `email` varchar(100) NOT NULL,
  `created_at` datetime DEFAULT current_timestamp(),
  `last_login` datetime DEFAULT NULL,
  `is_online` tinyint(1) DEFAULT 0,
  PRIMARY KEY (`account_id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8;

-- 테이블 데이터 embers.account:~19 rows (대략적) 내보내기
/*!40000 ALTER TABLE `account` DISABLE KEYS */;
REPLACE INTO `account` (`account_id`, `username`, `password_hash`, `password_salt`, `email`, `created_at`, `last_login`, `is_online`) VALUES
	(7, 'admin', 'oUFgXVi1dZD+WJzcgw95o0JOaBr6qB8o34Mg5JmxQGo=', 'io9wR503G+C45TLCIoLdwhp3g6bXFgbARZ3XnQNtsD4=', 'admin@naver.com', '2024-12-28 20:28:10', NULL, 1),
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
	(25, 'sssd2', 'kHvQk5RDjGlYCYvOyi3l/AVnB7+j2VgET0phYyp33qI=', 'Vip39f7Q5458baffbvVFKxguqOEpdIgaL7b/auJedIo=', '', '2024-12-31 23:58:09', NULL, 1);
/*!40000 ALTER TABLE `account` ENABLE KEYS */;

-- 테이블 embers.character 구조 내보내기
CREATE TABLE IF NOT EXISTS `character` (
  `character_id` int(11) NOT NULL AUTO_INCREMENT,
  `account_id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `level` int(11) DEFAULT 1,
  `faction` int(11) NOT NULL DEFAULT 0,
  `hp` int(11) DEFAULT 100,
  `maxHp` int(11) NOT NULL DEFAULT 100,
  `mp` int(11) DEFAULT 50,
  `maxMp` int(11) NOT NULL DEFAULT 50,
  `exp` int(11) DEFAULT 0,
  `attack` int(11) NOT NULL DEFAULT 10,
  `class` int(11) NOT NULL DEFAULT 0,
  `gender` int(11) NOT NULL DEFAULT 0,
  `sp` int(11) NOT NULL DEFAULT 0,
  `current_position_x` float DEFAULT 0,
  `current_position_y` float DEFAULT 0,
  `current_position_z` float DEFAULT 0,
  `mapCode` int(11) DEFAULT NULL,
  `gold` int(11) DEFAULT 0,
  `inventory_space` int(11) DEFAULT 20,
  `created_at` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`character_id`),
  UNIQUE KEY `unique_character_name` (`name`),
  KEY `account_id` (`account_id`),
  CONSTRAINT `character_ibfk_1` FOREIGN KEY (`account_id`) REFERENCES `account` (`account_id`)
) ENGINE=InnoDB AUTO_INCREMENT=72 DEFAULT CHARSET=utf8;

-- 테이블 데이터 embers.character:~5 rows (대략적) 내보내기
/*!40000 ALTER TABLE `character` DISABLE KEYS */;
REPLACE INTO `character` (`character_id`, `account_id`, `name`, `level`, `faction`, `hp`, `maxHp`, `mp`, `maxMp`, `exp`, `attack`, `class`, `gender`, `sp`, `current_position_x`, `current_position_y`, `current_position_z`, `mapCode`, `gold`, `inventory_space`, `created_at`) VALUES
	(15, 8, '짬지4', 10, 0, 892, 100, 50, 50, 0, 10, 0, 0, 0, 0, 0, 0, 1, 0, 20, '2025-01-01 16:18:54'),
	(16, 8, 'test1', 20, 0, 600, 800, 500, 600, 0, 10, 0, 0, 0, 50, 30, 10, 1, 0, 20, '2025-01-01 18:17:34'),
	(70, 7, 'gdgd', 1, 0, 100, 100, 50, 50, 0, 10, 0, 0, 0, 0, 0, 0, 1, 0, 20, '2025-01-01 23:19:27'),
	(71, 7, '꼬꼬', 1, 0, 100, 100, 50, 50, 0, 10, 1, 1, 0, 0, 0, 0, 1, 0, 20, '2025-01-01 23:20:50');
/*!40000 ALTER TABLE `character` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
