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
        stage('Deploy') {
            steps {
                sh "docker stop shop-app || true"
                sh "docker rm shop-app || true"
                sh "docker run -d --name shop-app -p 80:80 ${DOCKER_IMAGE}:latest"
            }
        }
    }
}
