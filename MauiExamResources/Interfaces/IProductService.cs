using MauiExamResources.Models;

namespace MauiExamResources.Interfaces;

public interface IProductService<T, TResult> where T : class where TResult : class
{
    ResponseResult<TResult> CreateProduct(T product);
    ResponseResult<IEnumerable<TResult>> GetAllProducts();
    ResponseResult<TResult> UpdateProduct(string id, Product updatedProduct);
    ResponseResult<TResult> DeleteProduct(string id);

}