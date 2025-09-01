# Uživatelská dokumentace

| Modul | Obsah |
| ----------- | ----------- |
| RealMatrix | Tvoření matic, modifikování a ořezávání matic, základní maticové operace|
| Algorithms | Rozklady matic, Inverz matice a řešení SLR, výpočet determinantu a vlastních čísel |  

## Příklad fungování knihovny

Zde je příklad využití knihovny, kde je nejprve vytvořena Hilbertova matice, na které je demonstrováno použití QR rozkladu obdelníkové matice.
Dále je ukázáno "ořezání" této matice do čtvercové podoby, na které lze ukázat výpočet determinantu.

```csharp
        static void Example()
        {
            // Nastavení přesnosti výpočtu na 10. desetinných míst
            Algorithms.Precision = 10;
        
            (int height, int width) = (3, 5);

            // Inicializace dat pro Hilbertovu matici
            double[,] Hilbert = new double[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Hilbert[i, j] = 1.0 / (i + j + 1);
                }
            }

            // Tvorba objektu Realmatrix z existujících dat
            RealMatrix HilbertMatrix = RealMatrix.From(Hilbert);

            Console.WriteLine("The original Hilbert Matrix:");
            HilbertMatrix.Print();
            Console.WriteLine();
            // Výpočet faktorů Q,R rozkladu matice
            (RealMatrix Q, RealMatrix R) = Algorithms.QR(HilbertMatrix);
            
            Q.Print();
            Console.WriteLine(" *");
            R.Print();
            Console.WriteLine(" =");
            (Q * R).Print();
            Console.WriteLine();

            // Demonstrace extrahování podmatic
            RealMatrix SquareHilbert = HilbertMatrix.SubMatrix(1, 3, 1, 3);
            Console.WriteLine("Extracted 3x3 Hilbert Matrix:");
            SquareHilbert.Print();

            // Výpočet determinantu ze čtvercové matice
            Console.WriteLine($"Computed determinant of a 3x3 square Hilbert matrix is: {Algorithms.Determinant(SquareHilbert)}");
            Console.WriteLine($"The correct value should be: {1.0 / 2160}");

        }
```
Výstup:
```
The original Hilbert Matrix:
            1           0,5  0,3333333333          0,25           0,2
          0,5  0,3333333333          0,25           0,2  0,1666666667
 0,3333333333          0,25           0,2  0,1666666667  0,1428571429

-0,8571428571  0,5016049166 -0,1170411472
-0,4285714286 -0,5684855721  0,7022468832
-0,2857142857 -0,6520863915 -0,7022468832
 *
-1,1666666667 -0,6428571429         -0,45 -0,3476190476 -0,2836734694
            0 -0,1017143303 -0,1053370325 -0,0969769505 -0,0875818108
            0             0 -0,0039013716 -0,0058520574 -0,0066880656
 =
 1,0000000000  0,5000000000  0,3333333333  0,2500000000  0,2000000000
 0,5000000000  0,3333333334  0,2500000000  0,2000000000  0,1666666666
 0,3333333333  0,2500000000  0,2000000000  0,1666666667  0,1428571429

Extracted 3x3 Hilbert Matrix:
            1           0,5  0,3333333333
          0,5  0,3333333333          0,25
 0,3333333333          0,25           0,2
Computed determinant of a 3x3 square Hilbert matrix is: 0,000462963
The correct value should be: 0,000462962962962963
```



## Obecné principy fungování knihovny
- Všechny funkce zachovávají původní stav matice, tj. výsledek je vždy vrácen a je nutno ho přiřadit.
- Funkce mohou (a dělají to často) vyvolávat chyby. Ty by neměly být řešeny, nýbrž by jim mělo být čištěním vstupů být předcházeno!

## Modul RealMatrix
- Atributy a zobrazení matice
  - `int Width { get; private set; }`
      - Vrací šířku matice
  - `int Height { get; private set; }`
      - Vrací výšku matice
  - `double Norm`
      - Vrací euklidovskou (pro vektory) resp. Frobéniovu (pro matice) normu
  - `void Print()`
      - Vypíše formátovaně prvky matice

