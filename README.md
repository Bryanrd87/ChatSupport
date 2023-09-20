# ChatSupport System

ChatSupport is a messaging application that facilitates seamless communication between users and agents. This application utilizes technologies such as ASP.NET Core, Entity Framework Core, and RabbitMQ for message queuing.

## Getting Started

Before you begin, ensure that you have [Docker](https://www.docker.com/) installed on your system to facilitate the RabbitMQ setup.

### Setting up RabbitMQ with Docker Compose

1. **Clone the Repository:** Start by cloning the repository to your local machine.

2. **Navigate to Project Directory:** Change your directory to where the `docker-compose.yml` file is housed.

3. **Build and Start RabbitMQ Container:** Build and initiate the RabbitMQ container by executing the following command:

   ```sh
   docker-compose up -d
   ```
   
## Application Configuration
Configuration parameters for the application are stored in the appsettings.json file. Here, you will find important details such as the API key for testing and the connection string for the RabbitMQ service. Ensure to utilize the API key mentioned in this file for testing authentication and authorization in the application.

```json
{
  "ApiKeys": {
    "Client1": "ut8406QeGIhegNpH"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": "5672",
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  }
}
```

## Technical Highlights

### MediatR and CQRS
The application employs the MediatR library to implement the mediator pattern, facilitating a clean separation of concerns between command and query responsibilities, also known as the CQRS pattern.

### In-Memory SQL Database
The application utilizes an in-memory SQL database to swiftly and reliably manage data without setting up a separate database server.

### Repository Pattern with Unit of Work
The application employs the repository pattern along with the unit of work pattern, enabling encapsulated and maintainable code for interacting with the database.

## RabbitMQ Integration
The system is integrated with RabbitMQ for message queuing to enhance scalability and reliability. The RabbitMQ service in the application is responsible for publishing chat sessions and consuming messages from the queue.

## Authentication and Authorization
The system is equipped with API key-based authentication and authorization to ensure secure access to the API endpoints.

## Error Handling and Logging
A logging mechanism has been introduced to capture errors effectively and facilitate troubleshooting.

## Usage
After setting up, run the application and utilize the Swagger UI for testing API endpoints. Ensure to use the correct API key for authentication.

## Testing

### XUnit Tests
XUnit tests have been written to test various components including:

### Create Session Command Handler
RabbitMQ Service
Refer to the ChatSupport.Application.UnitTests project for the implementation of unit tests.

### Testing RabbitMQ Service
To test the ConsumeChatSessionQueue method in the RabbitMQ service, refer to the unit test project where mock setups and assertions are defined for the service methods.

## API Documentation with Swagger
The project features Swagger integration for API documentation. You can access the Swagger UI at the /swagger endpoint to test the APIs and view the documentation.

## Initial Data Seeding
Due to the complexity of the task and time constraints, I did not add specific endpoints for managing users and agents. However, I have ensured that you can still experience the core functionalities by integrating initial data seeding into the application.
