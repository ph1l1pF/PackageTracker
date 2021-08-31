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

        private readonly IHttpClientFactory _clientFactory;
        private readonly IPackageService _packageService;

        public PackageController(IHttpClientFactory clientFactory, IPackageService packageService)
        {
            _clientFactory = clientFactory;
            _packageService = packageService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DhlPayload))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post(string packageTrackingNo)
        {
            var request = new HttpRequestMessage(
                            HttpMethod.Get,
                            $"https://api-eu.dhl.com/track/shipments?trackingNumber={packageTrackingNo}");
            request.Headers.Add("DHL-API-Key", Environment.GetEnvironmentVariable("DHL-API-Key"));
            var client = _clientFactory.CreateClient();
            var response = client.SendAsync(request);
            if (response.Result.IsSuccessStatusCode)
            {
                var jsonString = response.Result.Content.ReadAsStringAsync().Result;
                var payload = JsonConvert.DeserializeObject<DhlPayload>(jsonString);
                _packageService.StorePackage(payload, packageTrackingNo);
                return new OkObjectResult(payload);   
            }
            return new BadRequestResult();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Package))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get(string packageTrackingNo) 
        {
            return new OkObjectResult(_packageService.GetPackages(packageTrackingNo));
        }

    }
}
