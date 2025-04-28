# Use the official .NET SDK 8.0 image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the entire project into the container
COPY . .

# Restore the dependencies of your project (like 'dotnet restore')
RUN dotnet restore

# Build and publish the application in release mode to the out directory
RUN dotnet publish --no-restore -c Release -o out

# Set the entry point for your application
ENTRYPOINT ["dotnet", "out/FitnessAuthBackend.dll"]
