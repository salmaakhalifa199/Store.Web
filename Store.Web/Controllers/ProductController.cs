﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Repository.Specification.ProductSpecs;
using Store.Services.Services.ProductServices;
using Store.Services.Services.ProductServices.Dtos;
using Store.Web.Helper;

namespace Store.Web.Controllers
{
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        //[Route("GetAllBrands")]
        public async Task<ActionResult<IReadOnlyList<BrandTypeDetailsDto>>> GetAllBrands()
          => Ok(await _productService.GetAllBrandsAsync());

        [HttpGet/*("GetAllTypes")*/]
        // "GetAllTypes" 34an ntfada ay error 
        public async Task<ActionResult<IReadOnlyList<BrandTypeDetailsDto>>> GetAllTypes()
          => Ok(await _productService.GetAllTypesAsync());

        [HttpGet]
        [Cache(30)]
        public async Task<ActionResult<IReadOnlyList<ProductDetailsDto>>> GetAllProducts([FromQuery]ProductSpecification input )
          => Ok(await _productService.GetAllProductsAsync(input));


        [HttpGet]
        public async Task<ActionResult<ProductDetailsDto>> GetProductById(int? id)
          =>Ok( await _productService.GetProductByIdAsync(id));
          
        // ok bt3ml bt'kda ani el status ok 200 
    }
}
