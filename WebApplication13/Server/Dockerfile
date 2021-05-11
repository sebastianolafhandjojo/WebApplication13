#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["WebApplication13/Server/WebApplication13.Server.csproj", "WebApplication13/Server/"]
COPY ["WebApplication13/Client/WebApplication13.Client.csproj", "WebApplication13/Client/"]
COPY ["WebApplication13/Shared/WebApplication13.Shared.csproj", "WebApplication13/Shared/"]
RUN dotnet restore "WebApplication13/Server/WebApplication13.Server.csproj"
COPY . .
WORKDIR "/src/WebApplication13/Server"
RUN dotnet build "WebApplication13.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApplication13.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplication13.Server.dll"]
#CMD ASPNETCORE_URLS=http://*:$PORT dotnet WebApplication13.Server.dll