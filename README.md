# MCTG

Semesterprojekt SWEN 1
Ines Mayer

## Setup

### Postgres DB als Docker Container:
docker run --name mctg_postgres -e POSTGRES_PASSWORD=swen1 -d -p 5432:5432 postgres

Create-Script ausführen (siehe Git-Repository) oder Tabellen manuell anlegen siehe Abbildung Tabellenschema.

## Application Design

Meine Applikation is logisch in 3 Layer unterteilt: 

- Presentation Layer (PL): Schnittstelle nach außen, Http-Server, Controller(definieren Endpoints)
- Business Layer (BL): Logik in Form von Services; definierte Models
- Data Access Layer (DAL): Repositories inkl. jeweiligen Interface, PostgresAccess zur Verwaltung der DB-Connection

Weiteres beinhaltet meine Projektstruktur folgende Ordner:
 - Utility/Json: Zum Parsen von Json benötigte Models
 - Exceptions: Custom Exceptions

### Database

Beim Erstellen der Datenbank habe ich mein Wissen aus der LV "Datenmanagement" angewendet und mir zu Beginn zu besseren Visualisierung ein ERM für den konkreten Sachverhalt erstellt. Dies half mit dabei eine sinnvolle Tabellenstruktur zu entwickeln.  
Nachfolgende Abbildungen zeigen mein ERM, sowie die schlussendlich tatsächlich erstellten Tabellen.

![ERM Entwurf](./doku_img/erm.jpg)
![Tatsächliche DB](./doku_img/db.PNG)


Zugriff auf die Datenbank erfolgt via Repositories. Ich habe mich aktiv mit dem Repository Pattern beschäftigt.  

Durch Prepared Statements verhindere ich SQL Injections. Zudem arbeite ich teilweise mit Transactions um zu garatnieren, dass gewisse Operationen, wie etwa das Kaufen eine Packages als Ganzes ausgeführt wird oder garnicht (e.g. User soll keine Coins verlieren ohne die jeweiligen Packages)

Zudem habe ich darauf geachtet, dass zu jedem Zeitpunkt nur eine einzige Verbindung zur Datenbank existiert. Um Race.
DAzu habe ich die Klasse "PostgresDBAccess" erstellt.

### HttpServer & Routing

Für den Http-Server habe ich das Beispiel aus der Vorlesung abgeändert. Die grundlegende Logik zum Anhandeln der Clients wurde beibehalten. Den Http-Parser habe ich erweitert, sodass auch der Content von Post-Requests geparset wird. Zudem habe ich HttpRequest 
Http-Request,Zum Senden von Http-Response habe ich die Klasse. 

Router-Klasse enthält Dictonaryies für jeden (verwendeten) Http. e.g. PostRoutes, GetRoutes. MErhere Endpoints Rpute

## Register, Login, Token

User können sich registrieren
Simple Validierung der Userdaten.
Username muss unique sein.

## Game Logic
Statt einer Route /battle gibt es 2 Routen:

/battle/new -> Erstellt eine neue BattleRequst, Battle wird gestartet, sobald ein weiterer Spieler mit /battle/join die BattleRequest annimmt
/battle/join -> Suche für eine Minute nach offenen BattleRequests

Events & Pooling.

## Stats und Scoreboard

Verwaltung des Scoreboards als eigene Tabelle in der Datenbank. 

/states -> Return 
/score -> Return Scoreboard, sortiert nach Elo


## User Profile

Nicht implementiert


## Trading

Nicht implementiert


## Special Feature: Multiple Decks 

- User können mehrere Decks besitzen
- Erstellen eines neuen Decks via /deck/add (POST)
- Alle Decks eines Users auflisten /deck/all (GET)
- Aktives Deck setzen /deck (PUT)
- Aktives Deck anzeigen /deck (GET)
- Karten können Teil von mehreren Decks sein, da immer nur eins gespielt wird

## Weitere Erweiterungen

- Passwort als Hashwert in der DB
- Card Descriptions

# Änderungen im Curl-Script (Datei im Git-Repository)

- Json zum Erstellen der Karten abgeändert
- /login statt /sessions
- /register statt /users
- neue Routen zum starten bzw. joinen eines Battles
- neue Routen zum Erstellen, Wechseln eines Decks

## Unit Testing Decisions

Fokus meiner Unittests liegt beim Testen der Game-Logik, da diese aufgrund verzweigter if-else-Statements sehr fehleranfällig ist und auch nur schwer manuell überprüft werden kann.

Zudem habe ich meine UserService und CardService Klasse getestet, da diese integraler Bestandteil des Programms sind. 

## Lessons learned

- Unit Tests nebenbei machen (Stichwort TDD) und nicht erst am Ende des Projekts: Unit-Tests am Ende machen ist sehr zeitaufwenig, insebesondere wenn sich der Code nicht gut testen lässt (aufgrund e.g vieler Dependencies, private Methoden, ...). Da dieses Projekt mein erstes Projekt mit C# ist, habe ich mich mehr auf die Umsetzung fokusiert, und die Tests zum Schluss aufgehoben.
- Arbeiten mit Git verbesserungswürdig: Da ich bisher noch nie wirklich produktiv mit Git gearbeitet habe, war der Workflow sehr ungewohnt für mich. Ich habe öfter auf das Commiten Vergessen.  
- Durchs selbst Programmieren lernt man am besten: Deutliche Verbesserung meiner C# Kenntnisse da ich zum ersten Mal in diesem Semester die Zeit gefunden habe mich wiklich aktiv mit C# zu beschäftigen.

## Tracked Time

Keine Aufzeichnung

## Link zu GitRepos

https://github.com/xxaemy/MCTG