# Programátorská dokumentace

## Obecné informace
- Implementace knihovny je rozdělena na dva soubory, RealMatrix.cs a Algorithms.cs
- Spolu s knihovnou jsou k dispozici i základní testy, které by měly být průběžně rozšiřovány s přidanými funkcemi knihovny

## Kódové konvence
- Styl kódu by měl zůstat stejný, t.j názvy tříd a metod jsou psány CamelCasem, názvy lokálních proměnných pascalCasem.
- Speciálně, parametry jsou prefixované t_, soukromé členy tříd m_ a vrácené proměnné prefixovány s r_
- Je **vyloučeno** používat `try/catch/finally` bloky. Knihovna by měla výjimky vynášet ven do programu, který ji využívá, ale určitě je nikdy sama neřešit.

## Konkrétní pravidla pro soubor RealMatrix.cs
- V souboru RealMatrix.cs by se měl nacházet jen a pouze kód související přímo s tvorbou matic a maticovými operacemi.
- Tyto operace a funkce musí být **nutně** nijak nezasahující do podoby matice, od které se odvíjejí. Výsledek je vždy navrácen funkcí a původní matice zůstane stejná.
- Pokud to není vyloženě nevhodné, by měly smyčky a přístupy k datům matice být 1-indexované, jako je zvykem v matematických zápisech.
- Všechny funkce v tomto souboru by neměly být jakkoliv netriviální, či nepřipomínající přirozenou matematickou proceduru s maticemi
- Je **zakázáno** přidávat do třídy `RealMatrix` netrivální fields, které narušují proces debugování.


## Konkrétní pravidla pro soubor Algorithms.cs
- Naproti tomu v souboru Algorithms.cs by měly být různé netriviální maticové procedury
- Stejně jako v souboru Realmatrix.cs by měly smyčky a příštupy k prvkům matice být 1-indexované.
- Pokud je algoritmus v tomto souboru uveden v tabulce implementacích vybraných algoritmů, je nutné splnit jedno z následujících
  1. Pokud je algoritmus již efektivní a funkční, neměl by být zaměněn za jiný, tudíž jsou povoleny pouze drobné úpravy, tento případ je značen písmenem **F**
  2. Pokud algoritmus efektivní není, měl by být přepsán s ohledem na momentální vizi uvedenou v tabulce, tento případ je značem písmenem **R**




### Tabulka implementací vybraných algoritmů
| Algoritmus | Stav | Popis |
| ----------- | ----------- | ----------- |
| Norm | R | Norma by měla zůstat Frobeniova, ovšem hrozí pod/přeteční |
| QR | F | Funkce na QR rozklad by v obecném případě měla zůstat implementována pomocí Householderových reflexí |
| GivensQR | F | QR rozklad pomocí Givensových rotací musí být optimalizován na řidké (či Hessenbergovy) matice |
| Hessenberg | F | |
| Triangular | R | Tento algoritmus by měl být v budoucnu nahrazen o poznání složitějším algoritmem, který používá Wilkinsonovy posuny |
| BackwardTriangle / ForwardTriangle | R | V budoucnu by měly být přepsány, aby podporovaly i nereguálární matice |
| Inverse | F | |
| LinSys | R | Měl by být přepsán aby podporoval i neregulární matice|
| InternalLUP | R | Je možnost nahradit čtvrtý parametr chytřejší metodou na určení determinantu permutační matice |
| Determinant | F/R | Měl by i nadále využívat LU rozklad z knihovny. Pokud bude v budoucnu přidána Gaussova eliminace, měl by používat tu|
