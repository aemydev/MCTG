@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game, Battle Test
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo.

REM --------------------------------------------------
echo 12) show configured deck 
curl -X GET http://localhost:10001/deck --header "Authorization: Basic kienboec-mtcgToken"
echo.
curl -X GET http://localhost:10001/deck --header "Authorization: Basic altenhof-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 13) show configured deck different representation
echo not implemented
echo.
echo.

REM --------------------------------------------------
echo 14) edit user data
echo not implemented
echo.
echo.

REM --------------------------------------------------
echo 15) stats
curl -X GET http://localhost:10001/stats --header "Authorization: Basic kienboec-mtcgToken"
echo.
curl -X GET http://localhost:10001/stats --header "Authorization: Basic altenhof-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 16) scoreboard
curl -X GET http://localhost:10001/score --header "Authorization: Basic kienboec-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 17) battle
start /b "altenhof battle" curl -X GET http://localhost:10001/battle/new --header "Authorization: Basic altenhof-mtcgToken"
ping localhost -n 5 >NUL 2>NUL
start /b "kienboec battle" curl -X GET http://localhost:10001/battle/find --header "Authorization: Basic kienboec-mtcgToken"
ping localhost -n 10 >NUL 2>NUL
echo.

REM --------------------------------------------------
echo 18) Stats 
echo kienboec
curl -X GET http://localhost:10001/stats --header "Authorization: Basic kienboec-mtcgToken"
echo.
echo altenhof
curl -X GET http://localhost:10001/stats --header "Authorization: Basic altenhof-mtcgToken"
echo.

REM --------------------------------------------------
echo 19) scoreboard
curl -X GET http://localhost:10001/score --header "Authorization: Basic kienboec-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 20) trade
echo not implemented

REM --------------------------------------------------
echo end...

REM this is approx a sleep 
ping localhost -n 100 >NUL 2>NUL
@echo on
