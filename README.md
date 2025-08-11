## MatrixLibrary
---
### Only Czech description available.
---
### Základní maticová knihovna

*Specifikace:*
> - Souborové, datové I/O  
> - Reprezentace matice reálných čísel, základní maticové operace, manipulování s existujícími maticemi  
> - Výpočet norem, determinántů a realných vlastních čísel matic  
> - Základní algoritmy, LU a QR rozklad, inverz matice, řešení SLR udáno regulární maticí  
> - Sada základních testů k zajištění funkčnosti knihovny v případě změn či rozšíření  

*Co se do knihovny nevešlo / co by šlo vylepšit:*
> - SLR udané maticemi jinými než regulárními
> - Algoritmus na výpočet vlastních čísel je naivní, ne vždy konverguje a není schopen počítat komplexní vlastní čísla
> - Některé části knihovny jsou numericky nestabilní, např. výpočet norem či Givensových rotací není ošetřen proti pod/přeteční
