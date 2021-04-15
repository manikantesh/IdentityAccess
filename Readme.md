# Identity Access

Clone the project to visual studio and follow the instructions to runt the code locally.

Required Information to run the project:

  1. Postman to check the API
  2. Create an account on [SendGrid](https://sendgrid.com/) to send automated emails for verification. 
  3. Replace the personal API token from the appsettings with the API key generated from the SendGrid
  4. Add the following dependencies 
      Authentication.JwtBearer
      Identity.EntityFrameworkCore
      SendGrid
  5. Add-migration and update-database
