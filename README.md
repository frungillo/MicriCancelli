# MicriCancelli

Gestione del varco parcheggio della Scuola Calcio MICRI: stampa biglietti con codice QR su
termica 80 mm, legge il codice con uno scanner al varco e comanda l'apertura del cancello
tramite una scheda Arduino in rete.

Applicazione Windows Forms su .NET Framework 4.8 (progetto in formato SDK, si compila con
`dotnet build` o con Visual Studio 2022). Nessuna dipendenza da Crystal Reports.

## Struttura

| Cartella / file | Contenuto |
|---|---|
| `Data/` | Accesso a SQLite: `Database` (schema e migrazioni), `CodiciRepository`, `ParametriRepository` |
| `Models/` | `Codice` (biglietto) e `Parametro` |
| `Services/Cancello.cs` | Invio comandi `apri*` / `chiudi*` all'Arduino con timeout e log. Unico punto di apertura |
| `Services/LettoreCodici.cs` | Hook tastiera per lo scanner: accumula i tasti e passa il codice a un thread di lavoro |
| `Services/GestoreAccessi.cs` | Regole di validità dei biglietti e generazione di nuovi codici |
| `Services/StampaBiglietto.cs` | Layout e stampa del biglietto (GDI+, QRCoder). `Code39.cs` per i lettori laser 1D |
| `Services/Impostazioni.cs` | Parametri tipizzati e definizioni per il pannello (etichetta, default, validazione) |
| `Services/Log.cs` | Log su file in `logs\micricancelli-AAAAMMGG.log` più notifica alla finestra |
| `App.cs` | Composizione dei servizi, disponibili ai form tramite `App.*` |
| `DB/cancelli.db` | Database SQLite copiato accanto all'eseguibile |

## Database

Schema versione 2 (`PRAGMA user_version`). Le date sono testo ISO `yyyy-MM-dd HH:mm:ss`.

```sql
codici    (id_codice, codice UNIQUE, emesso_il, usato_il NULL, tipo)
parametri (id_parametro, key UNIQUE, value)
```

Al primo avvio su un DB vecchio la migrazione converte i codici degli ultimi 30 giorni ed elimina la
tabella originale: dopo anni di esercizio conteneva centinaia di migliaia di biglietti scaduti da tempo
(prova con 200.000 righe: 1,5 secondi, file da 11 MB a 0,8 MB).

**Pulizia periodica.** I codici sono a 6 cifre (900.000 numeri): senza pulizia, a qualche centinaio di
emissioni al giorno, la tabella crescerebbe fino a rendere frequenti le collisioni e poi a esaurire i numeri.
Il parametro `giorniConservazione` (default 30) fa cancellare, all'avvio e poi ogni ora, i codici emessi
oltre quella soglia: la tabella resta a poche migliaia di righe e i numeri tornano riutilizzabili.

## Distribuzione e aggiornamento

Le release stanno su GitHub: <https://github.com/frungillo/MicriCancelli/releases>. Ogni release ha
allegato `MicriCancelli-vX.Y.Z.zip` con il programma, un DB vuoto per le installazioni nuove e lo
script `aggiorna.ps1`.

**Creare una release** (macchina di sviluppo, GitHub CLI autenticata):

```bash
powershell -ExecutionPolicy Bypass -File .\deploy
elease.ps1 -Versione 2.0.1 -Note "Cosa cambia"
```

