﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace WebApiVersionByHeader.Controllers.V1_1
{
    [ApiVersion("1.1")]
    [Route("api/helloworld")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        //[HttpGet]
        //[Route("get")]
        //[SwaggerParameter(name:"v",description:"api version", ParameterLocation = ParameterLocation.Query)]
        ////[MapToApiVersion("1.0")]
        //public IActionResult Get() => Ok("This is a get method, version 1.1.Hello from version 1.1.");

        [HttpPost]
        //[MapToApiVersion("1.0")]
        [Route("post")]
        [SwaggerParameter(name: "v", description: "api version", ParameterLocation = ParameterLocation.Query)]
        //[SwaggerParameter(name: "api-version", description: "api version", ParameterLocation = ParameterLocation.Header)]
        public IActionResult Post(ApiVersion apiVersion) =>
            Ok($"This is a post method, major version is {apiVersion.MajorVersion}," +
                $"minor Version is {apiVersion.MinorVersion},status is {apiVersion.Status}," +
                $"group version is {apiVersion.GroupVersion}.Hello from version 1.1.");
    }
}
