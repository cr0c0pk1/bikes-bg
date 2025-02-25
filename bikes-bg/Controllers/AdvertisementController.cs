﻿using System.Collections.Generic;
using System.Linq;
using bikes_bg.Models;
using bikes_bg.Repository.Base;
using bikes_bg.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using bikes_bg.Utils;
using Microsoft.EntityFrameworkCore;

namespace bikes_bg.Controllers
{
    public class AdvertisementController : Controller
    {
        private IGenericRepository<BikeModel> bikeModelRepo;
        private IGenericRepository<BikeBrand> bikeBrandRepo;
        private IGenericRepository<BikeCategory> bikeCategoryRepo;
        private IGenericRepository<BikeEngineType> bikeEngineTypeRepo;
        private IGenericRepository<BikeColor> bikeColorRepo;
        private IGenericRepository<Region> regionRepo;
        private IGenericRepository<City> cityRepo;
        private IGenericRepository<Advertisement> advertisementRepo;

        public IHostingEnvironment hostingEnvironment { get; }

        public AdvertisementController(IGenericRepository<BikeModel> bikeModelRepo
            , IGenericRepository<BikeBrand> bikeBrandRepo
            , IGenericRepository<BikeCategory> bikeCategoryRepo
            , IGenericRepository<BikeEngineType> bikeEngineTypeRepo
            , IGenericRepository<BikeColor> bikeColorRepo
            , IGenericRepository<Region> regionRepo
            , IGenericRepository<City> cityRepo
            , IGenericRepository<Advertisement> advertisementRepo
            , IHostingEnvironment hostingEnvironment)
        {
            this.bikeModelRepo = bikeModelRepo;
            this.bikeBrandRepo = bikeBrandRepo;
            this.bikeCategoryRepo = bikeCategoryRepo;
            this.bikeEngineTypeRepo = bikeEngineTypeRepo;
            this.bikeColorRepo = bikeColorRepo;
            this.regionRepo = regionRepo;
            this.cityRepo = cityRepo;
            this.advertisementRepo = advertisementRepo;
            this.hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            CreateAdViewModel model = new CreateAdViewModel();
            model.bikeBrands = bikeBrandRepo.GetAll().ToList();
            model.bikeCategories = bikeCategoryRepo.GetAll().ToList();
            model.bikeEngineTypes = bikeEngineTypeRepo.GetAll().ToList();
            model.regions = regionRepo.GetAll().ToList();
            model.bikeColors = bikeColorRepo.GetAll().ToList();

            return View("CreateAd", model);
        }

