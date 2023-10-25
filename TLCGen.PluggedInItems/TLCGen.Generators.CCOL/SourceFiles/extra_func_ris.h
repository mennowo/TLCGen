/* EXTRA_FUNC_RIS.H */
/* ================ */

/* FILE :  extra_func_ris.h   */
/* DATUM:  09-03-2023         */

#ifndef __EXTRA_FUNC_RIS_H
#define __EXTRA_FUNC_RIS_H

/* include files */
/* ============= */
   #include "sysdef.c"  /* definitie typen variabelen	    */
   #include "rif.inc"   /* declaratie RIS Interface         */

/* Dedicated variabelen */
/* ==================== */
   mulv granted_verstrekt[FCMAX];  /* granted via SSM verstrekt, richting mag niet naar rood gestuurd worden       */

/* declaratie functies */
/* =================== */
   rif_bool ris_inmelding_selectief(count fc, rif_int approach_id, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_int role_bits, rif_int subrole_bits, rif_int importance_bits, rif_int eta_prm, rif_int priotypefc_id);

   rif_int ris_uitmelding_selectief(rif_int priotypefc_id);

   rif_int ris_srm_put_signalgroup(count fc, rif_int approach_id, rif_int role_bits, rif_int subrole_bits, count prm_line_first, count prm_line_max);

#if defined prioFCMAX && (prioFCMAX > 0) /* alleen indien PRIO */
   rif_int ris_verstuur_ssm(rif_int priotypefc_id, rif_int risgrenspriotype);
   void Bepaal_Granted_Verstrekt(void);
#endif
    
   rif_bool ris_check_heading(rif_float itsstation_heading, mulv heading, mulv heading_marge);

   rif_bool ris_detectie_heading(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup, mulv heading, mulv heading_marge);

   rif_bool ris_aanvraag_heading(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup, mulv heading, mulv heading_marge);

   rif_bool ris_verlengen_heading(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup, mulv heading, mulv heading_marge);
 
   mulv ris_itsstations_heading(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_bool match_signalgroup, mulv heading, mulv heading_marge);

#endif /* __EXTRA_FUNC_RIS_H  */

