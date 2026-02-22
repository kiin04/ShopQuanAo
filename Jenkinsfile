pipeline {
    agent any
    environment {
        DOCKER_IMAGE = "thainv28/shop-quanao"
    }
    stages {
        stage('Build Image') {
            steps {
                sh "docker build -t ${DOCKER_IMAGE}:latest ."
            }
        }
        stage('Push to DockerHub') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'dockerhub-pass', passwordVariable: 'PASS', usernameVariable: 'USER')]) {
                    sh "echo \$PASS | docker login -u \$USER --password-stdin"
                    sh "docker push ${DOCKER_IMAGE}:latest"
                }
            }
        }
        stage('Deploy with Compose') {
            steps {
                sh "docker-compose down || true"
                sh "docker-compose up -d --force-recreate"
            }
        }
        stage('Init SQL Database') {
            steps {
                script {
                    echo "Waiting for SQL Server to start..."
                    sh "sleep 30" 
                    
                   sh """
                       docker exec -i shop-sql-db /opt/mssql-tools18/bin/sqlcmd \
                       -S localhost -U sa -P 'YourStrongPassword123!' -C \
                       -Q "IF DB_ID('ShopQuanAo') IS NULL CREATE DATABASE ShopQuanAo"
                    """
                   echo "Importing data to ShopQuanAo..."
                   sh """
                       docker exec -i shop-sql-db /opt/mssql-tools18/bin/sqlcmd \
                       -S localhost -U sa -P 'YourStrongPassword123!' -C \
                       -d ShopQuanAo -i /docker-entrypoint-initdb.d/data.sql
                    """
                }
            }
        }
    }
}
