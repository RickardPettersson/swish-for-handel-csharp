# Swish för handel test projekt byggt i C# ASP.NET MVC

---

Detta är ett ASP.NET MVC projekt som använder ett .Net Standard Library som heter [SwishApi](https://github.com/RickardPettersson/swish-api-csharp) som går att installera från [NuGet](https://www.nuget.org/packages/SwishApi/) eller hitta senaste koden på [Github](https://github.com/RickardPettersson/swish-api-csharp).

## Testa

Projektet är ett ASP.NET MVC 5 projekt med Razor vyer och allt skriver i C# och Visual Studio 2017.

Om man laddar ner koden så är det förinställt att bara fungera och köra mot Getswish AB´s testmiljö med test certifikat och uppgifter.

Däremot är projektet förberett så du kan lägga in ditt egna produktions miljö certifikat och uppgifter i web.config och köra projektet i deras produktionsmiljö.

# Uppdateringar

2019-05-26 - Har nu uppdaterat test certifikatet och fått koden att fungera att köra en "PaymentRequest" och en "PaymentStatusCheck" mote senaste Swish Test API:et

2019-05-30 - Nu har jag lanserat [SwishApi](https://github.com/RickardPettersson/swish-api-csharp) som är ett .Net Standard Library och installeras genom [NuGet](https://www.nuget.org/packages/SwishApi/) och dettta repository är nu uppdaterat att använda detta class library.

# Öppet för alla
Projektet i dettta repository finns som en öppen website på http://www.tabetaltmedswish.se där ni kan läsa mera om detta projekt och hur ni får allt att fungera i produktionsmiljö.

Jag som gjort detta projekt heter Rickard Nordström Pettersson och ni hittar mina kontaktuppgifter på http://www.rickardp.se