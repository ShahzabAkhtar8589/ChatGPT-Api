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
                    bat 'dotnet restore ChatGPT-Api.sln' // Specify the solution or project file
                }
            }
        }

        stage('Build') {
            steps {
                script {
                    // Builds the application in Release mode
                    bat 'dotnet build ChatGPT-Api.sln --configuration Release' // Specify the solution
                }
            }
        }

        stage('Run Tests') {
            steps {
                script {
                    // Runs tests in the project
                    bat 'dotnet test ChatGPT-Api.sln --no-build --verbosity normal' // Specify the solution
                }
            }
        }

        stage('Publish') {
            steps {
                script {
                    // Publishes the application to the specified folder
                    bat "dotnet publish ChatGPT-Api.sln --configuration Release --output ${PUBLISH_DIR}" // Specify the solution
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

    node {
  stage('SCM') {
    checkout scm
  }
  stage('SonarQube Analysis') {
    def scannerHome = tool 'SonarScanner for MSBuild'
    withSonarQubeEnv() {
      bat "dotnet ${scannerHome}\\SonarScanner.MSBuild.dll begin /k:\"TestProjectJenkin\""
      bat "dotnet build"
      bat "dotnet ${scannerHome}\\SonarScanner.MSBuild.dll end"
    }
  }
}

}
