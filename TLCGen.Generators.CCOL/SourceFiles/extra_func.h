#ifndef EXTRA_FUNC
#define EXTRA_FUNC

bool ym_maxV1(count i, mulv to_verschil);
bool ym_max_vtgV1(count i);
void AanvraagSnelV2(count fc1, count dp);
bool Rateltikkers(count fc,       /* fase */
	count has,      /* hulpelement rateltikkers voor deze fase */
	count has_aan_, /* hulpelement tikkers werking */
	count has_cont_,/* hulpelement tikkers continu */
	count tnlrt,    /* tijd na EG dat de tikkers nog moeten worden aangestuurd indien niet continu */
	...);           /* drukknoppen */
bool Rateltikkers_Accross(count fc,       /* fase */
	count has,      /* hulpelement rateltikkers voor deze fase */
	count has_aan_, /* hulpelement tikkers werking */
	count has_cont_,/* hulpelement tikkers continu */
	...);           /* drukknoppen */
void NaloopVtgV2(count fc1, count fc2, count dk, count hdk, count tnl);

extern mulv * FC_type;

#endif