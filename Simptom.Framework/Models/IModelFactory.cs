using System;

namespace Simptom.Framework.Models
{
	public interface IModelFactory
	{
		IActivitiesSearch GenerateActivitiesSearch();
		IActivity GenerateActivity(IActivityKey activityKey);
		IActivityCategoriesSearch GenerateActivityCategoriesSearch();
		IActivityCategory GenerateActivityCategory(IActivityCategoryKey activityCategoryKey);
		IActivityCategoryKey GenerateActivityCategoryKey(Guid id);
		IActivityCategorySearch GenerateActivityCategorySearch();
		IActivityKey GenerateActivityKey(Guid id);
		IActivitySearch GenerateActivitySearch();
		IFlareUp GenerateFlareUp(IFlareUpKey flareUpKey);
		IFlareUpKey GenerateFlareUpKey(Guid id);
		IFlareUpSearch GenerateFlareUpSearch();
		IFlareUpsSearch GenerateFlareUpsSearch();
		IParticipation GenerateParticipation(IParticipationKey participationKey);
		IParticipationKey GenerateParticipationKey(Guid id);
		IParticipationSearch GenerateParticipationSearch();
		IParticipationsSearch GenerateParticipationsSearch();
		ISymptom GenerateSymptom(ISymptomKey symptomKey);
		ISymptomCategoriesSearch GenerateSymptomCategoriesSearch();
		ISymptomCategory GenerateSymptomCategory(ISymptomCategoryKey symptomCategoryKey);
		ISymptomCategoryKey GenerateSymptomCategoryKey(Guid id);
		ISymptomCategorySearch GenerateSymptomCategorySearch();
		ISymptomKey GenerateSymptomKey(Guid id);
		ISymptomSearch GenerateSymptomSearch();
		ISymptomsSearch GenerateSymptomsSearch();
		IUser GenerateUser(IUserKey userKey);
		IUserKey GenerateUserKey(Guid id);
		IUserSearch GenerateUserSearch();
		IUsersSearch GenerateUsersSearch();
	}
}
