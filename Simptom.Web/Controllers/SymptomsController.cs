using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

using Simptom.Framework.Models;
using Simptom.Framework.Repositories;
using Simptom.Framework.Services;
using Simptom.Models;
using Simptom.Server.Repositories;
using Simptom.Services;
using Simptom.Web.ViewModels;

namespace Simptom.Web.Controllers
{
    public class SymptomsController : Controller
    {
        private IModelFactory modelFactory;
        private ISymptomCategoryService symptomCategoryService;
        private ISymptomService symptomService;

        public SymptomsController()
        {
            this.modelFactory = new ModelFactory();

            IDbConnection connection = new SqlConnection("Data Source='BADASS-TABLET-P\\SQLEXPRESS'; Initial Catalog = 'Simptom'; User ID = 'SystemUser'; Password = 'insecure'");

            ISymptomCategoryRepository symptomCategoryRepository = new SymptomCategoryRepository(connection, modelFactory);
            this.symptomCategoryService = new SymptomCategoryService(symptomCategoryRepository, modelFactory);

            ISymptomRepository symptomRepository = new SymptomRepository(connection, modelFactory);
            this.symptomService = new SymptomService(symptomRepository, this.symptomCategoryService, modelFactory);
        }

        public ActionResult Index()
        {
            ISymptomCategoriesSearch categoriesSearch = this.modelFactory.GenerateSymptomCategoriesSearch();
            ISymptomsSearch symptomsSearch = this.modelFactory.GenerateSymptomsSearch();

            SymptomsViewModel viewModel = new SymptomsViewModel();
            viewModel.Categories = this.symptomCategoryService.Find(categoriesSearch);
            viewModel.Symptoms = this.symptomService.Find(symptomsSearch);

            return View(viewModel);
        }

        public ActionResult Create()
        {
            ISymptomCategoriesSearch categoriesSearch = this.modelFactory.GenerateSymptomCategoriesSearch();

            SymptomViewModel viewModel = new SymptomViewModel();
            viewModel.Categories = this.symptomCategoryService.Find(categoriesSearch);
            viewModel.Symptom = null;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(Symptom symptom)
        {
            ISymptomCategorySearch categorySearch = this.modelFactory.GenerateSymptomCategorySearch();
            categorySearch.ID = new Guid(Request.Form["Symptom.Category"]);

            symptom.Category = this.symptomCategoryService.FindSingle(categorySearch);

            this.symptomService.Save(symptom, null);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid id)
        {
            ISymptomCategoriesSearch categoriesSearch = this.modelFactory.GenerateSymptomCategoriesSearch();

            ISymptomSearch symptomSearch = this.modelFactory.GenerateSymptomSearch();
            symptomSearch.ID = id;
            ISymptom symptom = this.symptomService.FindSingle(symptomSearch);

            if (symptom == null)
                return HttpNotFound();

            SymptomViewModel viewModel = new SymptomViewModel();
            viewModel.Categories = this.symptomCategoryService.Find(categoriesSearch);
            viewModel.Symptom = symptom;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Symptom symptom)
        {
            ISymptomCategorySearch categorySearch = this.modelFactory.GenerateSymptomCategorySearch();
            categorySearch.ID = new Guid(Request.Form["Symptom.Category"]);

            symptom.Category = this.symptomCategoryService.FindSingle(categorySearch);

            this.symptomService.Save(symptom, null);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            string errorMessage = null;

            ISymptomKey key = this.modelFactory.GenerateSymptomKey(id);

            try
            {
                this.symptomService.Delete(key, null);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}