# Alpha-Helpdesk
Expert System/Automated Help desk system for a Games Company

Computer Science Coursework - Used to showcase skills of both VB .NET & SQL

This Project could easily be modified to communicate with a MySQL Server. For ease of testing, it was setup on local host, although it has been tested on WAN and DOES work.

What does each file do?

ApplicationEvents.vb - This file consists of SQL connections & queries that need to be made to setup the program, this includes creating the database and tables and populating tables where required. This also sets up the master admin account.

frmLogin.vb - This form performs the login operations for the program, this involves verifying user credentials (confirming password with hash stored)

frmRegister.vb - This form performs all registraion operations for the program, this involves allowing both consumer and staff accounts to be set up.

frmMainCustomer.vb - This is the form all customers will use, this allows them to search for issues related to their games as well as fixes that match their specific issues.

frmMainStaffLvl1.vb - This is the form for all customer service staff, this allows the staff memeber to add & remove games & issues relating to those games that the company offer support for.

frmMainStaffLvl2.vb - This is the form for the database administrator. This form allows them to change the MySQL server details (Server IP, Server Port, MySQL username & password). They can also view all table contents as well as generate new staff registration codes and create new versions of tables and the database itself.

Icon.ico - This file is just a copy of the icon for the program.


THIS PROGRAM WILL NOT WORK WITH JUST THESE FILES THIS IS JUST TO DISPLAY MY CODE

If you want a working copy, they just drop me a private message.
