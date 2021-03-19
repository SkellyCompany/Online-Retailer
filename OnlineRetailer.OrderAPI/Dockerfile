#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OnlineRetailer.OrderAPI/OnlineRetailer.OrderAPI.csproj", "OnlineRetailer.OrderAPI/"]
COPY ["OnlineRetailer.Entities/OnlineRetailer.Entities.csproj", "OnlineRetailer.Entities/"]
COPY ["OnlineRetailer.Messaging/OnlineRetailer.Messaging.csproj", "OnlineRetailer.Messaging/"]
RUN dotnet restore "OnlineRetailer.OrderAPI/OnlineRetailer.OrderAPI.csproj"
COPY . .
WORKDIR "/src/OnlineRetailer.OrderAPI"
RUN dotnet build "OnlineRetailer.OrderAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OnlineRetailer.OrderAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineRetailer.OrderAPI.dll"]
