#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ProductApi/OnlineRetailer.ProductApi.csproj", "ProductApi/"]
COPY ["Entities/OnlineRetailer.Entities.csproj", "Entities/"]
RUN dotnet restore "ProductApi/OnlineRetailer.ProductApi.csproj"
COPY . .
WORKDIR "/src/ProductApi"
RUN dotnet build "OnlineRetailer.ProductApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OnlineRetailer.ProductApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineRetailer.ProductApi.dll"]
