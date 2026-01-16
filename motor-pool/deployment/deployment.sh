# Check usage
if [ "$#" -ne 3 ]; then
    echo "Usage: $0 <server-ip> <server-password> <path-to-env-file>"
    exit 1
fi

# Collect command line arguments
SERVER_IP="$1"
SERVER_PASSWORD="$2"
ENV_FILE_PATH="$3"

# Commands to run on the server
SSH_PREFIX="sshpass -p $SERVER_PASSWORD ssh root@$SERVER_IP"

echo "$SSH_PREFIX"

# Check for Git and Docker
echo "Checking for necessary software on the server..."
$SSH_PREFIX 'which git || (echo "Installing Git..."; apt-get update && apt-get install -y git)'
$SSH_PREFIX 'which docker || (echo "Installing Docker..."; apt-get update && apt-get install -y docker.io)'
$SSH_PREFIX 'which docker-compose || (echo "Installing Docker Compose..."; curl -L "https://github.com/docker/compose/releases/download/1.29.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose && chmod +x /usr/local/bin/docker-compose)'

# Set up the deployment directory
DEPLOY_DIR="/var/www/motor-pool"
$SSH_PREFIX "rm -rf $DEPLOY_DIR/* $DEPLOY_DIR/.* || true"
$SSH_PREFIX "mkdir -p $DEPLOY_DIR"

# Clone the repository
REPO_URL="https://github.com/vernon-gant/MotorPool.git"
echo "Cloning the repository..."
$SSH_PREFIX "git clone $REPO_URL $DEPLOY_DIR"

# Copy the environment file
echo "Copying environment variables..."
scp -o StrictHostKeyChecking=no $ENV_FILE_PATH root@$SERVER_IP:$DEPLOY_DIR/deployment/.env

# Stop and remove current Docker Compose containers, networks, and volumes
echo "Stopping existing Docker Compose containers..."
$SSH_PREFIX "cd '$DEPLOY_DIR/deployment' && docker-compose down"

# Pull the latest images from Docker Hub
echo "Pulling the latest Docker images..."
$SSH_PREFIX "cd '$DEPLOY_DIR/deployment' && docker-compose pull"

# Rebuild and force recreate Docker Compose containers
echo "Rebuilding all Docker Compose images and forcing recreation of containers..."
$SSH_PREFIX "cd '$DEPLOY_DIR/deployment' && docker-compose up --build --force-recreate -d"

# Start the app with Docker Compose
echo "Starting the application with Docker Compose..."
$SSH_PREFIX "cd '$DEPLOY_DIR/deployment' && docker-compose up -d"

echo "Deployment completed successfully."