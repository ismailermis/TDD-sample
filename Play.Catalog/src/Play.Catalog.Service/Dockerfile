FROM mcr.microsoft.com/dotnet/sdk:5.0 AS base

WORKDIR /app

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

WORKDIR /src

COPY ["Play.Catalog.Service.csproj","./"]

RUN dotnet restore "Play.Catalog.Service.csproj"

COPY . .

RUN dotnet publish "Play.Catalog.Service.csproj" -c Release -o /app/publish

FROM base As final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT [ "dotnet","Play.Catalog.Service.dll" ]