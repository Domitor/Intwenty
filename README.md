![alt text](https://github.com/Domitor/Intwenty/blob/master/IntwentyDemo/wwwroot/images/intwenty_loggo_small.png)


# Intwenty
Create metadata driven applications with java script and ASP.NET Core, a tool for developers or users who wants to build quick business applications without or with minimal coding

# Why
- To minimize coding by reusing the same code with different sets of metadata.
- To quickly produce web applications.

# In short Intwenty intends to
1. Let the user or developer describe applications, user interface, data structures, and static data by inputing metadata.
2. Let Intwenty generate database and database objects based on metadata
3. Let Intwenty generate UI based on metadata
4. Let Intwenty generate standard logic such as user input validation based on metadata
5. Let Intwenty generate standard logic for I/O operations against the database based on metadata
6. Let the user easily override all the above when necessary

# Features
- Forget about entity framework code first, database first, migrations etc. With intwenty it's possible to change both the UI and datamodel without recompile or even publish.
- Easy import and export of metamodels which makes it possible for users to share models.
- Automatic CRUD endpoints for integrations.
- Generates documentation. 

# How
- Intwenty does not work with strongly typed models, except for the metamodel itself. It uses entity framework to store and retrive the metamodel and it uses a custom service to manage (application) data according to the model.
- Users/Developers can input metadata via a graphical UI, which allows for handling both the datamodel and the UI model. 
- It comes as an RCL (Razor Clas Library) which can easily be included in any asp.net core web application.

# Technologies Used
- asp.net core 3.1
- bootstrap 4.3.7
- vue.js 2.6.11
- jquery 3.3.1
- alasql 0.5.5

# Database
- Intwenty targets MS Sql Server and MySql

# How to get started
<a href="https://github.com/Domitor/Intwenty/wiki">Consult the Wiki</a>

# Status
Weekly updates








