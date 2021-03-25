unlock tables;
drop database if exists `somedb`;
create database `somedb`;
use `somedb`;

drop user if exists `app`@`localhost`;
create user `app`@`localhost` identified by "Medium strength password.";
grant all privileges on `somedb`.* to `app`@`localhost`;
flush privileges;

-- main tables
create table `files` (
  `ID` bigint unsigned not null auto_increment,
  `Name` varchar(255) not null,
  `MachineName` varchar(255) not null,
  `Path` varchar(255) not null,
  `Extension` varchar(255) not null,
  `Size` int not null,
  `UploadTimestamp` timestamp not null,
  primary key (`ID`)
);

create table `users` (
  `ID` bigint unsigned not null auto_increment,
  `ID_Photo` bigint unsigned,
  `Email` varchar(255) not null,
  `Password` varchar(255) not null,
  `Nickname` varchar(255) not null,
  `BIO` text not null,
  `IsDeleted` boolean not null,
  `RegistrationTimestamp` timestamp not null,
  primary key (`ID`),
  foreign key (`ID_Photo`) references `photos` (`ID`)
);

create table `roles` (
  `ID` bigint unsigned not null auto_increment,
  `Name` varchar(255) not null,
  `Discription` varchar(255) not null,
  primary key (`ID`)
);

create table `permissions` (
  `ID` bigint unsigned not null auto_increment,
  `Name` varchar(255) not null,
  `Discription` varchar(255) not null,
  primary key (`ID`)
);

create table `refresh_sessions` (
  `ID` bigint unsigned not null auto_increment,
  `ID_User` bigint unsigned not null,
  `RefreshToken` varchar(255) not null,
  `CreationTimestamp` timestamp not null,
  primary key (`ID`),
  foreign key (`ID_User`) references `users` (`ID`)
);

-- dependent tables
create table `user_roles` (
  `ID` bigint unsigned not null auto_increment,
  `ID_User` bigint unsigned not null,
  `ID_Role` bigint unsigned not null,
  primary key (`ID`),
  foreign key (`ID_User`) references `users` (`ID`),
  foreign key (`ID_Role`) references `roles` (`ID`)
);

create table `role_permissions` (
  `ID` bigint unsigned not null auto_increment,
  `ID_Role` bigint unsigned not null,
  `ID_Permission` bigint unsigned not null,
  primary key (`ID`),
  foreign key (`ID_Role`) references `roles` (`ID`),
  foreign key (`ID_Permission`) references `permissions` (`ID`)
);

