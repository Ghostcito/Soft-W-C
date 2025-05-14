# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia el archivo .csproj y restaura
COPY "Soft W&C.csproj" ./
RUN dotnet restore "./Soft W&C.csproj"

# Copiar el resto del código
COPY . ./
RUN dotnet publish "./Soft W&C.csproj" -c Release -o out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiar archivos publicados
COPY --from=build /app/out ./

# Exponer puerto y establecer la URL
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Ejecutar el proyecto
ENTRYPOINT ["dotnet", "Soft W&C.dll"]
