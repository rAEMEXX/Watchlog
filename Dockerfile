FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY . .

# 👇 change this path to your real web csproj path
RUN dotnet publish Watchlog.Web/Watchlog.Web.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Watchlog.Web.dll"]