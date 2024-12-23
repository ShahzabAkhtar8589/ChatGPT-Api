pipeline {
    agent any

    environment {
        DOTNET_VERSION = '8.0'                   // Specify .NET version (not directly used here)
        IIS_SITE_NAME = 'AksaStaging'            // The name of the IIS site
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
                    bat 'dotnet restore ChatGPT-Api.sln'  // Restore project dependencies
                }
            }
        }

        stage('Build') {
            steps {
                script {
                    bat 'dotnet build ChatGPT-Api.sln --configuration Release'  // Build the solution in Release mode
                }
            }
        }

        stage('Run Tests') {
            steps {
                script {
                    bat 'dotnet test ChatGPT-Api.sln --no-build --verbosity normal'  // Run unit tests
                }
            }
        }

        stage('Publish') {
            steps {
                script {
                    bat "dotnet publish ChatGPT-Api.sln --configuration Release --output ${PUBLISH_DIR}"  // Publish to output directory
                }
            }
        }

        stage('Deploy to IIS') {
            steps {
                script {
                    // Restart IIS website after deployment
                    bat """
                    powershell.exe -Command "Import-Module WebAdministration; Stop-Website -Name '${IIS_SITE_NAME}'; Start-Website -Name '${IIS_SITE_NAME}'"
                    """
                }
            }
        }

        stage('SonarQube Analysis') {
    steps {
        withSonarQubeEnv('SonarQubeServer') {  // Use the server name configured in Jenkins
            script {
                // Begin SonarQube analysis
                bat "dotnet ${tool(name: 'SonarQubeScannerMSBuild', type: 'hudson.plugins.sonar.MsBuildSQRunnerInstallation')}\\SonarScanner.MSBuild.dll begin /k:\"TestProjectJenkin\""
                bat 'dotnet build ChatGPT-Api.sln --configuration Release'  // Build project for analysis
                // End SonarQube analysis
                bat "dotnet ${tool(name: 'SonarQubeScannerMSBuild', type: 'hudson.plugins.sonar.MsBuildSQRunnerInstallation')}\\SonarScanner.MSBuild.dll end"
            }
        }
    }
}

    }

    post {
        always {
            cleanWs()  // Clean workspace after build
        }
        success {
            echo 'Deployment to IIS completed successfully!'
        }
        failure {
            echo 'Deployment failed. Check logs for more details.'
        }
    }
}
