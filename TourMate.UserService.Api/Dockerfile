FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# CHỈNH LẠI ĐƯỜNG DẪN Ở ĐÂY
COPY ./TourMate.UserService.Api/TourMate.UserService.Api.csproj ./TourMate.UserService.Api/
RUN dotnet restore ./TourMate.UserService.Api/TourMate.UserService.Api.csproj

COPY . ./
WORKDIR /src/TourMate.UserService.Api
RUN dotnet build TourMate.UserService.Api.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish TourMate.UserService.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TourMate.UserService.Api.dll"]
