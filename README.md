# Online Food Portal
*A locally deployed online food ordering portal connecting customers to the kitchen*

### Development
Developed using ASP.Net Core (C#), Core Identity (C#), MySQL, HTML5, JavaScript, and CSS

### Description
This web application connects customers (users) to the kitchen allowing orders to be placed online to be fulfilled by kitchen staff on-site  
Administrators can freely create and modify items and modifications for customers to order  
Kitchen staff can mark orders as completed, picked-up, or cancelled. The user order screen will show these updates in real-time

## Deployment
*All the data is deployed on a single MySQL database for simplified deployment*  
Run your MySQL Server on port 3306 with the username 'root'  
Execute the MySQL scripts 'Food Portal DDL.sql' and 'IdentityDDL.sql'  
Enter your MySQL password into the file 'sqlpassword.txt'  
Execute the application and register your first user  
Navigate to /Test and run the component tests to verify functionality  

A helper program under User Manager/Food Portal Users.exe is included to help assign Administrator/Kitchen roles  
It will run when the web application is deployed using the SQL Connection string as an argument


## Documentation
The plans and documentation for this project can be found under /Documentation  

<a href="https://youtu.be/1y843Kx5eXM" target="_blank">Project Demonstration (YouTube)</a>  

<a href="https://youtu.be/CPcnJr7TfC0" target="_blank">Component Tests (YouTube)</a>

![Basic physical design of the web application including users, kitchen staff, and adminsitrators](Documentation/Project%20Artifacts/Milestone-Physical%20Design.png)

![In depth flow of logic between users, staff, the application, and the database](Documentation/Project%20Artifacts/Milestone-UML%20Sequence%20Diagram.png)
