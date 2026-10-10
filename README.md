# ekospol-miniprojekt

1. problém narazil jsem na ENTITI FRAMEWORK (ef) aplikace funguje ale úpravy tabulek sloupců v db se musí dělat v codu (migrací) a ne přímo v db, jde o to že aplikace neví co se změní v db (tabulky indexi relace, atd) a změny se nepropisují.
   rozhodnutí používám pro db dbeaver a v něm mohu jen upravit data

2. problém migrace která, která se už dostala k ostatním (push), nikdy neupravujte ani nemažte. Kolegové ji už mají v databázi a změna by jim to rozbila
   rozhodnutí dělat vždy nové migrace

3. věc keycloak je dobré rozhodnutí, s menším dodatkem používá vlastí db (h2), ta je pomalejší a nezvládne víc instancí Keycloaku najednou, v produkci doporučuju ukládat do posgresu

4. problém nepoužívat funkci AsNoTracking() pokud nevím co dělám, i když funkce ušetří paměť programu tak vede do pasti, pro práci s objekty je potřeba je mít v paměti a trekovat změny, kde by funkce mohla mít smysl že vytažení dat/reportů jen pro nahlídnutí s jedním ale co se staně opakovaném dotazování http požadavků?

5. upsert ToDo dodělat
