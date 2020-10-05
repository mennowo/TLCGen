/* EXTRA_FUNC_RIS.H */
/* ================ */

/* CCOL :  versie 11.0     */
/* FILE :  extra_func.h    */
/* DATUM:  01-10-2020      */

#ifndef __EXTRA_FUNC_RIS_H
#define __EXTRA_FUNC_RIS_H


/* declaratie functies */
/* =================== */
   rif_bool ris_inmelding_selectief(count fc, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_int role, rif_int subrole, rif_int priotypefc_id); 
   rif_bool ris_inmelding_selectief_approach(rif_int approach, rif_string intersection, rif_int lane_id, rif_int stationtype_bits, rif_float length_start, rif_float length_end, rif_int role, rif_int subrole, rif_int priotypefc_id);

   rif_bool ris_uitmelding_selectief(rif_int priotypefc_id);

   void ris_verstuur_ssm(rif_int priotypefc_id);

#endif /* __EXTRA_FUNC_RIS_H  */
