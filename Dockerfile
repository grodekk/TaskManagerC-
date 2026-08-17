FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TaskManagerAPI/TaskManagerAPI.csproj TaskManagerAPI/
RUN dotnet restore TaskManagerAPI/TaskManagerAPI.csproj

COPY . .

WORKDIR /src/TaskManagerAPI

RUN dotnet publish TaskManagerAPI.csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskManagerAPI.dll"]