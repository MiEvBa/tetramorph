FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Install python
RUN apt-get update -y 
RUN apt-get install -y python3

# Install python wasm-tools
RUN dotnet workload install wasm-tools

# Build
COPY . ./
RUN dotnet publish -c Release -o output

FROM nginx:alpine-slim
WORKDIR /var/www/web
COPY --from=build-env /app/output/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80