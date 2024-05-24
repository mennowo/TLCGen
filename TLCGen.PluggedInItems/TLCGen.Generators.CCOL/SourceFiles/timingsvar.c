/* Definitie EventState */
/* ==================== */
extern s_int16 CCOL_FC_EVENTSTATE[FCMAX][3];

s_int16 set_fctiming(mulv i, mulv eventnr, s_int16 mask, s_int16 eventState, s_int16 startTime, s_int16 minEndTime, s_int16 maxEndTime, s_int16 likelyTime, s_int16 confidence, s_int16 nextTime);
s_int16 reset_fctiming(mulv i, mulv eventnr);
void msg_fctiming(mulv latency_minEndSG);