        [HttpPost]
        public ActionResult Create(CreateAdViewModel model)
        {
            if (!ModelState.IsValid)
                return View("CreateAd");

            string uniqueFileName;

            if (model.photo != null)
            {
                uniqueFileName = FileUpload.ProcessUploadedFile(model.photo, hostingEnvironment, "images");
            }
            else
            {
                uniqueFileName = "default-ad-image.png";
            }

            Advertisement advertisement = new Advertisement
            {
                modelId = model.selectedBikeModel,
                horsePower = model.bikeHorsePower,
                engineSize = model.bikeEngineSize,
                engineTypeId = model.selectedBikeEngineType,
                productionYear = model.bikeYear,
                mileage = model.bikeMileage,
                price = model.bikePrice,
                cityId = model.selectedCity,
                colorId = model.selectedBikeColor,
                categoryId = model.selectedBikeCategory,
                description = model.description,
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                photoPath = uniqueFileName
            };

            advertisementRepo.Insert(advertisement);

            return RedirectToAction("view", new { id = advertisement.id });
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult View(int id)
        {
            // This loading can be improved using eager loading
            var model = advertisementRepo.GetById(id);
            model.bikeModel = bikeModelRepo.GetById(model.modelId);
            model.bikeModel.bikeBrand = bikeBrandRepo.GetById(model.bikeModel.brandID);
            model.bikeCategory = bikeCategoryRepo.GetById(model.categoryId);
            model.city = cityRepo.GetById(model.cityId);
            model.bikeColor = bikeColorRepo.GetById(model.colorId);


            if (model == null)
            {
                Response.StatusCode = 404;
                return RedirectToAction("index", "home");
            }

            return View("ViewAd", model);
        }

        [HttpGet]
        public IActionResult Search()
        {
            SearchAdViewModel searchAdViewModel = new SearchAdViewModel();

            LoadFilterData(searchAdViewModel.searchFilter);
            searchAdViewModel.advertisements = advertisementRepo.GetTable()
                .Include(ad => ad.bikeModel)
                .Include(ad => ad.bikeModel.bikeBrand)
                .ToList();

            return View(searchAdViewModel);
        }

        [HttpPost]
        public IActionResult Search(SearchAdViewModel model)
        {
            AdFilterService adFilter = new AdFilterService(advertisementRepo);
            model.advertisements = adFilter.GetAdsByFilter(model.searchFilter);
            LoadFilterData(model.searchFilter);
            return View(model);
        }

        public bool LoadFilterData(AdSearchFilter searchFilter)
        {
            searchFilter.bikeBrands = bikeBrandRepo.GetAll().ToList();
            searchFilter.regions = regionRepo.GetAll().ToList();

            return true;
        }

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetModels(int id)
        {
            List<BikeModel> bikeModels = bikeModelRepo.GetAll().Where(model => (model.brandID == id)).ToList();
            IEnumerable<SelectListItem> selectList = bikeModels.Select(model => new SelectListItem
            {
                Value = model.id.ToString(),
                Text = model.name
            }).ToList();

            return Json(selectList);
        }

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetCities(int id)
        {
            List<City> cities = cityRepo.GetAll().Where(model => (model.regionID == id)).ToList();
            IEnumerable<SelectListItem> selectList = cities.Select(model => new SelectListItem
            {
                Value = model.id.ToString(),
                Text = model.name
            }).ToList();

            return Json(selectList);
        }

        [HttpGet]
        public ActionResult ViewEditAd(int id)
        {
            var modelCreateAdViewModel = new CreateAdViewModel();

            var modelAdvertisement = advertisementRepo.GetById(id);

            if (!(User.FindFirstValue(ClaimTypes.NameIdentifier).Equals(modelAdvertisement.userId)))
            {
                Response.StatusCode = 403;
                return View("~/Views/ErrorPages/Error403.cshtml");
            }

            modelAdvertisement.bikeModel = bikeModelRepo.GetById(modelAdvertisement.modelId);
            modelAdvertisement.bikeModel.bikeBrand = bikeBrandRepo.GetById(modelAdvertisement.bikeModel.brandID);
            modelAdvertisement.bikeCategory = bikeCategoryRepo.GetById(modelAdvertisement.categoryId);
            modelAdvertisement.city = cityRepo.GetById(modelAdvertisement.cityId);
            modelAdvertisement.bikeColor = bikeColorRepo.GetById(modelAdvertisement.colorId);

            modelCreateAdViewModel.bikeBrands = bikeBrandRepo.GetAll().ToList();
            modelCreateAdViewModel.bikeCategories = bikeCategoryRepo.GetAll().ToList();
            modelCreateAdViewModel.bikeEngineTypes = bikeEngineTypeRepo.GetAll().ToList();
            modelCreateAdViewModel.regions = regionRepo.GetAll().ToList();
            modelCreateAdViewModel.bikeColors = bikeColorRepo.GetAll().ToList();

            ViewBag.AdvertisementId = id;
            ViewBag.StringImage = modelAdvertisement.photoPath;
            modelCreateAdViewModel.advertisementUserId = modelAdvertisement.userId;
            modelCreateAdViewModel.description = modelAdvertisement.description;
            modelCreateAdViewModel.selectedBikeModel = modelAdvertisement.modelId;
            modelCreateAdViewModel.selectedBikeBrand = modelAdvertisement.bikeModel.brandID;
            modelCreateAdViewModel.bikeHorsePower = modelAdvertisement.horsePower;
            modelCreateAdViewModel.selectedBikeEngineType = modelAdvertisement.engineTypeId;
            modelCreateAdViewModel.bikeEngineSize = modelAdvertisement.engineSize;
            modelCreateAdViewModel.bikePrice = modelAdvertisement.price;
            modelCreateAdViewModel.selectedBikeCategory = modelAdvertisement.categoryId;
            modelCreateAdViewModel.bikeYear = modelAdvertisement.productionYear;
            modelCreateAdViewModel.bikeMileage = modelAdvertisement.mileage;
            modelCreateAdViewModel.selectedRegion = modelAdvertisement.city.regionID;
            modelCreateAdViewModel.selectedCity = modelAdvertisement.cityId;
            modelCreateAdViewModel.selectedBikeColor = modelAdvertisement.colorId;

            return View("EditAd", modelCreateAdViewModel);
        }

        [HttpPost]
        public ActionResult ViewEditAd(CreateAdViewModel model, int id)
        {
            if (!ModelState.IsValid)
            {
                return View("EditAd");
            }

            var advertisement = advertisementRepo.GetById(id);

            string uniqueFileName;

            if (model.photo != null)
                uniqueFileName = FileUpload.ProcessUploadedFile(model.photo, hostingEnvironment, "images");
            else
                uniqueFileName = advertisement.photoPath;

            advertisement.modelId = model.selectedBikeModel;
            advertisement.horsePower = model.bikeHorsePower;
            advertisement.engineSize = model.bikeEngineSize;
            advertisement.engineTypeId = model.selectedBikeEngineType;
            advertisement.productionYear = model.bikeYear;
            advertisement.mileage = model.bikeMileage;
            advertisement.price = model.bikePrice;
            advertisement.cityId = model.selectedCity;
            advertisement.colorId = model.selectedBikeColor;
            advertisement.categoryId = model.selectedBikeCategory;
            advertisement.description = model.description;
            advertisement.photoPath = uniqueFileName;

            advertisementRepo.Update(advertisement);

            return RedirectToAction("view", new { id = advertisement.id });
        }
    }
}
