![alt text](https://github.com/Domitor/Intwenty/blob/master/Intwenty/wwwroot/images/intwenty_loggo_small.png)


# Intwenty
Create meta data driven applications with java script and ASP.NET Core, a tool for developers or for people who wants to build quick business applications without or with minimal coding

# Why
- To minimize coding by reusing the same code with different sets of meta data.
- To quickly produce web applications.

# In short Intwenty intends to
1. Let the user or developer describe applications, UI, data structures, and static data by inputing meta data.
2. Let Intwenty generate database and database objects based on meta data
3. Let Intwenty generate UI based on meta data
4. Let Intwenty generate standard logic such as user input validation based on meta data
5. Let Intwenty generate standard logic for I/O operations against the database based on meta data
6. Let the user easily override all the above when necessary

# How
- Intwenty does not work with strongly typed models, except for the meta model itself. It uses entity framework to store and retrive the meta model and it uses a custom service to store data based on the model.
- Users/Developers can input meta data (create / update applications) via a graphical UI, which allows for handling both the data model and the UI model. For inputing the UI model Jquery form builder is used. 

# Technologies Used
- Asp.Net Core 3.1
- Jquery
- Jquery form builder
- Vue.js
- alasql.js

# Database
- Intwenty targets MS Sql Server and MySql

# How to get started
<a href="https://github.com/Domitor/Intwenty/wiki">Consult the Wiki</a>








