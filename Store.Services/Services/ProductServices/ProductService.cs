using AutoMapper;
using Store.Data.Entities;
using Store.Repository.Interfaces;
using Store.Repository.Specification.ProductSpecs;
using Store.Services.Helper;
using Store.Services.Services.ProductServices;
using Store.Services.Services.ProductServices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductEntity = Store.Data.Entities.Product;

namespace Store.Services.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitWork _unitWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitWork unitWork , IMapper mapper )
        {
            _unitWork = unitWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<BrandTypeDetailsDto>> GetAllBrandsAsync()
        {
            var brands = await _unitWork.Repository<ProductBrand, int>().GetAllAsNoTrackingAsync();
            var mappedBrands = _mapper.Map<IReadOnlyList<BrandTypeDetailsDto>>(brands);
            //IReadOnlyList<BrandTypeDetailsDto> mappedBrands = brands.Select(product => new BrandTypeDetailsDto
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    CreatedAt = product.CreatedAt
            //}).ToList();
            return mappedBrands;
        }

        public async Task<PaginatedResultDto<ProductDetailsDto>> GetAllProductsAsync(ProductSpecification input)
        {
            var specs = new ProductWithSpecification(input);

            var products = await _unitWork.Repository<ProductEntity, int>().GetAllWithSpecificationAsync(specs);

            var countSpecs = new ProductWithCountSpecification(input);

            var count = await _unitWork.Repository<ProductEntity, int>().GetCountSpecificationAsync(countSpecs);

            var mappedProducts =  _mapper.Map<IReadOnlyList<ProductDetailsDto>>(products);
            //var mappedProducts = products.Select(product => new ProductDetailsDto
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    Description = product.Description,
            //    PictureUrl = product.PictureUrl,
            //    CreatedAt = product.CreatedAt,
            //    BrandName = product.Brand.Name,
            //    TypeName = product.Type.Name
            //}).ToList();
            return new PaginatedResultDto<ProductDetailsDto>(input.PageIndex,input.PageSize, count, mappedProducts);
        }


        public async Task<IReadOnlyList<BrandTypeDetailsDto>> GetAllTypesAsync()
        {
            var types = await _unitWork.Repository<ProductType, int>().GetAllAsNoTrackingAsync();
            var mappedTypes = _mapper.Map<IReadOnlyList<BrandTypeDetailsDto>>(types);
            //IReadOnlyList<BrandTypeDetailsDto> mappedTypes = brands.Select(product => new BrandTypeDetailsDto
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    CreatedAt = product.CreatedAt
            //}).ToList();
            return mappedTypes;
        }

        public async Task<ProductDetailsDto> GetProductByIdAsync(int? productId)
        {
            if(productId is null )
                throw new Exception("Id is Null");

            var specs = new ProductWithSpecification(productId);

            var product = await _unitWork.Repository<ProductEntity, int>().GetAllWithSpecificationAsync(specs);


            if (product is null)
                throw new Exception("Product Not Found");

            var mappedProducts = _mapper.Map<ProductDetailsDto>(product);

            //var mappedProducts = new ProductDetailsDto
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    Description = product.Description,
            //    PictureUrl = product.PictureUrl,
            //    CreatedAt = product.CreatedAt,
            //    BrandName = product.Brand.Name,
            //    TypeName = product.Type.Name
            //};
            return mappedProducts;

        }
    }
}
