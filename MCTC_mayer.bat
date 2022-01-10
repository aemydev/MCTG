@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo.

REM --------------------------------------------------
echo 1) Create Users (Registration)
REM Create User
curl -X POST http://localhost:10001/register --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo.
curl -X POST http://localhost:10001/register --header "Content-Type: application/json" -d "{\"Username\":\"altenhof\", \"Password\":\"markus\"}"
echo.
curl -X POST http://localhost:10001/register --header "Content-Type: application/json" -d "{\"Username\":\"admin\",    \"Password\":\"istrator\"}"
echo.

echo should fail:
curl -X POST http://localhost:10001/register --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo.
curl -X POST http://localhost:10001/register --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"different\"}"
echo. 
echo.

REM --------------------------------------------------
echo 2) Login Users
curl -X POST http://localhost:10001/login --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo.
curl -X POST http://localhost:10001/login --header "Content-Type: application/json" -d "{\"Username\":\"altenhof\", \"Password\":\"markus\"}"
echo.
curl -X POST http://localhost:10001/login --header "Content-Type: application/json" -d "{\"Username\":\"admin\",    \"Password\":\"istrator\"}"
echo.

echo should fail:
curl -X POST http://localhost:10001/login --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"different\"}"
echo.
curl -X POST http://localhost:10001/login --header "Content-Type: application/json" -d "{\"Username\":\"unknown\", \"Password\":\"secret\"}"
echo.

