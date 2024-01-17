
namespace TimesheetApp.Model.Context
{
    public static class Captions
    {
        #region Workshift
        public const string WorkshiftCascUpdate = "L'orario di lavoro selezionato è in uso in uno o più giorni. In caso di aggiornamento, verranno ricalcolati eventuali ritardi e/o straordinari. Inoltre, questa operazione potrebbe causare anomalie nei dati. Procedere comunque?";
        public const string WorkshiftCascUpdateFail = "Errore nel ricalcolo di uno o più giorni di lavoro; Riprovare.";
        public const string WorkshiftDefaultInfo = "Abilita questa opzione per assegnare automaticamente ad ogni giornata questo orario di lavoro.";
        public const string WorkshiftDelete = "Sei sicuro di voler eliminare l'orario di lavoro selezionato? I dati non saranno in nessun modo recuperabili.";
        public const string WorkshiftDeleteFail = "A causa di un errore sconosciuto non è possibile eliminare l'orario di lavoro selezionato. Riprovare.";
        public const string WorkshiftDescAlredyExist = "È già stato registrato un orario di lavoro con questa descrizione. Modificare la descrizione.";
        public const string WorkshiftDescMissing = "Inserire la descrizione dell'orario di lavoro.";
        public const string WorkshiftEntExitInv = "L'ora di entrata inserita è maggiore dell'ora di uscita. Controllare i dati inseriti e riprovare.";
        public const string WorkshiftEnterInfo = "In caso di flessibilità oraria, inserire l'orario minimo di entrata.";
        public const string WorkshiftExitInfo = "In caso di flessibilità oraria, inserire l'orario minimo di uscita.";
        public const string WorkshiftHrsAlredyExist = "È già stato registrato un orario di lavoro con quest'ora di entrata e quest'ora di uscita. Modificare gli orari inseriti.";
        public const string WorkshiftInUse = "Non è possibile eliminare l'orario di lavoro selezionato poichè è attualmente in uso in uno o più giorni.";
        public const string WorkshiftLunchInv = "L'ora di fine pausa pranzo inserita è maggiore dell'ora di inizio pausa pranzo. Controllare i dati inseriti e riprovare.";
        public const string WorkshiftLunchPauseInfo = "Se questo orario di lavoro prevede una pausa, abilita questa opzione per impostare gli orari di inizio e fine.";
        public const string WorkshiftLunchTooEarly = "L'ora di inizio pausa pranzo inserita è minore dell'ora di entrata. Controllare i dati inseriti e riprovare.";
        public const string WorkshiftLunchTooLate = "L'ora di fine pausa pranzo inserita è maggiore dell'ora di uscita. Controllare i dati inseriti e riprovare.";
        public const string WorkshiftOrderFail = "Impossibile registrare l'orario di lavoro, gli orari inseriti non sono stati specificati nel corretto ordine";
        public const string WorkshiftSetFail = "Impossibile impostare il turno selezionato.";
        public const string WorkshiftSetInfo = "Imposta un nuovo orario di lavoro o modifica quello attualmente assegnato a questa giornata lavorativa.";
        #endregion

        #region Timestamp        
        public const string TimestampAlrReg = "Questa timbratura è già stata registrata; Vuoi sovrascrivere l'orario attualmente presente nel sistema?";
        public const string TimestampDelConfirm = "Confermi di voler eliminare la timbratura selezionata?";
        public const string TimestampFail = "Impossibile aggiungere la timbratura.";
        public const string TimestampFutureFail = "Impossbile aggiungere timbrature future.";
        public const string TimestampLunchEndMissing = "Impossibile inserire la timbratura. Non è stato registrata la fine della pausa pranzo.";
        public const string TimestampLunchStartMissing = "Impossibile inserire la timbratura. Non è stato registrato l'inizio della pausa pranzo.";
        public const string TimestampMorningMissing = "Impossibile inserire la timbratura. Non è stata registrata l'ora di entrata.";
        public const string TimestampOrderFail = "Impossibile aggiungere la timbratura. Rispettare l'ordine cronologico.";
        public const string TimestampOrderFailNext = "Impossibile aggiungere la timbratura. L'orario registrato non può essere maggiore della timbratura successiva.";
        public const string TimestampOrderFailPrev = "Impossibile aggiungere la timbratura. L'orario registrato non può essere minore della timbratura precedente.";
        public const string TimestampUpdateFail = "Impossibile aggiornare la timbratura.";
        public const string TimestampWsMissing = "Per poter timbrare è necessario specificare un orario di lavoro";
        #endregion

