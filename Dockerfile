# Use the official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY ["MovieRankerApp/Movie Ranker App/Movie Ranker/Movie Ranker.csproj", "./MovieRanker.csproj"]
RUN dotnet restore "MovieRanker.csproj"

# Copy remaining files and build the app
COPY "MovieRankerApp/Movie Ranker App/Movie Ranker/" ./
RUN dotnet publish -c Release -o out

# Use a smaller runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

#Copy built files
COPY --from=build /app/out .

# Expose the port used by the app
EXPOSE 8080

# Start the application
CMD ["dotnet", "MovieRanker.dll"]