REM --------------------------------------------------
echo 3) create packages (done by "admin")
curl -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Basic admin-mtcgToken" -d "[{\"Id\":\"cc8cae97-be23-4b1e-8bf6-5228385aaf8a\",\"Name\":\"Water Spell\",\"Description\":\"Description\",\"Damage\":11,\"Type\":1,\"ElementType\":1},{\"Id\":\"6271d7b3-2e22-4ced-a7e8-3ddfc5a08125\",\"Name\":\"Fire Spell\",\"Description\":\"Description\",\"Damage\":70,\"Type\":1,\"ElementType\":2},{\"Id\":\"01626fb0-6133-4360-bb9b-fb1aa71ca09f\",\"Name\":\"Normal Spell\",\"Description\":\"Description\",\"Damage\":22,\"Type\":0,\"ElementType\":1},{\"Id\":\"e2deb4ac-3e72-48f8-8681-ea70b8c6ae02\",\"Name\":\"Dragon\",\"Description\":\"Description\",\"Damage\":40,\"Type\":1,\"ElementType\":1},{\"Id\":\"87b7ebfe-dbec-4118-ad64-fa5d1347a27b\",\"Name\":\"Fire Elve\",\"Description\":\"Description\",\"Damage\":28,\"Type\":0,\"ElementType\":0}]"
echo.																																																																																		 				    
curl -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Basic admin-mtcgToken" -d "[{\"Name\":\"Goblin\",\"Description\":\"Description\",\"Damage\":11,\"Type\":1,\"ElementType\":1},{\"Name\":\"Wizard\",\"Description\":\"Description\",\"Damage\":70,\"Type\":1,\"ElementType\":1},{\"Name\":\"Orc\",\"Description\":\"Description\",\"Damage\":22,\"Type\":0,\"ElementType\":1},{\"Name\":\"Knights\",\"Description\":\"Description\",\"Damage\":40,\"Type\":1,\"ElementType\":1},{\"Name\":\"Kraken\",\"Description\":\"Description.\",\"Damage\":28,\"Type\":0,\"ElementType\":0}]"
echo.																																																																																		 				    
curl -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Basic admin-mtcgToken" -d "[{\"Name\":\"Goblin\",\"Description\":\"Description\",\"Damage\":11,\"Type\":1,\"ElementType\":1},{\"Name\":\"Wizard\",\"Description\":\"Description\",\"Damage\":70,\"Type\":1,\"ElementType\":1},{\"Name\":\"Orc\",\"Description\":\"Description\",\"Damage\":22,\"Type\":0,\"ElementType\":1},{\"Name\":\"Knights\",\"Description\":\"Description\",\"Damage\":40,\"Type\":1,\"ElementType\":1},{\"Name\":\"Kraken\",\"Description\":\"Description.\",\"Damage\":28,\"Type\":0,\"ElementType\":0}]"
echo.																																																																																		 				    
curl -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Basic admin-mtcgToken" -d "[{\"Name\":\"Goblin\",\"Description\":\"Description\",\"Damage\":11,\"Type\":1,\"ElementType\":1},{\"Name\":\"Wizard\",\"Description\":\"Description\",\"Damage\":70,\"Type\":1,\"ElementType\":1},{\"Name\":\"Orc\",\"Description\":\"Description\",\"Damage\":22,\"Type\":0,\"ElementType\":1},{\"Name\":\"Knights\",\"Description\":\"Description\",\"Damage\":40,\"Type\":1,\"ElementType\":1},{\"Name\":\"Kraken\",\"Description\":\"Description.\",\"Damage\":28,\"Type\":0,\"ElementType\":0}]"
echo.																																																																																		 				    																																																																																		 				    
echo.

REM --------------------------------------------------
echo 4) acquire packages kienboec
curl -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Basic kienboec-mtcgToken" -d ""
echo.
curl -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Basic kienboec-mtcgToken" -d ""
echo.
curl -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Basic kienboec-mtcgToken" -d ""
echo.
curl -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Basic kienboec-mtcgToken" -d ""
echo.
echo should fail (no money):
curl -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Basic kienboec-mtcgToken" -d ""
echo.
echo.

REM --------------------------------------------------
echo 5) acquire packages altenhof
echo should fail (no package):?
curl -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Basic altenhof-mtcgToken" -d ""
echo.
echo.

REM --------------------------------------------------
echo 6) add new packages
curl -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Basic admin-mtcgToken" -d "[{\"Name\":\"Goblin\",\"Description\":\"Description\",\"Damage\":11,\"Type\":1,\"ElementType\":1},{\"Name\":\"Wizard\",\"Description\":\"Description\",\"Damage\":70,\"Type\":1,\"ElementType\":1},{\"Name\":\"Orc\",\"Description\":\"Description\",\"Damage\":22,\"Type\":0,\"ElementType\":1},{\"Name\":\"Knights\",\"Description\":\"Description\",\"Damage\":40,\"Type\":1,\"ElementType\":1},{\"Name\":\"Kraken\",\"Description\":\"Description.\",\"Damage\":28,\"Type\":0,\"ElementType\":0}]"
echo.
curl -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Basic admin-mtcgToken" -d "[{\"Name\":\"Goblin\",\"Description\":\"Description\",\"Damage\":11,\"Type\":1,\"ElementType\":1},{\"Name\":\"Wizard\",\"Description\":\"Description\",\"Damage\":70,\"Type\":1,\"ElementType\":1},{\"Name\":\"Orc\",\"Description\":\"Description\",\"Damage\":22,\"Type\":0,\"ElementType\":1},{\"Name\":\"Knights\",\"Description\":\"Description\",\"Damage\":40,\"Type\":1,\"ElementType\":1},{\"Name\":\"Kraken\",\"Description\":\"Description.\",\"Damage\":28,\"Type\":0,\"ElementType\":0}]"
echo.
curl -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Basic admin-mtcgToken" -d "[{\"Id\":\"0d868225-6ecb-4420-9ed0-60347fa2d603\",\"Name\":\"Water Spell\",\"Description\":\"Description\",\"Damage\":11,\"Type\":1,\"ElementType\":1},{\"Id\":\"38656ea7-8325-4e58-9537-146b0f144a57\",\"Name\":\"FireSpell\",\"Description\":\"Description\",\"Damage\":70,\"Type\":1,\"ElementType\":2},{\"Id\":\"23f74038-90f4-4e37-b30f-03ad8b443b70\",\"Name\":\"Normal Spell\",\"Description\":\"Description\",\"Damage\":22,\"Type\":0,\"ElementType\":1},{\"Id\":\"c6e93c35-f963-43d8-bc19-3015d33bbc76\",\"Name\":\"Dragon\",\"Description\":\"Description\",\"Damage\":40,\"Type\":1,\"ElementType\":1},{\"Id\":\"6036d512-c58b-4a39-bc77-6dc1a51d538a\",\"Name\":\"Fire Elve\",\"Description\":\"Description\",\"Damage\":28,\"Type\":0,\"ElementType\":0}]"
echo.
curl -X POST http://localhost:10001/packages --header "Content-Type: application/json" --header "Authorization: Basic admin-mtcgToken" -d "[{\"Name\":\"Goblin\",\"Description\":\"Description\",\"Damage\":11,\"Type\":1,\"ElementType\":1},{\"Name\":\"Wizard\",\"Description\":\"Description\",\"Damage\":70,\"Type\":1,\"ElementType\":1},{\"Name\":\"Orc\",\"Description\":\"Description\",\"Damage\":22,\"Type\":0,\"ElementType\":1},{\"Name\":\"Knights\",\"Description\":\"Description\",\"Damage\":40,\"Type\":1,\"ElementType\":1},{\"Name\":\"Kraken\",\"Description\":\"Description.\",\"Damage\":28,\"Type\":0,\"ElementType\":0}]"
echo.
echo.

REM --------------------------------------------------
echo 7) acquire newly created packages altenhof
curl -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Basic altenhof-mtcgToken" -d ""
echo.
curl -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Basic altenhof-mtcgToken" -d ""
echo.
curl -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Basic altenhof-mtcgToken" -d ""
echo.
curl -X POST http://localhost:10001/transactions/packages --header "Content-Type: application/json" --header "Authorization: Basic altenhof-mtcgToken" -d ""
echo.
echo.

REM --------------------------------------------------
echo 8) show all acquired cards kienboec
curl -X GET http://localhost:10001/cards --header "Authorization: Basic kienboec-mtcgToken"
echo should fail (no token)
curl -X GET http://localhost:10001/cards 
echo.
echo.

REM --------------------------------------------------
echo 9) show all acquired cards altenhof
curl -X GET http://localhost:10001/cards --header "Authorization: Basic altenhof-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 10) show unconfigured deck
curl -X GET http://localhost:10001/deck --header "Authorization: Basic kienboec-mtcgToken"
echo.
curl -X GET http://localhost:10001/deck --header "Authorization: Basic altenhof-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 11) show all decks (none)
curl -X GET http://localhost:10001/deck/all --header "Authorization: Basic kienboec-mtcgToken"
echo.
curl -X GET http://localhost:10001/deck/all --header "Authorization: Basic altenhof-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 11) create new deck kienboec
curl -X POST http://localhost:10001/deck/add --header "Authorization: Basic kienboec-mtcgToken" -d "{\"Id\":\"07311901-09d0-4879-8781-e3d9ad2c6f62\",\"Title\":\"supercooles_deck\",\"Cards\":[\"cc8cae97-be23-4b1e-8bf6-5228385aaf8a\",\"6271d7b3-2e22-4ced-a7e8-3ddfc5a08125\",\"01626fb0-6133-4360-bb9b-fb1aa71ca09f\",\"e2deb4ac-3e72-48f8-8681-ea70b8c6ae02\",\"87b7ebfe-dbec-4118-ad64-fa5d1347a27b\"]}"
echo.
echo.

REM --------------------------------------------------
echo 11) create new deck altenhof
curl -X POST http://localhost:10001/deck/add --header "Authorization: Basic altenhof-mtcgToken" -d "{\"Id\":\"6036d512-c58b-4a39-bc77-12c1a51d538a\",\"Title\":\"altenhof_winner_deck\",\"Cards\":[\"0d868225-6ecb-4420-9ed0-60347fa2d603\",\"38656ea7-8325-4e58-9537-146b0f144a57\",\"23f74038-90f4-4e37-b30f-03ad8b443b70\",\"c6e93c35-f963-43d8-bc19-3015d33bbc76\",\"6036d512-c58b-4a39-bc77-6dc1a51d538a\"]}"
echo.
echo.

REM --------------------------------------------------
echo 11) show all decks -> new decks
curl -X GET http://localhost:10001/deck/all --header "Authorization: Basic kienboec-mtcgToken"
echo.
curl -X GET http://localhost:10001/deck/all --header "Authorization: Basic altenhof-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 11) set new deck as active deck
curl -X PUT http://localhost:10001/deck --header "Authorization: Basic kienboec-mtcgToken" -d "{\"Id\":\"07311901-09d0-4879-8781-e3d9ad2c6f62\"}"
echo.
curl -X PUT http://localhost:10001/deck --header "Authorization: Basic altenhof-mtcgToken" -d "{\"Id\":\"6036d512-c58b-4a39-bc77-12c1a51d538a\"}"
echo.
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
