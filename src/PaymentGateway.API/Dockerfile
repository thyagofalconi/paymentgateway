#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
#COPY ["src/PaymentGateway.API/PaymentGateway.API.csproj", "src/PaymentGateway.API/"]
#COPY ["src/PaymentGateway.Domain/PaymentGateway.Domain.csproj", "src/PaymentGateway.Domain/"]
#COPY ["src/PaymentGateway.Model/PaymentGateway.Model.csproj", "src/PaymentGateway.Model/"]
#COPY ["src/PaymentGateway.Data/PaymentGateway.Data.csproj", "src/PaymentGateway.Data/"]
RUN dotnet restore "/src/PaymentGateway.API/PaymentGateway.API.csproj"
COPY . .
WORKDIR "/src/src/PaymentGateway.API"
RUN dotnet build "PaymentGateway.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentGateway.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentGateway.API.dll"]