Compila in Release con quel numero di versione (che compare nel titolo della finestra), crea lo zip in
`pubblicazione\` e pubblica tag e release su GitHub.

**Token di accesso.** Il repository è privato, quindi il PC del varco ha bisogno di un token GitHub di
sola lettura. Si crea una volta da GitHub → Settings → Developer settings → Fine-grained tokens:
repository `frungillo/MicriCancelli` soltanto, permesso **Contents: Read-only**, scadenza lunga (es. 1 anno).
Lo script lo salva in `C:\MicriCancelli\github-token.txt` e da lì lo riusa per gli aggiornamenti.
Se il repository dovesse diventare pubblico, il token non serve più e la procedura resta la stessa.

**Installare sul PC del varco la prima volta** (PowerShell, non serve essere amministratore):

```powershell
$env:MICRI_GITHUB_TOKEN = 'github_pat_...'
irm -Headers @{Authorization="Bearer $env:MICRI_GITHUB_TOKEN"} https://raw.githubusercontent.com/frungillo/MicriCancelli/main/deploy/aggiorna.ps1 | iex
```

Installa in `C:\MicriCancelli`, crea il collegamento sul desktop e avvia il programma. Poi, dal
pannello Parametri: stampante, IP dell'Arduino e chiave della piattaforma.

**Aggiornare**: doppio clic su `aggiorna.cmd` nella cartella del programma. Lo script confronta la
versione installata con l'ultima release, scarica lo zip, chiude il programma, sovrascrive i file
**senza toccare `DB\` e `logs\`** e lo riavvia. Con `-Versione 2.0.0` installa una versione precisa
(anche per tornare indietro). In alternativa, da un PC con accesso a GitHub, si scarica lo zip dalla
pagina della release e lo si porta sul PC del varco (anche in sessione remota): estrarlo sopra la
cartella del programma lasciando `DB\` e `logs\`.

## Biglietto e scanner

Il QR contiene le cifre del codice seguite dalla lettera di terminazione (es. `865081E`).
Lo scanner in emulazione tastiera "digita" quella sequenza: la lettera finale (o Invio)
chiude la lettura. I tasti devono arrivare entro 400 ms l'uno dall'altro, quindi la
digitazione umana non viene interpretata come lettura.

- Serve uno **scanner 2D** per il QR. Con un lettore laser 1D impostare `tipoCodice = CODE39`
  dal pannello Parametri: il biglietto stampa un Code 39 con lo stesso contenuto.
- Nel software dello scanner lasciare il suffisso Invio (o nessun suffisso): entrambi funzionano.

Regole di validità: un biglietto vale per `minutiValidita` minuti dall'emissione ed è a uso
singolo; una seconda lettura entro `secondiGrazia` secondi dal primo passaggio riapre ancora
(per le riletture accidentali).

## Stampante termica 80 mm

- La stampa usa la stampante scelta nel parametro `stampante` (vuoto = predefinita di Windows).
- L'area utile è 72 mm; l'altezza pagina è 150 mm.
- Il **taglio carta** lo esegue il driver: nelle preferenze della stampante impostare
  "Taglio a fine documento" (o equivalente, es. "Cut at document end" nei driver Epson/ESC-POS).
- Per vedere il biglietto senza stampare:

```bash
MicriCancelli.exe --anteprima biglietto.png QR
```

## Pannello Parametri

Protetto da password; nel DB è salvato solo l'hash SHA-256 (`parametri.passwordHash`).
La password iniziale viene seminata da `App.config` (`passwordParametriSha256`) al primo avvio
e si cambia dal pannello stesso ("Nuova password pannello").

Ogni parametro ha il proprio pulsante di salvataggio e viene applicato subito, senza riavvio.

## Comandi al cancello

`Cancello.ApriChiudiAsync(motivo)` invia `apri*`, attende `inching` ms, invia `chiudi*`.
Connessione TCP con timeout `timeoutArduino` ms; gli errori finiscono nel log, mai in un crash.
Se una sequenza è già in corso la nuova richiesta viene ignorata e loggata.

Da qui passano anche i comandi remoti della piattaforma IA.

## Apertura remota dalla piattaforma MICRI AI

`Services/ComandiRemoti.cs` tiene una connessione **in uscita** verso la piattaforma (long polling su
`GET /api/varco/comandi`, autenticato con l'header `X-Varco-Chiave`): sul PC del varco non serve aprire
porte né avere un IP pubblico. Quando un Dirigente o un Admin chiede alla chat "apri la sbarra" (o usa il
pulsante in Impostazioni > Varco parcheggio), la piattaforma accoda il comando, il PC lo riceve entro un
secondo, esegue la sequenza apri/chiudi e riporta l'esito, che la chat mostra all'utente.

Configurazione, dal pannello Parametri:

| Parametro | Valore |
|---|---|
| Indirizzo piattaforma IA | `https://apia.micricalcio.it` |
| Chiave piattaforma | copiata da Impostazioni > Varco parcheggio > "Mostra chiave" (solo Admin) |

Chiave vuota = apertura remota disattivata. I comandi non prelevati entro 45 secondi scadono, quindi
una richiesta fatta mentre il PC era spento non apre la sbarra al riavvio. Ogni comando resta nel log
del PC e nella tabella `ComandiVarco` della piattaforma (chi, quando, esito).