        #region Timesheet
        public const string TimesheetDeleteConfirm = "Confermi di voler eliminare la giornata lavorativa e tutte le relative timbrature? I dati non saranno in alcun modo recuperabili";
        public const string TimesheetInfoDelete = "Elimina la giornata lavorativa e le relative timbrature.";
        public const string TimesheetFull = "Impossibile aggiungere ulteriori timbrature, è già stata registrata la fine della giornata.";
        public const string TimesheetAmounsFail = "Impossibile aggiornare ritardi e/o straordinari. Eliminare ed inserire nuovamente la timbratura.";
        public const string TimesheetAmounsOvertimeFail = "Impossibile aggiornare ritardi e/o straordinari. Riavviare l'applicazione.";
        public const string TimesheetOvertimeFail = "Impossibile impostare questa giornata come lavorativa o straordinaria. Riavviare l'applicazione.";
        public const string TimesheetSetAsOvertimeInfo = "Imposta tutte le ore lavorate in questa giornata lavorativa come straordinari.";
        public const string TimesheetRemoveOvertimeInfo = "Imposta questa giornata lavorativa come giornata standard.";
        public const string TimesheetNotesInfo = "Aggiungi delle note alla giornata corrente o modifica quelle già aggiunte.";
        public const string TimesheetNotesFail = "Impossibile aggiornare le note relative alla giornata corrente.";
        public const string TimesheetNotesResetConfirm = "Confermi di voler eliminare le note aggiunte alla giornata corrente?";
        #endregion

        #region Settings
        public const string SettingsResetConfirm = "Confermi di voler eliminare le impostazioni correnti e sostituirle con quelle di default?";
        public const string SettingsUpdateFail = "Impossibile aggiornare le impostazioni correnti. Operazione annullata.";
        public const string SettingsUpdateHourlyBankConfirm = "Aggiornando queste impostazioni, la banca ore verrà ricalcolata, vuoi procedere?";
        public const string SettingsUpdateLateConfirm = "Aggiornando queste impostazioni, tutti i ritardi verranno ricalcolati, vuoi procedere?";
        public const string SettingsUpdateOvertimeConfirm = "Aggiornando queste impostazioni, tutti gli straordinari verranno ricalcolati, vuoi procedere?";
        public const string SettingsUpdateOvertimeDayConfirm = "Aggiornando queste impostazioni, i giorni lavorativi verranno ricalcolati, vuoi procedere?";
        public const string SettingsDBResetInfo = "Selezionando questa opzione, l'applicazione verrà arrestata e ripristinata, eliminando sia i dati che le impostazioni. I dati eliminati non saranno in alcun modo recuperabili.";
        public const string SettingsDBResetConfirm = "Ripristinando l'applicazione, i dati eliminati non saranno in alcun modo recuperabili. Vuoi procedere?";
        public const string SettingsDBResetSecondConfirm = "Sei sicuro di voler procedere all'eliminazione di tutti i dati?";
            
        public const string AfternoonOvertimeInfo = "Abilita per aggiungere straordinari in caso di uscita posticipata.";
        public const string AfternoonHourlyBankInfo = "Abilita per aggiungere minuti in banca ore in caso di uscita posticipata.";
        public const string EarlyPauseLateInfo = "Abilita per aggiungere ritardo in caso di inizio pausa anticipato.";
        public const string MaxHourlyBankConsumption = "Indica la quantità massima di tempo che puoi utilizzare giornalmente dalla tua banca ore (se ne disponi).";
        public const string MinOvertimeAmountInfo = "Indica la quantità minima di tempo che viene conteggiato come straordinario.";
        public const string MinLateAmountInfo = "Indica la quantità minima di tempo che viene conteggiato come ritardo.";
        public const string MorningOvertimeInfo = "Abilita per aggiungere straordinari in caso di entrata anticipata.";
        public const string MorningHourlyBankInfo = "Abilita per aggiungere minuti in banca ore in caso di entrata anticipata.";
        #endregion
    }
}
