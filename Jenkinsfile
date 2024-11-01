pipeline {
    agent any

    environment {
        DOTNET_VERSION = '8.0'               // Specify .NET version (not directly used here)
        IIS_SITE_NAME = 'AksaStaging'        // The name of the IIS site
        PUBLISH_DIR = 'C:\\inetpub\\wwwroot\\JazzStaging'  // Local IIS publish directory
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore Dependencies') {
            steps {
                script {
                    // Restores project dependencies
                    bat 'dotnet restore'
                }
            }
        }

        stage('Build') {
            steps {
                script {
                    // Builds the application in Release mode
                    bat 'dotnet build --configuration Release'
                }
            }
        }

        stage('Run Tests') {
            steps {
                script {
                    // Runs tests in the project
                    bat 'dotnet test --no-build --verbosity normal'
                }
            }
        }

        stage('Publish') {
            steps {
                script {
                    // Publishes the application to the specified folder
                    bat "dotnet publish --configuration Release --output ${PUBLISH_DIR}"
                }
            }
        }

        stage('Deploy to IIS') {
            steps {
                script {
                    // Uses PowerShell to restart the IIS website after deploying
                    bat """
                    powershell.exe -Command "Import-Module WebAdministration; Stop-Website -Name '${IIS_SITE_NAME}'; Start-Website -Name '${IIS_SITE_NAME}'"
                    """
                }
            }
        }
    }

    post {
        always {
            cleanWs()
        }
        success {
            echo 'Deployment to IIS completed successfully!'
        }
        failure {
            echo 'Deployment failed. Check logs for more details.'
        }
    }
}
