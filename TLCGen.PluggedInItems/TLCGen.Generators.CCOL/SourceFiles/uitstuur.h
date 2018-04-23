#ifndef __UITSTUUR_H__
#define __UITSTUUR_H__

  #if defined MLMAX || defined PLMAX
  void SegmentSturing(count displ,
                      count us1, count us2, count us3, count us4, count us5, count us6, count us7);
  void SegmentSturingDubbel(count displ,
                            count us1_1, count us2_1, count us3_1, count us4_1, count us5_1, count us6_1, count us7_1,
                            count us1_2, count us2_2, count us3_2, count us4_2, count us5_2, count us6_2, count us7_2);
  void SegmentSturingDrie(count displ,
                          count us1_1, count us2_1, count us3_1, count us4_1, count us5_1, count us6_1, count us7_1,
                          count us1_2, count us2_2, count us3_2, count us4_2, count us5_2, count us6_2, count us7_2,
                          count us1_3, count us2_3, count us3_3, count us4_3, count us5_3, count us6_3, count us7_3);
  #endif
#endif
