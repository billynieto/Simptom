using System;
using System.Collections.Generic;

using Simptom.Framework;
using Simptom.Framework.Models;
using Simptom.Models;

namespace Simptom.Test.Factories
{
	public class ValidModelFactory : IModelFactory
	{
		public IActivitiesSearch GenerateActivitiesSearch()
		{
			ActivitiesSearch activitiesSearch = new ActivitiesSearch();
			
			return activitiesSearch;
		}
		
		public IActivity GenerateActivity(IActivityKey activityKey)
		{
			IActivity activity = new Activity(activityKey);
			activity.Category = null;
			activity.Name = "wgtiBuiJcNSwK";
			
			return activity;
		}
		
		public IActivityCategoriesSearch GenerateActivityCategoriesSearch()
		{
			ActivityCategoriesSearch activityCategoriesSearch = new ActivityCategoriesSearch();
			
			return activityCategoriesSearch;
		}
		
		public IActivityCategory GenerateActivityCategory(IActivityCategoryKey activityCategoryKey)
		{
			IActivityCategory activityCategory = new ActivityCategory(activityCategoryKey);
			activityCategory.Name = "rFKrENYPkWsarRPbaEtOFSty";
			
			return activityCategory;
		}
		
		public IActivityCategoryKey GenerateActivityCategoryKey(Guid id)
		{
			IActivityCategoryKey activityCategoryKey = new ActivityCategoryKey(id);
			
			return activityCategoryKey;
		}
		
		public IActivityCategorySearch GenerateActivityCategorySearch()
		{
			ActivityCategorySearch activityCategorySearch = new ActivityCategorySearch();
			
			return activityCategorySearch;
		}
		
		public IActivityKey GenerateActivityKey(Guid id)
		{
			IActivityKey activityKey = new ActivityKey(id);
			
			return activityKey;
		}
		
		public IActivitySearch GenerateActivitySearch()
		{
			ActivitySearch activitySearch = new ActivitySearch();
			
			return activitySearch;
		}
		
		public IFlareUp GenerateFlareUp(IFlareUpKey flareUpKey)
		{
			IFlareUp flareUp = new FlareUp(flareUpKey);
			flareUp.ExperiencedOn = new DateTime(2020, 5, 22, 5, 48, 50);
			flareUp.Severity = 0.61087077605113d;
			flareUp.Symptom = null;
			flareUp.User = null;
			
			return flareUp;
		}
		
		public IFlareUpKey GenerateFlareUpKey(Guid id)
		{
			IFlareUpKey flareUpKey = new FlareUpKey(id);
			
			return flareUpKey;
		}
		
		public IFlareUpSearch GenerateFlareUpSearch()
		{
			FlareUpSearch flareUpSearch = new FlareUpSearch();
			
			return flareUpSearch;
		}
		
		public IFlareUpsSearch GenerateFlareUpsSearch()
		{
			FlareUpsSearch flareUpsSearch = new FlareUpsSearch();
			
			return flareUpsSearch;
		}
		
		public IParticipation GenerateParticipation(IParticipationKey participationKey)
		{
			IParticipation participation = new Participation(participationKey);
			participation.Activity = null;
			participation.PerformedOn = new DateTime(2020, 5, 22, 5, 48, 50);
			participation.Severity = 0.144081865970083d;
			participation.User = null;
			
			return participation;
		}
		
		public IParticipationKey GenerateParticipationKey(Guid id)
		{
			IParticipationKey participationKey = new ParticipationKey(id);
			
			return participationKey;
		}
		
		public IParticipationSearch GenerateParticipationSearch()
		{
			ParticipationSearch participationSearch = new ParticipationSearch();
			
			return participationSearch;
		}
		
		public IParticipationsSearch GenerateParticipationsSearch()
		{
			ParticipationsSearch participationsSearch = new ParticipationsSearch();
			
			return participationsSearch;
		}
		
		public ISymptom GenerateSymptom(ISymptomKey symptomKey)
		{
			ISymptom symptom = new Symptom(symptomKey);
			symptom.Category = null;
			symptom.Name = "sCeJnDrdeDM";
			
			return symptom;
		}
		
		public ISymptomCategoriesSearch GenerateSymptomCategoriesSearch()
		{
			SymptomCategoriesSearch symptomCategoriesSearch = new SymptomCategoriesSearch();
			
			return symptomCategoriesSearch;
		}
		
		public ISymptomCategory GenerateSymptomCategory(ISymptomCategoryKey symptomCategoryKey)
		{
			ISymptomCategory symptomCategory = new SymptomCategory(symptomCategoryKey);
			symptomCategory.Name = "loGWwPBdrTbModVSzlmLxBFnfmewZOKyoEhzZVWIeeUzBsukH";
			
			return symptomCategory;
		}
		
		public ISymptomCategoryKey GenerateSymptomCategoryKey(Guid id)
		{
			ISymptomCategoryKey symptomCategoryKey = new SymptomCategoryKey(id);
			
			return symptomCategoryKey;
		}
		
		public ISymptomCategorySearch GenerateSymptomCategorySearch()
		{
			SymptomCategorySearch symptomCategorySearch = new SymptomCategorySearch();
			
			return symptomCategorySearch;
		}
		
		public ISymptomKey GenerateSymptomKey(Guid id)
		{
			ISymptomKey symptomKey = new SymptomKey(id);
			
			return symptomKey;
		}
		
		public ISymptomSearch GenerateSymptomSearch()
		{
			SymptomSearch symptomSearch = new SymptomSearch();
			
			return symptomSearch;
		}
		
		public ISymptomsSearch GenerateSymptomsSearch()
		{
			SymptomsSearch symptomsSearch = new SymptomsSearch();
			
			return symptomsSearch;
		}
		
		public IUser GenerateUser(IUserKey userKey)
		{
			IUser user = new User(userKey);
			user.Name = "ETraCKJsQkLfdDiDxJVwTDoD";
			user.Password = "sHgMNSUOBjRgocxQGeMXYbqqXULolFaE";
			
			return user;
		}
		
		public IUserKey GenerateUserKey(Guid id)
		{
			IUserKey userKey = new UserKey(id);
			
			return userKey;
		}
		
		public IUserSearch GenerateUserSearch()
		{
			UserSearch userSearch = new UserSearch();
			
			return userSearch;
		}
		
		public IUsersSearch GenerateUsersSearch()
		{
			UsersSearch usersSearch = new UsersSearch();
			
			return usersSearch;
		}
	}
}
