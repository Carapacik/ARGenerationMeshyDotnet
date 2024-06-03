FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ARModelGeneration.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o ARModelGeneration

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/ARModelGeneration ./
ENTRYPOINT ["dotnet", "ARModelGeneration.dll"]
