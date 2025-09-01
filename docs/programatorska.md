# Programátorská dokumentace

## Obecné informace
- Implementace knihovny je rozdělena na dva soubory, RealMatrix.cs a Algorithms.cs.
- Spolu s knihovnou jsou k dispozici i základní testy, které by měly být průběžně rozšiřovány s přidanými funkcemi knihovny.

## Kódové konvence
- Styl kódu by měl zůstat stejný, t.j názvy tříd a metod jsou psány CamelCasem, názvy lokálních proměnných pascalCasem.
- Speciálně, parametry jsou prefixované t_, soukromé členy tříd m_ a vrácené proměnné prefixovány s r_.
- Je **vyloučeno** používat `try/catch/finally` bloky. Knihovna by měla výjimky vynášet ven do programu, který ji využívá, ale určitě je nikdy sama neřešit.

## Konkrétní pravidla pro soubor RealMatrix.cs
- V souboru RealMatrix.cs by se měl nacházet jen a pouze kód související přímo s tvorbou matic a maticovými operacemi.
- Tyto operace a funkce musí být **nutně** nijak nezasahující do podoby matice, od které se odvíjejí. Výsledek je vždy navrácen funkcí a původní matice zůstane stejná.
- Pokud to není vyloženě nevhodné, by měly smyčky a přístupy k datům matice být 1-indexované, jako je zvykem v matematických zápisech.
- Všechny funkce v tomto souboru by neměly být jakkoliv netriviální, či nepřipomínající přirozenou matematickou proceduru s maticemi.
- Je **zakázáno** přidávat do třídy `RealMatrix` netrivální fields, které narušují proces debugování.
  
## Konkrétní pravidla pro soubor Algorithms.cs
- Naproti tomu v souboru Algorithms.cs by měly být různé netriviální maticové procedury.
- Stejně jako v souboru Realmatrix.cs by měly smyčky a přístupy k prvkům matice být 1-indexované.
- Pokud je algoritmus v tomto souboru uveden v tabulce implementacích vybraných algoritmů, je nutné splnit jedno z následujících.
  1. Pokud je algoritmus již efektivní a funkční, neměl by být zaměněn za jiný, tudíž jsou povoleny pouze drobné úpravy, tento případ je značen písmenem **F**.
  2. Pokud algoritmus efektivní není, měl by být přepsán s ohledem na momentální vizi uvedenou v tabulce, tento případ je značem písmenem **R**.

## Alternativní řešení a diskuse volby algoritmů
K některým z řešených problémů lze přistupovat i jiným způsobem. Zde jsou vyjmenované některé z nich. K výpočtu norem je používán naivní výpočet přepon trojúhelníku. Ten lze nahradit chytřejším výpočtem, který využívá škálování prvků. Ten je sice o něco náročnější výpočetně a paměťově, ale poskytuje stabilní výpočet i v okrajových případech. K výpočtu determinantu je možné místo LU rozkladů použít víceméně výpočetní složitostí ekvivalentní  
Gaussovou eliminací. Stejně tak lze tento postup použít u řešení SLR, které momentálně spoléhají na maticové inverzy. Gaussova eliminace by v tomto případě umožňovala algoritmus rozšířit i pro jiné typy matic.  
Základní metoda QR algoritmu je založena na principu Householderových reflexí, které jsou v momentální formě explicitně konstruovány. Je možné algoritmus přepsat a konstruovat je implicitně, čímž by mělo být dosaženo podstateného zrychlení chodu algoritmu.  
Algoritmus na výpočet vlastních čísel využívá naivního teoretického postupu, který v některých případech selhává. Alternativní možností je použít algoritmus podobný, používající posuny a štěpení matic. To by poskytlo mimo jiné i podstatné zrychlení výpočtů. Nevýhodou tohoto postupu by bylo, že by se musela přidat zásadní podpora komplexních čísel.

## Postup prací a informace k testům
Knihovna vznikla postupným rozšiřování funkčnosti v tématických vrstvách. Základní vrstvou je samotná reprezentace a tvorba matic, spolu s přidruženými základními operacemi. Další vrstvou jsou různé specifické modifikace matic. Nad těmito vrstvami pracují algoritmy maticových rozkladů a podobnostních úprav, ze kterých vychází nejvyšší vrstva, kterou jsou specifické algoritmy na řešení rovnic, výpočty determinantů, vlastních čísel.
V momentě, kdy má být přidána nová funkčnost, nebo stávající být upravena, je vhodné pokračovat od nejnižších vrstev k nejvyšším. Tj. pokud lze vylepšit základní funkčnost, je to preferováno oproti specifickým úpravám vyšších vrstev.  
Dalším logickým krokem rozšíření knihovny je (krom oprav stávajících algoritmů) implementace Gaussovy eliminace, která by následně měla být využita k implementaci alternativních verzí algoritmů zmíněných v sekci výše. Dále je vhodné vylepšit algoritmus na výpočet vlastních čísel, se kterým by měla být přidána podpora komplexních čísel, a to v takové formě, která umožňuje v budoucnosti plynulý přechod k maticím, které obsahují i komplexní složky.  

**Testy** jsou důležitou součástí procesu vývoje. Neprocházející testy by měly být první věcí, která je v daný moment řešena, jelikož značí závažnou vadu v fungování programu. Vždy po přidání nové funkčnosti by měla být přidána alespoň základní sada pseudo-náhodných testů ověřující její správnost. Stejně tak při rozšiřování procedur stávajících je doporučeno staré testy rozšířit. Testy by měly být navíc, krom rozdělení podle testovaného modulu, být logicky sdružovány s dalšímy testy s podobnou funkčností.

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


## Závěr
V současném podání umožňuje knihovna pracovat s maticemi v rovině jednoduchých výpočtů a základní experimentace. Pro laické publikum je tato knihovna v praxi dostačující. To neznamená, že je z daleka perfektní. Mnoho algoritmů je implementováno příliš teoretickými postupy, které ve složitějších případech selhávají kvůli vysoké výpočetní složitosti. Je mým očekáváním, že s každou novou verzí knihovny budou tyto algoritmy postupně nahrazovány výpočetně vhodnějšími variantami.
