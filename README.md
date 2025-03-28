`Hej Rasmus,

Du kan starte Appen ved at køre følgende kommandoer:

cd SecureChatApp/
dotnet watch run

Dette åbner en browser på localhost:5252/login.hmtl så du kan logge ind.
Hvis du åbner endnu et vindue (gerne i incognito mode) kan du logge ind med 2 forskellige brugere og skrive beskeder til hinanden.


Burgernes credentials er:

Bob:
- Username: Bob@bob.com
- Password: S3cret1!

Alice:
- Username: A@alice.com
- Password: S3cret2!


Når begge brugere er logget ind, deriver de en shared key, og kan nu skrive beskeder til hinanden.
Beskederne er end-to-end encrypted, og kan kun læses af Bob og Alice.
Loggeren logger beskeder i deres crypterede form, i console så man kan verificere at serveren ikke modtager ucrpyteret data.
