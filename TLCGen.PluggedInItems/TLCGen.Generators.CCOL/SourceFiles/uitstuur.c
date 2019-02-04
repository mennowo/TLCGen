#include "uitstuur.h"

#if defined MLMAX || defined PLMAX
void SegmentSturing(count displ,
                    count us1, count us2, count us3, count us4, count us5, count us6, count us7)
{
   CIF_GUS[us1] = (displ >= 0)  && (displ != 1) &&  (displ != 4)  &&  /*   1   */
                  (displ != 11) && (displ != 13);                     /* 2   3 */
   CIF_GUS[us2] = (displ == 0)  || (displ > 3)  &&  (displ != 7)  &&  /*   4   */
                  (displ != 13);                                      /* 5   6 */
   CIF_GUS[us3] = ((displ >= 0) && (displ < 5)  ||  (displ > 6))  &&  /*   7   */
                  (displ < 11) || (displ == 13);
   CIF_GUS[us4] = (displ > 1)   && (displ != 7) &&  (displ != 12);
   CIF_GUS[us5] = (displ >= 0)  && (displ != 1) && ((displ < 3)   ||
                  (displ > 5))  && (displ != 7) &&  (displ != 9);
   CIF_GUS[us6] = (displ >= 0)  && (displ != 2) &&  (displ != 12) &&
                  (displ < 14);
   CIF_GUS[us7] = (displ >= 0)  && (displ != 1) &&  (displ != 4)  &&
                  (displ != 7)  && (displ != 10) && (displ < 15);
}

void SegmentSturingDubbel(count displ,
                          count us1_1, count us2_1, count us3_1, count us4_1, count us5_1, count us6_1, count us7_1,
                          count us1_2, count us2_2, count us3_2, count us4_2, count us5_2, count us6_2, count us7_2)
{
   count displ_l = (displ % 100) / 10;  /* tientallen    */
   count displ_r = displ % 10; /* eenheden   */

   CIF_GUS[us1_1] = (displ_l >= 0)  && (displ_l != 1) &&  (displ_l != 4)  &&  /*   1   */
                    (displ_l != 11) && (displ_l != 13);                       /* 2   3 */
   CIF_GUS[us2_1] = (displ_l == 0)  || (displ_l > 3)  &&  (displ_l != 7)  &&  /*   4   */
                    (displ_l != 13);                                          /* 5   6 */
   CIF_GUS[us3_1] = ((displ_l >= 0) && (displ_l < 5)  ||  (displ_l > 6))  &&  /*   7   */
                    (displ_l < 11) || (displ_l == 13);
   CIF_GUS[us4_1] = (displ_l > 1)   && (displ_l != 7) &&  (displ_l != 12);
   CIF_GUS[us5_1] = (displ_l >= 0)  && (displ_l != 1) && ((displ_l < 3)   ||
                    (displ_l > 5))  && (displ_l != 7) &&  (displ_l != 9);
   CIF_GUS[us6_1] = (displ_l >= 0)  && (displ_l != 2) &&  (displ_l != 12) &&
                    (displ_l < 14);
   CIF_GUS[us7_1] = (displ_l >= 0)  && (displ_l != 1) &&  (displ_l != 4)  &&
                    (displ_l != 7)  && (displ_l != 10) && (displ_l < 15);

   CIF_GUS[us1_2] = (displ_r >= 0)  && (displ_r != 1) &&  (displ_r != 4)  &&
                    (displ_r != 11) && (displ_r != 13);
   CIF_GUS[us2_2] = (displ_r == 0)  || (displ_r > 3)  &&  (displ_r != 7)  &&
                    (displ_r != 13);
   CIF_GUS[us3_2] = ((displ_r >= 0) && (displ_r < 5)  ||  (displ_r > 6))  &&
                    (displ_r < 11) || (displ_r == 13);
   CIF_GUS[us4_2] = (displ_r > 1)   && (displ_r != 7) &&  (displ_r != 12);
   CIF_GUS[us5_2] = (displ_r >= 0)  && (displ_r != 1) && ((displ_r < 3)   ||
                    (displ_r > 5))  && (displ_r != 7) &&  (displ_r != 9);
   CIF_GUS[us6_2] = (displ_r >= 0)  && (displ_r != 2) &&  (displ_r != 12) &&
                    (displ_r < 14);
   CIF_GUS[us7_2] = (displ_r >= 0)  && (displ_r != 1) &&  (displ_r != 4)  &&
                    (displ_r != 7)  && (displ_r != 10) && (displ_r < 15);
}

