
# Handlenett API

This is the ASP.NET Core Web API backend for the Handlenett shopping list application, designed to run on both Windows and macOS systems. The API is containerized using Docker for consistent environments and deployed as a container app in Azure.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Development Setup](#development-setup)
  - [Windows (Visual Studio or Visual Studio Code)](#windows-visual-studio-or-visual-studio-code-setup)
  - [macOS](#macos-setup)
  - [Environment Variables](#environment-variables)
  - [Swagger Documentation](#swagger-documentation)
- [Production Setup](#production-setup)
  - [Build and Deployment Workflow](#build-and-deployment-workflow)
- [Azure Resources](#azure-resources)

---

## Prerequisites

Before running or building the project, ensure the following software is installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://docs.docker.com/get-docker/) (for local development and testing)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

## Development Setup

### Windows Visual Studio or Visual Studio Code Setup

1. **Using Visual Studio or Visual Studio Code**  
   Open the project in your preferred IDE (Visual Studio or VS Code). You can build and run the project using the built-in tools for running Docker containers.  
   In Visual Studio, simply press **F5** or click **Run** to start the API in a Docker container. Visual Studio Code users can use the **Run** button as well.

2. **Docker Image**  
   The Docker image can be built locally for testing directly within your IDE using Docker support. Ensure Docker Desktop is running.

   The API will be available at `http://localhost:32769` after running.

### macOS Setup

1. **Using Visual Studio Code**  
   For macOS, use Visual Studio Code as the primary IDE. Make sure Docker Desktop for Mac is installed and running.

2. **Build the Docker Image**  
   From the terminal in Visual Studio Code, build the Docker image:

   ```bash
   docker build -t handlenett-api .
   ```

3. **Run the Docker Container**  
   After the image is built, run the Docker container:

   ```bash
   docker run -e ASPNETCORE_ENVIRONMENT=Development -p 32769:80 handlenett-api
   ```

   The API will be available at `http://localhost:32769`.

4. **Troubleshooting**  
   Ensure Docker Desktop is running. If there are permission issues, try:

   ```bash
   sudo docker run -e ASPNETCORE_ENVIRONMENT=Development -p 32769:80 handlenett-api
   ```

### Environment Variables

Secrets and environment variables are automatically managed through Azure Key Vault (`handlenett-prod-kv`). There's no need to manually set these values, as the application will access them securely during runtime.


### Swagger Documentation

The Handlenett API includes Swagger documentation with extended functionality for better API exploration and testing. Swagger allows you to easily view the API endpoints, input parameters, and responses. It also supports authentication using bearer tokens for secure API requests.

#### Retrieving a JWT for Local Development

To interact with the secured API endpoints, you'll need a valid JWT. Here's how to retrieve it:

1. Open the following URL in your browser:
   
   ```
   https://login.microsoftonline.com/7534ab3e-6d52-4213-8493-1d674b529724/oauth2/v2.0/authorize?client_id=6409e25f-f9b7-4b70-a84c-6c077440d740&redirect_uri=https%3A%2F%2Foidcdebugger.com%2Fdebug&scope=api%3A%2F%2F6409e25f-f9b7-4b70-a84c-6c077440d740%2Faccess_as_user%20User.Read&response_type=token&response_mode=form_post&nonce=w6esugjg4wf
   ```

2. This URL will take you to the Microsoft identity platform's login page. Sign in with your Azure account that has the necessary permissions.

3. After successful login, you’ll be redirected to [OIDC Debugger](https://oidcdebugger.com), where you can copy the `access_token` provided in the response.

4. Once you have the token, go to the Swagger UI for the API (running locally at `http://localhost:32769/swagger`).

5. Click the **Authorize** button in the Swagger UI and paste the token in the following format:

   ```
   Bearer <your_token>
   ```

6. Now, you can interact with protected API endpoints directly from Swagger.

#### Accessing Swagger UI

1. After starting the API locally (on either Windows or macOS), you can access the Swagger UI at:

   ```
   http://localhost:32769/swagger
   ```

2. The Swagger UI will list all the available endpoints and allow you to interact with the API directly from the browser.

---


## Production Setup

The project is set up with a CI/CD pipeline that automates the build and deployment of the Docker container to Azure. The production setup uses GitHub Actions for managing this workflow.

### Build and Deployment Workflow

The production build and deployment are automated using the `handlenett-api.yml` workflow in the GitHub repository. This workflow:

1. Builds the Docker image
2. Pushes the image to the Azure Container Registry (`handlenett`)
3. Deploys the image to the Azure Container App (`handlenett-api`)

For production, the workflow will automatically trigger on changes to the relevant paths, so there's no need to manually deploy unless necessary.

## Azure Resources

The API interacts with several Azure resources:

- **Azure SQL Database**: The API stores user data in a SQL Server database named `Handlenett`.
- **Cosmos DB**: The Cosmos DB account (`handlenett-cosmosdb-ac`) is used to store shopping list items.
- **Azure Key Vault**: Secrets are securely stored and accessed using app authentication in Azure Key Vault (`handlenett-prod-kv`).
- **Azure Storage Account**: The storage account (`handlenettsa`) is used for storing user and item images.
