# Swish för handel test projekt byggt i C# ASP.NET MVC
Getswish AB har lanserat Swish för handel men har inte släppt några kodexempel förutom cURL exempel vilket gör det svårt att testa i Windows och att implementera Swish för handel i sitt programmerings projekt.

Så när jag nu lagt ner väldigt många timmar för att få Swish för handel att fungera i C# och ASP.NET MVC så har jag valt att släppa detta test projekt öppet på GIthub så andra kan ha det att kolla på när dem gör sin implementation.

Projektet är ett ASP.NET MVC 5 projekt med Razor vyer och allt skriver i C# och Visual Studio 2017.

Om man laddar ner koden så är det förinställt att bara fungera och köra mot Getswish AB´s testmiljö med test certifikat och uppgifter.

Däremot är projektet förberett så du kan lägga in ditt egna produktions miljö certifikat och uppgifter i web.config och köra projektet i deras produktionsmiljö.

# Uppdateringar
Uppdatering 2019-05-26 - Har nu uppdaterat test certifikatet och fått koden att fungera att köra en "PaymentRequest" och en "PaymentStatusCheck" mote senaste Swish Test API:et

# Öppet för alla
Projektet i dettta repository finns som en öppen website på http://www.tabetaltmedswish.se där ni kan läsa mera om detta projekt och hur ni får allt att fungera i produktionsmiljö.

Jag som gjort detta projekt heter Rickard Nordström Pettersson och ni hittar mina kontaktuppgifter på http://www.rickardp.se