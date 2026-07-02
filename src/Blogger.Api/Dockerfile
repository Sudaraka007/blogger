FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Blogger.Domain/Blogger.Domain.csproj src/Blogger.Domain/
COPY src/Blogger.Persistence/Blogger.Persistence.csproj src/Blogger.Persistence/
COPY src/Blogger.Api/Blogger.Api.csproj src/Blogger.Api/
RUN dotnet restore src/Blogger.Api/Blogger.Api.csproj

COPY src/Blogger.Domain/ src/Blogger.Domain/
COPY src/Blogger.Persistence/ src/Blogger.Persistence/
COPY src/Blogger.Api/ src/Blogger.Api/
RUN dotnet publish src/Blogger.Api/Blogger.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Blogger.Api.dll"]
