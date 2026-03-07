@echo off
setlocal enabledelayedexpansion

for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c%%a%%b)
for /f "tokens=1-2 delims=/:" %%a in ('time /t') do (set mytime=%%a%%b)

set foldername=test_%mydate%%mytime%

mkdir "./test_output/%foldername%"
jmeter -n -t ./tests/load-test.jmx -l "./test_output/%foldername%/results.jtl" -e -o "./test_output/%foldername%/report"
