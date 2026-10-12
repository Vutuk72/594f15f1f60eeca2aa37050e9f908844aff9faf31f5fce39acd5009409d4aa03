# ekospol-miniprojekt

dokumentace:

stáhnul jsem si potřebné frameworky a propojil je, na jeden příkaz docker compose up.
Díky přístupu Ai do logů se problémy při konfiguraci vyřešily rychle.
dále bylo potřeba udělat data model v db, to jsem dělal přes migrace v Enitity Framework(EF).
seznámil jsem se s endpointama a udělal web
zjsištění nedostatku v projektu

hesla: admin, admin, user, user

## NEDOSTATKY:

1. problém: narazil jsem na ENTITY FRAMEWORK (ef) aplikace funguje ale úpravy tabulek sloupců v db se musí dělat v codu (migrací) a ne přímo v db, jde o to že aplikace neví co se změní v db (tabulky indexi relace, atd) a změny se nepropisují.
   rozhodnutí: používám pro nahlížení do db dbeaver a v něm mohu jen upravit data

2. problém: migrace která, která se už dostala k ostatním (push), nikdy neupravujte ani nemažte. Kolegové ji už mají v databázi a změna by jim to rozbila.
   rozhodnutí: dělat vždy nové migrace

3. věc keycloak je dobré rozhodnutí, s menším dodatkem používá vlastí db (h2), ta je pomalejší a nezvládne víc instancí Keycloaku najednou, v produkci doporučuju ukládat do posgresu

4. server nepropaguje frontend automaticky každá změna vyžeduje rebuild, je to otravné a zdržuje práci, bylo by vhodno vyřešit co nejdříve
   řešení pro vývoj ng serve

5. problém nepoužívat funkci AsNoTracking() pokud nevím co dělám, i když funkce ušetří paměť programu tak vede do pasti, pro práci s objekty je potřeba je mít v paměti a trekovat změny, kde by funkce mohla mít smysl že vytažení dat/reportů jen pro nahlídnutí s jedním ale co se staně opakovaném dotazování http požadavků?

6. upsert problém že Entity Framework(EF) neumí pořádně dělat upsert do db, dělá se to složení selct a pak insert, nebo update. tím vzniká race mezi dvěma požadavky. taková věc bude problém v produkci kde bude více uživatelů řešení je napsáním vlastního sqldotazu(INSERT … ON CONFLICT)

7. Entity Framework(EF) problém v průběhu programu si EF do paměti načte z db data, a zároveň si dělá snapshot původních dat. Efekt je pro malé množství dat zanedbatelný, ale pro velké množství objektů je to velký problém zhruba 2x když budu mít v poměti objekty 1GB tak realně budu mít 2GB, otázka je kolik paměti budete mít na serveru a kolik za to zaplatíte. řešení může být požít funkci AsNoTracking(), ale je to past

8. Entity Framework(EF) je anemický. Entita má jen get a set, pro příklad email nebo telefonní číslo potřebuje validaci dat Regex, tento regex se musí objevit všude kde se setuje email/tel.číslo když v budoucnu budete zavádět novej zdroj informace a budete chtít setovat email/tel.číslo musí se tam objevit regex jinak do setteru se bude moc uložit hloupost. časem kod bude nečitelný a bude se muset přepsat.
   admin po příhlášení je ukázka, kde se dá do tel.čísla uložit "ahoj"

Závěr projektu:
doufám že jsem splnil požadavky a očekávání, v nedostkách chci ukázat budoucí problémy v projektu. Můj cíl neí kritizovat projekt, vlastně technologický stack se mi líbí ale vidím v něm nedostatky, které projekt bude mít v budoucnu.

## Spuštění

Potřeba: Docker Desktop.

0. `git clone https://github.com/Vutuk72/594f15f1….git` a `cd` do složky
1. `cp .env.example .env` (Windows: `copy .env.example .env`)
2. `docker compose up -d --build`
3. Počkat, až naběhne Keycloak (může trvat několik minut)

| Co                 | Adresa                                    |
| ------------------ | ----------------------------------------- |
| Aplikace           | http://localhost:8080                     |
| API                | http://localhost:5000/api/...             |
| Keycloak admin     | http://localhost:8180 (kcadmin / zmen-me) |
| Databáze (DBeaver) | localhost:5432, appdb / appuser / zmen-me |

Testovací účty: `user` / `user` (role user), `admin` / `admin` (role user + admin)
