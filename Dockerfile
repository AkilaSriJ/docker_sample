# STEP 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Genx.TrainTatkalBooking.Api/Genx.TrainTatkalBooking.Api.csproj"
RUN dotnet publish "Genx.TrainTatkalBooking.Api/Genx.TrainTatkalBooking.Api.csproj" -c Release -o /app/publish

# STEP 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Genx.TrainTatkalBooking.Api.dll"]
