# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Discord Kor.csproj", "./"]
RUN dotnet restore "Discord Kor.csproj"

# Copy the rest of the source code
COPY . .

# Build and publish the application
RUN dotnet publish "Discord Kor.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "Discord Kor.dll"]
