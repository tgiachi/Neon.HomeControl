FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Neon.HomeControl.Web/Neon.HomeControl.Web.csproj", "Neon.HomeControl.Web/"]
RUN dotnet restore "Neon.HomeControl.Web/Neon.HomeControl.Web.csproj"
COPY . .
WORKDIR "/src/Neon.HomeControl.Web"
RUN dotnet build "Neon.HomeControl.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Neon.HomeControl.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Neon.HomeControl.Web.dll"]