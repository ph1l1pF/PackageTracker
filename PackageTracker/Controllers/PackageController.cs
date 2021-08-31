using System;
using System.Collections.Generic;
using System.Net.Http;
using PackageTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PackageTracker.Services;

namespace PackageTracker.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PackageController
    {

        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DhlPayload))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post(string packageTrackingNo, string productDescription)
        {
            var package = _packageService.StorePackage(packageTrackingNo, productDescription);
            if(package != null)
                return new OkObjectResult(package);
            return new BadRequestResult();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Package))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get(string packageTrackingNo) 
        {
            return new OkObjectResult(_packageService.GetPackages(packageTrackingNo));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("StartUpdating")]
        public IActionResult StartUpdating() 
        {
            _packageService.StartUpdatingPackages();
            return new OkObjectResult(null);
        }
    }
}
