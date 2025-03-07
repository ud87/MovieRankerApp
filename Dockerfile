# Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project file and restore dependencies
COPY ["MovieRankerApp/Movie Ranker App/Movie Ranker/Movie Ranker.csproj", "./MovieRanker.csproj"]
RUN dotnet restore "MovieRanker.csproj"

# Copy the entire project directory and build the app
COPY ["MovieRankerApp/Movie Ranker App/Movie Ranker/", "./"]
RUN dotnet publish "MovieRanker.csproj" -c Release -o out

# Use a smaller runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the built files from the build stage
COPY --from=build /app/out ./

# Expose the port used by the application
EXPOSE 8080

# Start the application
CMD ["dotnet", "MovieRanker.dll"]
