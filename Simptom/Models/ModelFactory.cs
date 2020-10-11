using System;
using System.Collections.Generic;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Models
{
	public class ModelFactory : IModelFactory
	{
		public IActivitiesSearch GenerateActivitiesSearch()
		{
			return new ActivitiesSearch();
		}
		
		public IActivity GenerateActivity(IActivityKey activityKey)
		{
			return new Activity(activityKey);
		}
		
		public IActivityCategoriesSearch GenerateActivityCategoriesSearch()
		{
			return new ActivityCategoriesSearch();
		}
		
		public IActivityCategory GenerateActivityCategory(IActivityCategoryKey activityCategoryKey)
		{
			return new ActivityCategory(activityCategoryKey);
		}
		
		public IActivityCategoryKey GenerateActivityCategoryKey(Guid id)
		{
			return new ActivityCategoryKey(id);
		}
		
		public IActivityCategorySearch GenerateActivityCategorySearch()
		{
			return new ActivityCategorySearch();
		}
		
		public IActivityKey GenerateActivityKey(Guid id)
		{
			return new ActivityKey(id);
		}
		
		public IActivitySearch GenerateActivitySearch()
		{
			return new ActivitySearch();
		}
		
		public IFlareUp GenerateFlareUp(IFlareUpKey flareUpKey)
		{
			return new FlareUp(flareUpKey);
		}
		
		public IFlareUpKey GenerateFlareUpKey(Guid id)
		{
			return new FlareUpKey(id);
		}
		
		public IFlareUpSearch GenerateFlareUpSearch()
		{
			return new FlareUpSearch();
		}
		
		public IFlareUpsSearch GenerateFlareUpsSearch()
		{
			return new FlareUpsSearch();
		}
		
		public IParticipation GenerateParticipation(IParticipationKey participationKey)
		{
			return new Participation(participationKey);
		}
		
		public IParticipationKey GenerateParticipationKey(Guid id)
		{
			return new ParticipationKey(id);
		}
		
		public IParticipationSearch GenerateParticipationSearch()
		{
			return new ParticipationSearch();
		}
		
		public IParticipationsSearch GenerateParticipationsSearch()
		{
			return new ParticipationsSearch();
		}
		
		public ISymptom GenerateSymptom(ISymptomKey symptomKey)
		{
			return new Symptom(symptomKey);
		}
		
		public ISymptomCategoriesSearch GenerateSymptomCategoriesSearch()
		{
			return new SymptomCategoriesSearch();
		}
		
		public ISymptomCategory GenerateSymptomCategory(ISymptomCategoryKey symptomCategoryKey)
		{
			return new SymptomCategory(symptomCategoryKey);
		}
		
		public ISymptomCategoryKey GenerateSymptomCategoryKey(Guid id)
		{
			return new SymptomCategoryKey(id);
		}
		
		public ISymptomCategorySearch GenerateSymptomCategorySearch()
		{
			return new SymptomCategorySearch();
		}
		
		public ISymptomKey GenerateSymptomKey(Guid id)
		{
			return new SymptomKey(id);
		}
		
		public ISymptomSearch GenerateSymptomSearch()
		{
			return new SymptomSearch();
		}
		
		public ISymptomsSearch GenerateSymptomsSearch()
		{
			return new SymptomsSearch();
		}
		
		public IUser GenerateUser(IUserKey userKey)
		{
			return new User(userKey);
		}
		
		public IUserKey GenerateUserKey(Guid id)
		{
			return new UserKey(id);
		}
		
		public IUserSearch GenerateUserSearch()
		{
			return new UserSearch();
		}
		
		public IUsersSearch GenerateUsersSearch()
		{
			return new UsersSearch();
		}
	}
}
