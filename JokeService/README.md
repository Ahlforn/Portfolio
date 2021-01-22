# JokeService
## Projekt fra Distribueret Programmering kurset på Datamatiker studiet

Implementeret ved hjælp af Node.js, Express.js, MongoDB og PUG-templates.

Projektets formål først og fremmest at lave en joke database API, med et lille website hvor man kunne både læse og tilføje jokes til databasen igennem asynkrone kald til APIen. APIen struktur var dikteret i opgavebeskrivelsen sådan at den var standardiseret på tværs af holdets mange projekter. Det havde det formål at websitet også skulle kunne læse fra andre projekters API der var online. Information om hvilke projekter der var online var til rådighed fra en central register API. Derfor skulle hvert projekt registrer sig når det kom online og give besked når det blev lukket ned.