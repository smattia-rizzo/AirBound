using System;

public class Tris
{
    // Array 1D che rappresenta il tabellone 3x3 (inizializzato automaticamente a 0)
    private int[] tavoliere;

    public Tris()
    {
        tavoliere = new int[9];
    }

    // Proprietà per accedere al tabellone in sola lettura dall'esterno (es. per la UI di Unity)
    public int[] Tavoliere => tavoliere;

    public int[] VisualizzaTavoliere(int numGiocatore)
    {
        if (numGiocatore != 1 && numGiocatore != 2)
        {
            throw new ArgumentException("Valore giocatore non valido (deve essere 1 o 2)");
        }

        // Cloniamo l'array originale per non modificarlo
        int[] tavolo = (int[])tavoliere.Clone();

        for (int i = 0; i < tavolo.Length; i++)
        {
            if (numGiocatore == 1)
            {
                if (tavolo[i] == 2) tavolo[i] = -1;
            }
            else // numGiocatore == 2
            {
                if (tavolo[i] == 1) tavolo[i] = -1;
                else if (tavolo[i] == 2) tavolo[i] = 1;
            }
        }
        return tavolo;
    }

    public bool? CheckVittoria(int numGiocatore)
    {
        if (numGiocatore != 1 && numGiocatore != 2)
        {
            throw new ArgumentException("Valore giocatore non valido (deve essere 1 o 2)");
        }

        // Invece di usare NumPy, definiamo le 8 combinazioni vincenti del Tris
        int[][] combinazioniVincenti = new int[][]
        {
            new int[] {0, 1, 2}, new int[] {3, 4, 5}, new int[] {6, 7, 8}, // Righe
            new int[] {0, 3, 6}, new int[] {1, 4, 7}, new int[] {2, 5, 8}, // Colonne
            new int[] {0, 4, 8}, new int[] {2, 4, 6}                       // Diagonali
        };

        // Controllo se il giocatore ha completato una combinazione
        foreach (var combo in combinazioniVincenti)
        {
            if (tavoliere[combo[0]] == numGiocatore &&
                tavoliere[combo[1]] == numGiocatore &&
                tavoliere[combo[2]] == numGiocatore)
            {
                return true; // Vittoria
            }
        }

        // Controllo se il tavolo è pieno (nessuna casella a 0)
        bool pieno = true;
        foreach (int casella in tavoliere)
        {
            if (casella == 0)
            {
                pieno = false;
                break;
            }
        }

        if (pieno)
        {
            return null; // Patta (corrisponde al None in Python)
        }

        return false; // Il gioco continua
    }

    public bool Play(int numGiocatore, int mossa)
    {
        if (numGiocatore != 1 && numGiocatore != 2)
        {
            throw new ArgumentException("Valore giocatore non valido");
        }
        if (mossa < 0 || mossa > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(mossa), "Mossa non valida (deve essere tra 0 e 8)");
        }

        if (tavoliere[mossa] == 0)
        {
            tavoliere[mossa] = numGiocatore;
            return true; // Mossa eseguita con successo
        }
        else
        {
            return false; // Mossa illegittima (casella occupata)
        }
    }
}