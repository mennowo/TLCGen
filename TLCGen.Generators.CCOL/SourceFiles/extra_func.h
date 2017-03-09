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
	count dr1,      /* drukknop buiten deze fase */
	count dr2,      /* drukknop binnen deze fase */
	count dr3);      /* drukknop buiten evt. naloop fase */
bool Rateltikkers_Accross(count fc,       /* fase */
	count has,      /* hulpelement rateltikkers voor deze fase */
	count has_aan_, /* hulpelement tikkers werking */
	count has_cont_,/* hulpelement tikkers continu */
	count dr1,      /* drukknop buiten deze fase */
	count dr2,      /* drukknop binnen deze fase */
	count dr3);      /* drukknop buiten evt. naloop fase */
void NaloopVtgV2(count fc1, count fc2, count dk, count hdk, count tnl);

extern mulv * FC_type;

#endif