FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/CreditCardService/CreditCardService.csproj", "src/Services/CreditCardService/"]
RUN dotnet restore "src/Services/CreditCardService/CreditCardService.csproj"
COPY . .
WORKDIR "/src/src/Services/CreditCardService"
RUN dotnet build "CreditCardService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CreditCardService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CreditCardService.dll"]
