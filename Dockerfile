# Stage Build
FROM mono:latest
RUN apt-get update && apt-get install -y mono-xsp4 && rm -rf /var/lib/apt/lists/*
WORKDIR /app

COPY . ./
RUN nuget restore WebMusic.csproj -PackagesDirectory ./packages
RUN msbuild /p:Configuration=Release /p:OutputPath=./out WebMusic.csproj

RUN mkdir -p bin && cp out/*.dll bin/

EXPOSE 8080
CMD ["xsp4", "--port", "8080", "--nonstop", "--address", "0.0.0.0"]
