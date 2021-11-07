#!/usr/bin/env bash
podman run -d --name hub-db -p 3306:3306 -e MYSQL_ROOT_PASSWORD=pass mysql --default-authentication-plugin=mysql_native_password 
podman run -d --name hub-adminer -p 5200:8080 adminer
