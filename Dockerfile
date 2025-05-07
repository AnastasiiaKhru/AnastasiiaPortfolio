FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AnastasiiaPortfolio.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish "AnastasiiaPortfolio.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "AnastasiiaPortfolio.dll"] 