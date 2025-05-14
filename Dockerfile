# Imagen base para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar archivos del proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto de archivos y publicar
COPY . ./
RUN dotnet publish -c Release -o out

# Imagen base para producción
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiar desde el build
COPY --from=build /app/out .

# Exponer el puerto que usa tu app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Cambia esto si tu DLL tiene otro nombre
ENTRYPOINT ["dotnet", "Soft W&C.dll"]