FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["vitalia-backend.csproj", "."]
RUN dotnet restore "./vitalia-backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "vitalia-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "vitalia-backend.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "vitalia-backend.dll"]
