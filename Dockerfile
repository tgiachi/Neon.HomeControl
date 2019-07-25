FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
ARG IS_DOCKER=1
ENV IS_DOCKER=${IS_DOCKER}
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 1883

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
#COPY .git/ ./.git/
COPY global.json ./
COPY Neon.HomeControl.sln ./
COPY Neon.HomeControl.Api/*.csproj ./Neon.HomeControl.Api/
COPY Neon.Plugin.Test/*.csproj ./Neon.Plugin.Test/
COPY Neon.HomeControl.Components/*.csproj ./Neon.HomeControl.Components/
COPY Neon.HomeControl.Entities/*.csproj ./Neon.HomeControl.Entities/
COPY Neon.HomeControl.Dto/*.csproj ./Neon.HomeControl.Dto/
COPY Neon.HomeControl.Service/*.csproj ./Neon.HomeControl.Service/
COPY Neon.HomeControl.Services/*.csproj ./Neon.HomeControl.Services/
COPY Neon.HomeControl.StandardLuaLibrary/*.csproj ./Neon.HomeControl.StandardLuaLibrary/
COPY Neon.HomeControl.Web/*.csproj ./Neon.HomeControl.Web/

RUN dotnet restore
COPY . .
WORKDIR /src/Neon.HomeControl.Web
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENV IS_DOCKER=${IS_DOCKER}
ENTRYPOINT ["dotnet", "Neon.HomeControl.Web.dll"]