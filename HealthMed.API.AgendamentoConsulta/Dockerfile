# Define the .NET version and build configuration as arguments
ARG DOTNET_VERSION=8.0
ARG BUILD_CONFIGURATION=Release

# Use the .NET SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:$DOTNET_VERSION AS build

# Set the working directory in the container
WORKDIR /app

# Copy project files into the container
COPY ./ ./

# Restore dependencies
RUN dotnet restore "./HealthMed.API.AgendamentoConsulta.csproj"

# Build and publish the application
RUN dotnet publish "./HealthMed.API.AgendamentoConsulta.csproj" -c $BUILD_CONFIGURATION -o out

# Use the ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:$DOTNET_VERSION AS runtime

# Set the working directory in the runtime container
WORKDIR /app

# Copy published output from the build stage
COPY --from=build /app/out .

# Expose port 80 for the application
EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "HealthMed.API.AgendamentoConsulta.dll"]