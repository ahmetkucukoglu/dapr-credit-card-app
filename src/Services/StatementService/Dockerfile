FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/StatementService/StatementService.csproj", "src/Services/StatementService/"]
RUN dotnet restore "src/Services/StatementService/StatementService.csproj"
COPY . .
WORKDIR "/src/src/Services/StatementService"
RUN dotnet build "StatementService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StatementService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StatementService.dll"]
