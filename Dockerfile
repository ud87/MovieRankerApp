# Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY ["MovieRankerApp/Movie Ranker App/Movie Ranker/Movie Ranker.csproj", "./"]
RUN dotnet restore "Movie Ranker.csproj"

# Copy remaining files and build the app
COPY . ./
RUN dotnet publish -c Release -o out

# Use a smaller runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expose the port used by the app
EXPOSE 8080

# Start the application
CMD ["dotnet", "MovieRankerApp.dll"]
