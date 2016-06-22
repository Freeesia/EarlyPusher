@echo off
set BASEDIR=%~dp0

pushd %BASEDIR%
powershell -NoProfile -ExecutionPolicy Unrestricted .\build.ps1
popd