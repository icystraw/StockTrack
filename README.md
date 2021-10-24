# StockTrack
An inventory and work order tracking and management software.
* Written in C#/WPF/.NET Framework 4.8.
* Designed to be a companion utility to the popular accounting software MYOB, with an emphasis on work order and inventory item history tracking.
* Designed for small businesses.
## How to build and run
* The project can be opened and compiled in Visual Studio 2019.
* This program requires SQL Server database engine as backend data storage. SQL Server 2014 or later recommended.
* The database script file is script.sql. Please run it on a blank database to create data structure.
* Data connection string is located in StockTrack.exe.config. Please modify 'conStr' to suit your database environment.
