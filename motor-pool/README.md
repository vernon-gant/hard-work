# Motor Pool Management System

## Project Overview

The Motor Pool Management System is a comprehensive web application designed for managing vehicles, enterprises, drivers, and trips efficiently. It provides features for tracking vehicle details, managing enterprises and their managers, assigning drivers, handling GPS tracks, and generating detailed reports.

## Features and Functionalities

### Vehicle Management
- **Vehicles**: Add, edit, view, and delete vehicles with details such as cost, manufacture year, mileage, and acquisition date.
- **Brands**: Separate model to manage brand details including type (passenger car, truck, bus), fuel tank capacity, payload capacity, and seating.

![enterprise vehicles](https://github.com/user-attachments/assets/410269d1-4065-4c69-9437-eda56a82b014)
![vehicle brands](https://github.com/user-attachments/assets/fb29f47b-e97f-458e-8b79-8a122ae928dd)


### Enterprise Management
- **Enterprises**: Add and manage enterprises, each associated with vehicles, drivers, and managers. Enterprises are assigned time zones for accurate time management.
- **Managers**: Enterprises have assigned managers, inheriting from standard users with authorization.

![home](https://github.com/user-attachments/assets/5765baeb-a8b8-4e0c-aad9-19dc4c41e083)

### REST API
- Comprehensive API for managing vehicles, brands, enterprises, and drivers.
- Supports CRUD operations for vehicles and enterprises.
- Paged responses for large datasets.
- Time-zone-aware API for date-time fields.

### Driver Management
- Assign drivers to enterprises and vehicles. Drivers can be active for specific vehicles, ensuring accurate tracking.
- Supports many-to-many relationships between vehicles and drivers.

![vehicle details](https://github.com/user-attachments/assets/bec2c2c4-d6b4-469f-97bf-ac71273cdda6)

### GPS Tracking and Trips
- **Trips**: Tracks vehicle trips with start and end timestamps in UTC, associated with enterprise time zones.
- **GPS Tracks**: Efficient storage and retrieval of GPS data for trips, with support for geoJSON.
- Generate realistic GPS tracks using city maps and routing APIs.

![trips](https://github.com/user-attachments/assets/e85cae74-f600-4cb7-9900-c393556fbfa0)
![map](https://github.com/user-attachments/assets/f720cbf3-7f8e-4459-b2a0-28778050b5fc)

### Reports
- Generate reports for vehicle mileage over specified periods (day, month, year).
- REST API for report generation with customizable parameters.
- Web interface for report visualization and downloading.

![reports](https://github.com/user-attachments/assets/18d3ad5d-c9e3-4900-8738-da2ad00a2744)
![vehicle mileage report](https://github.com/user-attachments/assets/ecb8d00d-35e1-47b5-b539-b260bdd70ebd)

## Key Technical Highlights

- **Technologies Used**: ASP.NET Core MVC, Entity Framework Core, RESTful APIs.
- **Pagination**: Efficient handling of large datasets using paginated responses.
- **Time Zone Management**: Accurate time handling across multiple time zones.
- **Routing and Maps**: Integration with OpenRouteService and Map APIs for GPS track generation and visualization.
- **Data Models**: Separation of concerns with distinct models for vehicles, brands, enterprises, drivers, trips, and reports.

## Technical Experiments and Educational Additions

### Data Generation Tools
- **Vehicles and Drivers**: Generates thousands of vehicles and drivers, with realistic details. Useful for development and testing.
- **GPS Tracks**: Simulates realistic vehicle tracks over a specified range, with routing APIs (e.g., OpenRouteService).

### Deployment
- Automated deployment using a Docker Compose setup with the UI and database in separate containers.
- Script installs Docker and builds the project for seamless deployment.

### Telegram Bot
- Created a Telegram bot for manager interaction
- Features login functionality and report generation for vehicles and enterprises.
- Commands include `/login` for authentication and mileage report retrieval.

![tg](https://github.com/user-attachments/assets/8ad46d92-fcf0-47d0-989b-224762a02c87)

### Caching
- Implemented caching for improved performance
- Output caching middleware for web pages.
- IMemoryCache for storing frequently accessed data, such as reports.

### Azure DevOps
- Experimented with CI/CD pipelines in Azure
- Built services, created Docker images, and pushed them to Azure Container Registry.
- Focused on learning the fundamentals of CI/CD pipelines.
- Configured Blob Storage for telemetry data storage.
- Experimented with Logic Apps for automated workflows.

![cicd](https://github.com/user-attachments/assets/c55e1e5d-66d6-4d62-a946-f0fec7351788)
![blob](https://github.com/user-attachments/assets/0be0a0ae-bcba-4d22-8837-b26110db7f5d)
![logic app](https://github.com/user-attachments/assets/fdc7c48f-a5b9-45c6-bef6-8c8f51f990bb)


### Microservices
- Added microservices using Kafka for educational purposes
- One service stores telemetry data in Azure Blob Storage.
- Another monitors vehicle speeds and sends alerts if thresholds are exceeded.

### Load Testing
- Conducted load testing using the NBomber library
- Tested API endpoints under high traffic to evaluate performance and scalability.

### Integration and E2E Testing
- **Integration Tests**: Used ASP.NET integration testing framework:
    - Spun up Docker containers for databases during tests.
    - Verified end-to-end system behavior.
- **E2E Tests**: Developed Cypress tests for various user workflows in the UI.

### Logging
- Integrated logging with ASP.NET Core and experimented with Serilog
- Configured structured logging and log analysis tools for better traceability.

### Load and Stress Testing
- Conducted load testing to simulate real-world usage scenarios
- Explored tools like NBomber to evaluate API resilience.

### Microservices Implementation
- Implemented microservices for telemetry processing
- Used Kafka as the message broker.
- Developed services for data storage and threshold-based alerts.

### Integration Tests
- Added comprehensive integration tests for REST APIs
- Used Dockerized databases for testing in isolated environments.
- Covered core use cases and business logic.

## Educational Insights
- All implementations were undertaken for educational purposes.
- Gained practical insights into multiple technologies and frameworks.
- These experiments were designed to explore potential solutions and understand the basics, not to create production-grade systems.
