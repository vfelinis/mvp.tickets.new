FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["mvp.tickets.web/mvp.tickets.web.csproj", "mvp.tickets.web/"]
RUN dotnet restore "mvp.tickets.web/mvp.tickets.web.csproj"
COPY . .
WORKDIR "/src/mvp.tickets.web"
RUN dotnet build "mvp.tickets.web.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "mvp.tickets.web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mvp.tickets.web.dll"]