# Stage Build
FROM mono:latest
RUN sed -i 's/deb.debian.org/archive.debian.org/g' /etc/apt/sources.list && \
    sed -i 's|security.debian.org/debian-security|archive.debian.org/debian-security|g' /etc/apt/sources.list && \
    sed -i '/stretch-updates/d' /etc/apt/sources.list && \
    apt-get update && \
    apt-get install -y mono-xsp4 && \
    ln -s /usr/bin/xsp4 /usr/local/bin/xsp4 && \ 
    rm -rf /var/lib/apt/lists/*
WORKDIR /app

COPY . ./
RUN nuget restore WebMusic.csproj -PackagesDirectory ./packages
RUN msbuild /p:Configuration=Release /p:OutputPath=./out WebMusic.csproj

RUN mkdir -p bin && cp out/*.dll bin/

EXPOSE 8080
CMD ["xsp4", "--port", "8080", "--nonstop", "--address", "0.0.0.0"]
