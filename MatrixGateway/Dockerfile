FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["MatrixGateway.csproj", "./"]
RUN dotnet add MatrixGateway.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
RUN dotnet restore "MatrixGateway.csproj"
COPY . .
RUN dotnet publish "MatrixGateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000 
ENTRYPOINT ["dotnet", "MatrixGateway.dll"]