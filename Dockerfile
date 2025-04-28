# Use a base image with .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy only the necessary files (adjust paths as needed)
COPY . .  # This assumes FitnessAplikacia is not required and you are only copying what is needed for FitnessAuthBackend.

# Restore dependencies for the solution (only for relevant projects)
RUN dotnet restore FitnessAuthBackend.sln

# Publish the application to the out directory
RUN dotnet publish FitnessAuthBackend.sln -c Release -o out

# Define entry point (adjust if necessary)
ENTRYPOINT ["dotnet", "out/FitnessAuthBackend.dll"]