void SegmentSturingDrie(count displ,
                        count us1_1, count us2_1, count us3_1, count us4_1, count us5_1, count us6_1, count us7_1,
                        count us1_2, count us2_2, count us3_2, count us4_2, count us5_2, count us6_2, count us7_2,
                        count us1_3, count us2_3, count us3_3, count us4_3, count us5_3, count us6_3, count us7_3)
{
   count displ_l = displ / 100; /* honderdtallen */
   count displ_m = (displ % 100) / 10;  /* tientallen    */
   count displ_r = displ % 10;  /* eenheden      */

   CIF_GUS[us1_1] = (displ_l >= 0)  && (displ_l != 1) &&  (displ_l != 4)  &&  /*   1   */
                    (displ_l != 11) && (displ_l != 13);                       /* 2   3 */
   CIF_GUS[us2_1] = (displ_l == 0)  || (displ_l > 3)  &&  (displ_l != 7)  &&  /*   4   */
                    (displ_l != 13);                                          /* 5   6 */
   CIF_GUS[us3_1] = ((displ_l >= 0) && (displ_l < 5)  ||  (displ_l > 6))  &&  /*   7   */
                    (displ_l < 11) || (displ_l == 13);
   CIF_GUS[us4_1] = (displ_l > 1)   && (displ_l != 7) &&  (displ_l != 12);
   CIF_GUS[us5_1] = (displ_l >= 0)  && (displ_l != 1) && ((displ_l < 3)   ||
                    (displ_l > 5))  && (displ_l != 7) &&  (displ_l != 9);
   CIF_GUS[us6_1] = (displ_l >= 0)  && (displ_l != 2) &&  (displ_l != 12) &&
                    (displ_l < 14);
   CIF_GUS[us7_1] = (displ_l >= 0)  && (displ_l != 1) &&  (displ_l != 4)  &&
                    (displ_l != 7)  && (displ_l != 10) && (displ_l < 15);

   CIF_GUS[us1_2] = (displ_m >= 0)  && (displ_m != 1) &&  (displ_m != 4)  &&
                    (displ_m != 11) && (displ_m != 13);
   CIF_GUS[us2_2] = (displ_m == 0)  || (displ_m > 3)  &&  (displ_m != 7)  &&
                    (displ_m != 13);
   CIF_GUS[us3_2] = ((displ_m >= 0) && (displ_m < 5)  ||  (displ_m > 6))  &&
                    (displ_m < 11) || (displ_m == 13);
   CIF_GUS[us4_2] = (displ_m > 1)   && (displ_m != 7) &&  (displ_m != 12);
   CIF_GUS[us5_2] = (displ_m >= 0)  && (displ_m != 1) && ((displ_m < 3)   ||
                    (displ_m > 5))  && (displ_m != 7) &&  (displ_m != 9);
   CIF_GUS[us6_2] = (displ_m >= 0)  && (displ_m != 2) &&  (displ_m != 12) &&
                    (displ_m < 14);
   CIF_GUS[us7_2] = (displ_m >= 0)  && (displ_m != 1) &&  (displ_m != 4)  &&
                    (displ_m != 7)  && (displ_m != 10) && (displ_m < 15);

   CIF_GUS[us1_3] = (displ_r >= 0)  && (displ_r != 1) &&  (displ_r != 4)  &&
                    (displ_r != 11) && (displ_r != 13);
   CIF_GUS[us2_3] = (displ_r == 0)  || (displ_r > 3)  &&  (displ_r != 7)  &&
                    (displ_r != 13);
   CIF_GUS[us3_3] = ((displ_r >= 0) && (displ_r < 5)  ||  (displ_r > 6))  &&
                    (displ_r < 11) || (displ_r == 13);
   CIF_GUS[us4_3] = (displ_r > 1)   && (displ_r != 7) &&  (displ_r != 12);
   CIF_GUS[us5_3] = (displ_r >= 0)  && (displ_r != 1) && ((displ_r < 3)   ||
                    (displ_r > 5))  && (displ_r != 7) &&  (displ_r != 9);
   CIF_GUS[us6_3] = (displ_r >= 0)  && (displ_r != 2) &&  (displ_r != 12) &&
                    (displ_r < 14);
   CIF_GUS[us7_3] = (displ_r >= 0)  && (displ_r != 1) &&  (displ_r != 4)  &&
                    (displ_r != 7)  && (displ_r != 10) && (displ_r < 15);
}

#endif
