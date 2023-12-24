#!/bin/bash
echo "Stop container"
docker stop meterreadingsbot
docker rm meterreadingsbot
docker image rm kest3107/meterreadingsbot
echo "Pull image"
docker pull kest3107/meterreadingsbot
echo "Start frontend container"
docker run --name meterreadingsbot -d kest3107/meterreadingsbot
echo "Finish deploying!"
