#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ChatSupportAPI/ChatSupport.WebAPI.csproj", "ChatSupportAPI/"]
COPY ["ChatSupport.Application/ChatSupport.Application.csproj", "ChatSupport.Application/"]
COPY ["ChatSupport.Domain/ChatSupport.Domain.csproj", "ChatSupport.Domain/"]
COPY ["ChatSupport.Persistence/ChatSupport.Persistence.csproj", "ChatSupport.Persistence/"]
RUN dotnet restore "ChatSupportAPI/ChatSupport.WebAPI.csproj"
COPY . .
WORKDIR "/src/ChatSupportAPI"
RUN dotnet build "ChatSupport.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatSupport.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatSupport.WebAPI.dll"]