# Stage 1: Build
FROM mono:latest
WORKDIR /app

COPY . ./
RUN nuget restore WebMusic.csproj
RUN msbuild /p:Configuration=Release WebMusic.csproj

# Stage 2: Runtime
EXPOSE 8080
CMD ["xsp4", "--port", "8080", "--nonstop"]
