
// Error: Activity Name is a Required string and MUST have a Minimum value set.
// Error: ActivityCategory Name is a Required string and MUST have a Minimum value set.
// Error: Symptom Name is a Required string and MUST have a Minimum value set.
// Error: SymptomCategory Name is a Required string and MUST have a Minimum value set.
// Error: User Name is a Required string and MUST have a Minimum value set.
// Error: User Password is a Required string and MUST have a Minimum value set.
// No Warnings
// No Comments

#region Activity

////Associates: Participation
//Enemies: Participation

////Children: None

////Parents: ActivityCategory
//Unfit Parents: ActivityCategory

////Significant Others: None

//Affected Models: None
//Dependants: None
//Guardians: None

#endregion Activity

#region ActivityCategory

////Associates: None

////Children: Activity
//Custodied Children: Activity

////Parents: None

////Significant Others: None

//Affected Models: None
//Dependants: None
//Guardians: None

#endregion ActivityCategory

#region FlareUp

////Associates: Symptom, User
//Admires: Symptom, User

////Children: None

////Parents: None

////Significant Others: None

//Affected Models: Symptom, SymptomCategory, User
//Dependants: None
//Guardians: None

#endregion FlareUp

#region Participation

////Associates: Activity, User
//Admires: Activity, User

////Children: None

////Parents: None

////Significant Others: None

//Affected Models: Activity, User
//Dependants: None
//Guardians: None

#endregion Participation

#region Symptom

////Associates: FlareUp, SymptomCategory
//Admires: SymptomCategory
//Enemies: FlareUp

////Children: None

////Parents: None

////Significant Others: None

//Affected Models: SymptomCategory
//Dependants: None
//Guardians: None

#endregion Symptom

#region SymptomCategory

////Associates: Symptom
//Enemies: Symptom

////Children: None

////Parents: None

////Significant Others: None

//Affected Models: None
//Dependants: None
//Guardians: None

#endregion SymptomCategory

#region User

////Associates: FlareUp, Participation
//Enemies: FlareUp, Participation

////Children: None

////Parents: None

////Significant Others: None

//Affected Models: None
//Dependants: None
//Guardians: None

#endregion User