- "Konstruktory"
  - `static RealMatrix Zeros(int t_Length)`
  - `static RealMatrix Zeros(int t_Height, int t_Width)`
      - Vrací nulovou matici požadovaných dimenzí
  - `static RealMatrix Eye(int t_Length)`
  - `static RealMatrix Eye(int t_Height, int t_Width)`
      - Vrací jednotkovou matici požadovaných dimenzí
  - `static RealMatrix Elementary(int t_Length, int t_Nonzero)`
      - Vrací elementární vektor, který má na zadané pozici jednotku
  - `static RealMatrix From(double[,] t_Elements)`
      - Vrací matici sestrojenou z dat dvojrozměrného pole
  - `static RealMatrix[] From(string t_File)`
      - Vrací matici sestrojenou z jednoduché .mat reprezentace (viz apendix A)

- Ukládání matice do souboru .mat (viz apendix A)
  - `static void SaveMatricesTo(RealMatrix t_Matrix, string t_File)`
  - `static void SaveMatricesTo(RealMatrix[] t_Matrices, string t_File)`
      - Ukládá matici/matice do příslušného .mat souboru (viz apendix A)

- Operátory a indexer
  - `delegate double Operation(double t_first, double t_second)`
  - `double this[int t_Row, int t_Column]`
      - Umožňuje 1-indexovaný přístup k prvkům matice
  - `static RealMatrix operator ~(RealMatrix t_Matrix)`
      - Unární perátor generující kopii matice
  - `static RealMatrix operator -(RealMatrix t_Matrix)`
      - Unární perátor generující obrácenou kopii matice
  - `static RealMatrix operator *(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)`
      - Binární operátor maticového násobení
  - `static RealMatrix operator +(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)`
      - Binární operátor sčítání matic
  - `static RealMatrix operator -(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)`
      - Binární operátor odčítání matic
  - `static RealMatrix ElementwiseOperation(Operation t_operation, RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)`
      - Funkce umožňující aplikování definované binární operace pracující po prvcích na dvě matice
  - `static bool operator ==(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)`
  - `static bool operator !=(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)`
      - Operátory rovnosti a nerovnosti matic

- Podmatice a modifikování existujících matic
  - `RealMatrix Column(int t_Column)`
      - Vrací vybraný sloupec matice
  - `RealMatrix Row(int t_Row)`
      - Vrací vybraný řádek matice
  - `RealMatrix SubMatrix(int t_RowBegin, int t_RowEnd, int t_ColumnBegin, int t_ColumnEnd)`
      - Vrací vybranou podmatici
  - `RealMatrix Insert(int t_Row, int t_Column, RealMatrix t_InsertedMatrix)`
      - Vrací výslednou matici vytvořenou vložením jiné matice do původní
  - `RealMatrix Modify(Operation t_operation, double t_value)`
      - Vrací matici modifikovanou aplikací binární operace na její prvky
  - `RealMatrix Round(int Precision)`
      - Vrací matici s jejími prvky zaokrouhlenými na danou přesnost
  - `RealMatrix Transpose()`
      - Vrací transponovanou matici
   
  ## Modul Algorithms
  - Přesnost výpočtů
    - `static int Precision`
      - Nastavuje přesnost výsledku / výpočtů (defaultně 16 desetinných cifer)

  - Rozklady matic
    - `static (RealMatrix, RealMatrix) QR(RealMatrix t_Matrix)`
      - Vrací QR rozklad matice pomocí Householderových reflexí
    - `static (RealMatrix,RealMatrix) GivensQR(RealMatrix t_Matrix)`
      - Vrací QR rozklad matice pomocí Givensových rotací
    - `static (RealMatrix, RealMatrix, RealMatrix) LU(RealMatrix t_Matrix)`
      - Vrací LUP rozklad matice

  - Řešení SLR a inverze matice
    - `static RealMatrix Inverse(RealMatrix t_Matrix)`
        - Vrací inverz regulární matice
    - `static RealMatrix LinSys(RealMatrix t_Matrix, RealMatrix b_Matrix)`
        - Vrací vektor řešení soustavy lineárních rovnic s pravou stranou

  - Determinant a spektrum matice
    - `static double Determinant(RealMatrix t_Matrix)`
        - Vrací determinant čtvercové matice
    - `static double[] Eigenvalues(RealMatrix t_Matrix)`
        - Vrací spektrum matice

  ## Apendix A
    - Program pracuje s vlastním souborovým formátem, nuceně označeným typem souboru .mat
    - Struktura souboru .mat je následující:
        1. Řádek s přirozeným číslem, označující počet matic v souboru, následovaný prázdným řádkem
        2. Jeden či více záznamů o matici, složený z:
           1. Řádku s dvěma hodnotami reprezentujícími výšku a šířku matice
           2. Řádkově oddělenými hodnotami matice, které jsou na řádku odděleny mezerou
    


