![alt text](https://github.com/Domitor/Intwenty/blob/master/IntwentyDemo/wwwroot/images/intwenty_loggo_small.png)


# Intwenty
Create metadata driven applications with java script and ASP.NET Core, a tool for developers or users who wants to build quick business applications without or with minimal coding

# Why
- To minimize coding by reusing the same code with different sets of metadata.
- To quickly produce web applications.
- To allow for hot swapping UI and data model without recompile or publish.

# In short Intwenty intends to
1. Let the user or developer describe applications, user interface, data structures, and static data by inputing metadata.
2. Let Intwenty generate database and database objects based on metadata.
3. Let Intwenty generate UI based on metadata.
4. Let Intwenty generate standard logic such as user input validation based on metadata.
5. Let Intwenty generate standard logic for I/O operations against the database based on metadata.
6. Let the user easily override all the above when necessary.

# Features
- Forget about entity framework code first, database first, migrations etc. With intwenty it's possible to change both the UI and datamodel without recompile or even publish.
- Easy import and export of metamodels which makes it possible for users to share models.
- Automatic CRUD endpoints for integrations.
- Generates documentation. 

# How
- Intwenty does not work with strongly typed models. Instead it performs work both on the data and UI level based on the information in the metamodel.
- Users/Developers can input metadata via a graphical UI, which allows for handling both the datamodel and the UI model. 
- It comes as an RCL (Razor Clas Library) which can easily be included in any asp.net core web application.

# Intentions
1. Boost productivity
2. To be lightweight
3. Keep dependencies to a minimum.

# Backend Technologies
- asp.net core 3.1
- Microsoft.AspNetCore.Mvc 2.2.0
- Microsoft.Data.Sqlite.Core 3.1.3
- System.Data.SQLite.Core 1.0.112.1
- System.Data.SqlClient 4.8.1
- MongoDB.Driver 2.10.3
- MySql.Data 8.0.19
- Npgsql 4.1.3.1

# Frontend Technologies
- bootstrap 4.3.7
- vue.js 2.6.11
- jquery 3.3.1
- alasql 0.5.5

# Works with the following databases
- MS Sql Server
- MySql
- Maria DB
- PostgreSQL
- SQLite
- MongoDB
- LiteDB

# How to get started
<a href="https://github.com/Domitor/Intwenty/wiki">Consult the Wiki</a>

# Status
Weekly updates, but not ready for production use.








