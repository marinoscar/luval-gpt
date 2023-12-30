#!/bin/bash
echo "Stopping the service..."
sudo systemctl stop gpt_marin_cr-app.service
echo "Removing the old files..."
sudo rm -rf /var/www/gpt_marin_cr/*
echo "Unzipping the new package..."
sudo unzip '/var/www/packages/gpt.zip' -d '/var/www/gpt_marin_cr'
echo "Restarting the web server..."
sudo systemctl restart nginx
echo "Restarting the web app..."
sudo systemctl start gpt_marin_cr-app.service
echo "Update process completed."
